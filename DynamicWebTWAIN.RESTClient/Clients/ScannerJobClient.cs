using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// ScannerJobClient class is used to manage scanner jobs and communicate with the scanner device.
    /// </summary>
    public class ScannerJobClient : ApiClient, IScannerJobClient
    {
        // The WebSocket client used for communication with the scanner device.
        private ClientWebSocket _websocket = new ClientWebSocket();

        // The task used for initialization of the WebSocket connection.
        private Task _initializationTask;
        // The task used for listening to messages from the WebSocket.
        private Task _websocketTask = null;
        // The cancellation token used to cancel the WebSocket connection.
        private System.Threading.CancellationToken _token = new System.Threading.CancellationToken();
        // The number of pages scanned.
        private int _scannedPages = 0;

        // Event triggered when a page is scanned.
        public event PageScannedEventHandler PageScanned;
        // Event triggered when the transfer of all pages is ended.
        public event EventHandler TransferEnded;
        // Event triggered when the job is deleted.
        public delegate void PageScannedEventHandler(object sender, PageScannedEventArgs e);
        // The job associated with this client.
        public ScannerJob ScannerJob { get; private set; }
        // The API connection used for communication with the server.
        public bool HasWebSocketConnection { get; private set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScannerJobsClient"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection.</param>
        public ScannerJobClient(IApiConnection apiConnection, ScannerJob job) : base(apiConnection)
        {
            Ensure.ArgumentNotNull(apiConnection, nameof(apiConnection));
            Ensure.ArgumentNotNull(job, nameof(job));

            ScannerJob = job;
            
            _initializationTask = InitializeAsync();
        }

        /// <summary>
        /// Initializes the WebSocket connection to the scanner device.
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync()
        {
            if (ScannerJob.Protocol != null)
            {
                _websocket.Options.AddSubProtocol(ScannerJob.Protocol.Websocket.Protocol);
                _websocket.Options.SetRequestHeader("Origin", ApiConnection.Connection.GetHttpHeader("Origin"));
                await _websocket.ConnectAsync(new Uri(ScannerJob.Protocol.Websocket.Server), _token);

                if (_websocket.State == WebSocketState.Open)
                {
                    byte[] message = Encoding.UTF8.GetBytes(ApiConnection.Connection.Serialize(ScannerJob.Protocol.Websocket.Response));
                    await _websocket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, _token);

                    _websocketTask = Task.Run(() => WebsocketListen());
                    HasWebSocketConnection = true;
                }
            }
        }

        /// <summary>
        /// Gets the value of a specific key from a JsonObject.
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private JsonObject GetObjectValue(JsonObject keyValuePairs, string key)
        {
            if (keyValuePairs == null)
                return null;
            JsonObject value = null;
            keyValuePairs.TryGetValue(key, out object obj);
            if (obj != null)
                value = obj as JsonObject;
            return value;
        }

        /// <summary>
        /// Gets the string value of a specific key from a JsonObject.
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetStringValue(JsonObject keyValuePairs, string key)
        {
            if (keyValuePairs == null)
                return null;
            string value = null;
            keyValuePairs.TryGetValue(key, out object obj);
            if (obj != null)
                value = obj.ToString();
            return value;
        }

        /// <summary>
        /// Listens for messages from the WebSocket connection and processes them.
        /// </summary>
        /// <returns></returns>
        private async Task WebsocketListen()
        {
            try
            {
                // define buffer here and reuse, to avoid more allocation
                const int chunkSize = 1024 * 1024;
                var buffer = new ArraySegment<byte>(new byte[chunkSize]);

                do
                {
                    WebSocketReceiveResult result = null;
                    while (true)
                    {
                        result = await _websocket.ReceiveAsync(buffer, _token);
                        if (result.EndOfMessage)
                            break;
                    }

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        do
                        {
                            string response = Encoding.UTF8.GetString(buffer.ToArray(), 0, result.Count);
                            //Console.WriteLine(response);
                            JsonObject st = SimpleJson.DeserializeObject(response) as JsonObject;
                            if (st != null)
                            {
                                var evt = GetObjectValue(st, "evt");
                                if (evt == null)
                                    break;
                                var name = GetStringValue(evt, "name");
                                if (name == null)
                                    break;
                                
                                if (name == "onPostTransferAsync" && this.PageScanned != null)
                                {
                                    var info = GetObjectValue(evt, "info");
                                    if (info == null)
                                        break;
                                    ++_scannedPages;
                                    PageScannedEventArgs args = new PageScannedEventArgs();
                                    args.Raw = SimpleJson.SerializeObject(info);       
                                    args.Uid = GetStringValue(info, "imageuid");
                                    args.Url = GetStringValue(info, "url");
                                    args.PageNumber = _scannedPages;
                                    PageScanned.Invoke(this, args);
                                }
                                else if (name == "onPostAllTransferAsync" && this.TransferEnded != null)
                                {
                                    TransferEnded.Invoke(this, EventArgs.Empty);
                                }
                            }
                        } while (false);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        return;
                    }


                } while (_websocket.State == WebSocketState.Open && !_token.IsCancellationRequested);
            }
            catch (TaskCanceledException)
            {   
                // task was canceled, ignore
            }
            catch (OperationCanceledException)
            {
                // operation was canceled, ignore
            }
            catch (ObjectDisposedException)
            {
                // client was disposed, ignore
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Clears the resources used by the WebSocket connection.
        /// </summary>
        private void ClearResources()
        {
            // Close and dispose the WebSocket
            if (_websocket != null)
            {
                try
                {
                    if (_websocket.State == WebSocketState.Open || _websocket.State == WebSocketState.CloseReceived)
                    {
                        _websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Clearing resources", _token).Wait();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error while closing WebSocket: {ex.Message}");
                }
                finally
                {
                    _websocket.Dispose();
                    _websocket = null;
                }
            }

            // Wait for the WebSocket listening task to complete
            if (_websocketTask != null)
            {
                try
                {
                    _websocketTask.Wait();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error while waiting for WebSocket task: {ex.Message}");
                }
                finally
                {
                    _websocketTask = null;
                }
            }

            if (_initializationTask != null)
            {
                try
                {
                    _initializationTask.Wait();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error while waiting for initialization task: {ex.Message}");
                }
                finally
                {
                    _initializationTask = null;
                }
            }
        }

        /// <summary>
        /// Ensures that the WebSocket connection is initialized.
        /// </summary>
        /// <returns></returns>
        public Task EnsureInitializedAsync() => _initializationTask;

        /// <summary>
        /// Gets the number of pages scanned.
        /// </summary>
        /// <returns></returns>
        public int GetPageCount()
        {
            return this._scannedPages;
        }

        /// <summary>
        /// Gets the next image from the scanner job.
        /// </summary>
        /// <returns></returns>
        public Task<byte[]> GetNextImage()
        {
            return GetNextImage(OutputFormat.ImageJpeg);
        }

        /// <summary>
        /// Gets the next image from the scanner job in the specified format.
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        [ManualRoute("GET", "/api/device/scanners/jobs/{jobuid}/next-page")]
        public Task<byte[]> GetNextImage(StringEnum<OutputFormat> Format)
        {
            Ensure.ArgumentNotNull(Format, nameof(Format));

            var query = new Dictionary<string, string>
            {
                { "type", Format.ToString() }
            };
            return ApiConnection.GetRaw(ApiUrls.ScannerJobPage(ScannerJob.Jobuid), query);
        }

        /// <summary>
        /// Gets the status of the scanner job.
        /// </summary>
        /// <returns></returns>
        [ManualRoute("GET", "/api/device/scanners/jobs/{jobuid}")]
        public Task<ScannerJobStatus> GetJobStatus()
        {
            return ApiConnection.Get<ScannerJobStatus>(ApiUrls.ScannerJob(ScannerJob.Jobuid));
        }

        /// <summary>
        /// Starts the scanner job if it is in a pending state.
        /// </summary>
        /// <returns></returns>
        public Task<StringEnum<JobStatus>> StartJob()
        {
            if (JobStatus.Pending == ScannerJob.Status)
            {
                return UpdateJobStatus(JobStatus.Running);
            }
            
            return Task.FromResult(ScannerJob.Status);
        }

        /// <summary>
        /// Updates the status of the scanner job.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [ManualRoute("PATCH", "/api/device/scanners/jobs/{jobuid}")]
        public async Task<StringEnum<JobStatus>> UpdateJobStatus(JobStatus status)
        {
            var requestData = new
            {
                Status = status
            };
            var jobStatus = await ApiConnection.Patch<ScannerJobStatus>(ApiUrls.ScannerJob(ScannerJob.Jobuid), requestData);
            return jobStatus.Status;
        }

        /// <summary>
        /// Make sure to call this method to delete the job and close the websocket connection
        /// </summary>
        /// <returns></returns>
        [ManualRoute("DELETE", "/api/device/scanners/jobs/{jobuid}")]
        public async Task DeleteJob()
        {
            await ApiConnection.Delete(ApiUrls.ScannerJob(ScannerJob.Jobuid));
            ClearResources();
        }

        /// <summary>
        /// Gets the image by URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [ManualRoute("Get", "/api/device/scanners/jobs/{jobuid}")]
        public Task<byte[]> GetImageByUrl(string url)
        {
            Ensure.ArgumentNotNull(url, nameof(url));
            return ApiConnection.GetRaw(new Uri(url), null);
        }


        /// <summary>
        /// Gets the document URL for the specified output.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public Uri GetDocumentUrl(DocumentOutput output)
        {
            Ensure.ArgumentNotNull(output, nameof(output));

            Uri uri = new Uri(Connection.BaseAddress, ApiUrls.ScannerJobContent(ScannerJob.Jobuid));
            return uri.ApplyParameters(output.ToParametersDictionary());
        }

        /// <summary>
        /// Gets the document content for the specified output.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        [ManualRoute("Get", "/api/device/scanners/jobs/{jobuid}/content")]
        public Task<byte[]> GetDocument(DocumentOutput output)
        {
            Ensure.ArgumentNotNull(output, nameof(output));
            return ApiConnection.GetRaw(ApiUrls.ScannerJobContent(ScannerJob.Jobuid), output.ToParametersDictionary());
        }
    }

    /// <summary>
    /// Event arguments for the PageScanned event.
    /// </summary>
    public class PageScannedEventArgs : EventArgs
    {
        public string Raw { get; set; }
        public string Url { get; set; }
        public string Uid { get; set; }

        public int PageNumber { get; set; }
    }

}
