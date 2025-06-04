using System;
using System.Diagnostics;
using System.Net.WebSockets;

namespace DynamicWebTWAIN.Service
{
    public class Service : IService
    {
        public Uri BaseAddress { get; private set; }

        public Uri NormalBaseAddress { get; private set; }
        
        internal Process Process { get; private set; }

        private ClientWebSocket _websocket = new ClientWebSocket();
        // The cancellation token used to cancel the WebSocket connection.
        private System.Threading.CancellationToken _token = new System.Threading.CancellationToken();

        public bool IsAlive
        {
            get
            {
                if (Process == null)
                {// is service, not created by ServiceManager
                    return true;
                }

                return !this.Process.HasExited;
            }
        }

        internal Service(Uri baseAddress, Uri normalBaseAddress, Process process)
        {
            BaseAddress = baseAddress;
            NormalBaseAddress = normalBaseAddress;
            Process = process;

            // create a websocket connection, avoid to be closed auto
            _websocket.Options.AddSubProtocol("dwt_command");
            var wsService = NormalBaseAddress.AbsoluteUri;
            wsService = wsService.Replace("http://", "ws://");
            _websocket.ConnectAsync(new Uri(wsService), _token);
        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Process != null)
                {
                    try
                    {
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

                        // or we can send message to exit service
                        Process.Kill();
                    }
                    catch (System.Exception ex) // Fixed: Changed to System.Exception
                    {
                        Trace.WriteLine("Error closing service: " + ex.Message); // Fixed: Corrected property name to 'Message'
                    }
                }
            }
        }
    }
}
