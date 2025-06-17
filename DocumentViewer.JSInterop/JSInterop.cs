using DynamicWebTWAIN.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Dynamsoft.DocumentViewer
{
    public class JSInterop
    {
        private DWTClient _client;
        private Task _initializationTask;

        public JSInterop(JSInteropOptions options, IWebViewBridge webview)
            : this(options, webview, DWTClient.DWTApiUrl)
        {

        }

        public JSInterop(JSInteropOptions options, IWebViewBridge webview, Uri dwtServiceUrl)
        {
            Ensure.ArgumentNotNull(options, nameof(options));
            Ensure.ArgumentNotNull(options.ProductKey, nameof(options.ProductKey));
            Ensure.ArgumentNotNull(webview, nameof(webview));

            Options = options;
            WebView = webview;

            if (null != dwtServiceUrl)
            {
                _client = new DWTClient(dwtServiceUrl, Options.ProductKey);
            }
            else
            {
#if WINDOWS || MACCATALYST
                _client = new DWTClient(Options.ProductKey);
#else
                _client = null;
#endif
            }

            WebView.RegisterCallback(HandleJavascriptCallback);

            _initializationTask = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            Uri pageUrl = new Uri(_client.BaseAddress, "app/site/default/index.html");
            if (Options.SiteUrl != null)
            {
                if (Uri.IsWellFormedUriString(Options.SiteUrl, UriKind.Absolute))
                {
                    pageUrl = new Uri(Options.SiteUrl, UriKind.Absolute);
                }
                else
                {// handle relative url
                    pageUrl = new Uri(Options.SiteUrl, UriKind.Relative);
                }
            }

            var task = CreateJSTask("load");

            await WebView.LoadUrlAsync(pageUrl);

            // wait for DOMContentLoaded
            var resultList = await task.Task;
            if (resultList == null || resultList.Count != 3)
            {
                throw new InvalidOperationException("Failed to initialize document viewer");
            }

            // Convert the anonymous type to an object array to match the expected parameter type
            var parameters = new object[]
            {
                new
                {
                    productKey = Options.ProductKey,
                    messageType = string.IsNullOrEmpty(Options.MessageType) ? "" : Options.MessageType,
                    uiConfig = Options.UIConfig ?? "desktop-default",
                }
            };
            var result = await ExecuteJavaScript("initView", parameters);
            if (result == null || !(result is string))
            {
                throw new InvalidOperationException("Failed to initialize document viewer");
            }

            _client.AddHttpHeader("Origin", result as string);// let ddv has the same origin with rest api            
        }

        private bool HandleJavascriptCallback(string message)
        {
            Ensure.ArgumentNotNull(message, nameof(message));

            string context, error;
            object result;
            if (!ParseJavascriptResult(message, out context, out result, out error))
            {
                throw new InvalidOperationException("Invalid message type");
            }

            lock (_javascriptCallTasks)
            {
                if (_javascriptCallTasks.TryGetValue(context, out var tcs))
                {
                    List<object> results = new List<object>();
                    results.Add(context);
                    results.Add(result);
                    results.Add(error);
                    // Complete the TaskCompletionSource with the result
                    tcs.SetResult(results);
                    _javascriptCallTasks.Remove(context);
                    return true;
                }
            }

            // check name in message, we should predfine some supported function name
            if (context == "onClickSaveToPdf" && result is string)
            {// may be we should return a format, not only pdf??????
                var byteArray = Convert.FromBase64String(result as string);
                DocumentSaved?.Invoke(this, new DocumentSavedEventArgs(byteArray, OutputFormat.ApplicationPdf));
                return true;
            }

            return false;
        }

        private TaskCompletionSource<List<object>> CreateJSTask(string name)
        {
            var tcs = new TaskCompletionSource<List<object>>();

            // Add the TaskCompletionSource to the dictionary
            lock (_javascriptCallTasks)
            {
                _javascriptCallTasks[name] = tcs;
            }

            return tcs;
        }

        private void RemoveJSTask(string name)
        {
            lock (_javascriptCallTasks)
            {
                if (_javascriptCallTasks.ContainsKey(name))
                {
                    _javascriptCallTasks.Remove(name);
                }
            }
        }

        private readonly Dictionary<string, TaskCompletionSource<List<object>>> _javascriptCallTasks = new Dictionary<string, TaskCompletionSource<List<object>>>();

        public Task EnsureInitializedAsync() => _initializationTask;

        public JSInteropOptions Options { get; private set; }
        public IWebViewBridge WebView { get; private set; }
        public DWTClient DWTClient
        {
            get
            {
                return _client;
            }
            set
            {
                if (_client == value) return;
                if (_client != null)
                {
                    _client.Dispose();
                    _client = null;
                }
                _client = value;
            }
        }

        private async Task<object> ExecuteJavaScript(string name)
        {
            return await ExecuteJavaScript(name, null);
        }

        private async Task<object> ExecuteJavaScript(string name, object[] parameters)
        {
            var taskName = $"{name}-{Guid.NewGuid().ToString()}";
            var task = CreateJSTask(taskName);
            try
            {
                JsonArray jsonArray = new JsonArray();
                if (parameters != null && parameters.Length > 0)
                {
                    foreach (var item in parameters)
                    {
                        jsonArray.Add(item);
                    }
                }

                var message = await WebView.ExecuteJavaScriptAsync(
                    $"invokeJavaScript('{name}', '{jsonArray.ToString()}', '{taskName}')");

                // javascript return structure [context, result, error]
                string context, error;
                object result;

                // wpf webview2 return null, winforms webview2 return "{}"
                bool validReturn = ParseJavascriptResult(message, out context, out result, out error);

                // wpf webview2 return null, winforms webview2 return "{}"
                if (!validReturn)
                {// is async method, we should wait for message from javascript
                 // wait for message from javascript
                 // if we call async method in javascript, we should wait message from javascript
                    var resultList = await task.Task;
                    if (resultList == null || resultList.Count != 3)
                    {
                        throw new InvalidOperationException("Invalid result format from javascript");
                    }
                    result = resultList[1];
                    error = resultList[2] as string;
                    // context = resultList[0] as string; should be equal to taskName
                }

                if (!string.IsNullOrEmpty(error))
                {// may be we should define a new exception type
                    throw new InvalidOperationException(error);
                }

                return result;
            }
            finally
            {
                RemoveJSTask(taskName);
            }
        }

        public static bool ParseJavascriptResult(string message,
            out string context, out object result, out string error)
        {
            // javascript return structure, json array [context, result, error]
            context = null;
            result = null;
            error = null;

            if (string.IsNullOrEmpty(message))
            {
                return false;
            }

            if (message.StartsWith("__RawMessage|["))
            {// this is hyberidwebview message, we can remove this header
                message = message.Substring("__RawMessage|".Length);// keep [, is array start flag
            }

            var st = SimpleJson.DeserializeObject(message);
            if (st == null)
            {
                return false;
            }
            if (st is IEnumerable<object> array)
            {
                int index = 0;
                foreach (var item in array)
                {
                    if (index == 0 && item is string str1)
                    {
                        context = str1;
                    }
                    else if (index == 1)
                    {
                        result = item;
                    }
                    else if (index == 2)
                    {
                        error = item.ToString();
                    }
                    else
                    {
                        return false;
                    }

                    ++index;
                }
            }

            return (null != context && null != result && null != error);
        }

        public event EventHandler<DocumentSavedEventArgs> DocumentSaved;

        private ScannerJobManager _scannerJobManager = new ScannerJobManager();


        /// <summary>
        /// You can create a scan job with the options you want. once the job is created, you can start the scan job later.
        /// Now, your scanner was locked by your job, others cannot access the scanner.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<IScannerJobClient> CreateScanToViewJob(CreateScanJobOptions options)
        {
            Ensure.ArgumentNotNull(options, nameof(options));

            var jobClient = await DWTClient.ScannerControlClient.ScannerJobs.CreateJob(options);
            try
            {
                _scannerJobManager.AddJob(jobClient.ScannerJob.Jobuid, jobClient);

                if ((bool)!options.AutoRun && options.RequireWebsocket)
                {
                    jobClient.PageScanned += async (sender, e) =>
                    {
                        DocumentOutput ot = new DocumentOutput();
                        ot.Format = OutputFormat.ImageJpeg;
                        ot.Pages = (e.PageNumber - 1).ToString();
                        var pageUri = jobClient.GetDocumentUrl(ot);
                        var jsCallback = $"[{{\"name\": \"{jobClient.ScannerJob.Jobuid}\"}}, \"{e.PageNumber}\"]";
                        _scannerJobManager.AddPage(jobClient.ScannerJob.Jobuid, e.PageNumber);
                        try
                        {
                            await ExecuteJavaScript("loadDocument", new object[] { pageUri });
                            await _scannerJobManager.RemovePage(jobClient.ScannerJob.Jobuid, e.PageNumber);
                        }
                        catch (Exception ex)
                        {
                            // Handle the exception if needed
                            Console.WriteLine($"Error loading document: {ex.Message}, delete the job directly");
                            await _scannerJobManager.DeleteJob(jobClient.ScannerJob.Jobuid);
                        }
                    };

                    jobClient.TransferEnded += async (sender, e) =>
                    {
                        await _scannerJobManager.SetJobTransferEnded(jobClient.ScannerJob.Jobuid);
                    };
                }
            }
            catch
            {
                try
                {
                    if (!await _scannerJobManager.DeleteJob(jobClient.ScannerJob.Jobuid))
                    {
                        await jobClient.DeleteJob();
                    }
                }
                catch
                {
                }
            }

            return jobClient;
        }

        public async Task StartJob(IScannerJobClient jobClient)
        {
            Ensure.ArgumentNotNull(jobClient, nameof(jobClient));

            try
            {
                await jobClient.StartJob();

                if (!jobClient.HasWebSocketConnection)
                {
                    do
                    {
                        //StringEnum<OutputFormat> type = OutputFormat.ImageJpeg;
                        // require jpeg, returned is may be not a jpeg image
                        var result = await jobClient.GetNextImage();
                        if (result == null)
                        {
                            await jobClient.DeleteJob();
                            break;
                        }
                        else
                        {
                            await ExecuteJavaScript("loadSource", new object[] { Convert.ToBase64String(result) });
                        }
                    } while (true);
                }
            }
            catch
            {
                try
                {
                    await jobClient.DeleteJob();
                }
                catch
                {
                }
            }

        }

        public async Task ScanImageToView(CreateScanJobOptions options)
        {
            var jobClient = await CreateScanToViewJob(options);

            await StartJob(jobClient);
        }

        public Task SetCursorToPan()
        {
            return SetToolMode("pan");
        }

        public Task SetCursorToCrop()
        {
            return SetToolMode("crop");
        }

        private async Task SetToolMode(string mode)
        {
            await ExecuteJavaScript("setToolMode", new object[] { mode });
        }

        public Task RotateLeft(bool currentOnly)
        {
            return Rotate(currentOnly, -90);
        }

        public Task RotateRight(bool currentOnly)
        {
            return Rotate(currentOnly, 90);
        }

        private async Task Rotate(bool currentOnly, float angle)
        {
            await ExecuteJavaScript(currentOnly ? "rotateCurrentPage" : "rotateSelectedPages", new object[] { angle });
        }
        public Task Crop(bool currentOnly)
        {
            return ExecuteJavaScript(currentOnly ? "cropCurrentPage" : "cropSelectedPages");
        }

        private static readonly string[] AnnotationModeNames = new string[]
        {
                        "rectangle",
                        "ellipse",
                        "line",
                        "polyline",
                        "ink",
                        "textBox",
                        "textTypewriter",
                        "stamp"
        };
        public Task SetAnnotationMode(AnnotationMode mode)
        {
            if (mode < AnnotationMode.Rectangle || mode > AnnotationMode.Stamp)
            {
                throw new ArgumentOutOfRangeException(nameof(mode), "Invalid annotation mode");
            }

            return ExecuteJavaScript("setAnnotationMode", new object[] { AnnotationModeNames[(int)mode] });
        }

        public Task Undo()
        {
            return ExecuteJavaScript("undo");
        }
        public Task Redo()
        {
            return ExecuteJavaScript("redo");
        }

        public Task ZoomToFitWindow()
        {
            return SetZoomFitMode("window");
        }

        public Task ZoomToActualSize()
        {
            return SetZoomFitMode("actualSize");
        }

        private async Task SetZoomFitMode(string mode)
        {
            await ExecuteJavaScript("setFitMode", new object[] { mode });
        }

        public Task DeleteCurrent()
        {
            return ExecuteJavaScript("deleteCurrentPage");
        }

        public Task DeleteSelected()
        {
            return ExecuteJavaScript("deleteSelectedPages");
        }

        public Task DeleteAll()
        {
            return ExecuteJavaScript("deleteAllPages");
        }

        public async Task<UInt32> GetSelectedPagesCount()
        {
            var result = await ExecuteJavaScript("getSelectedPagesCount");
            if (result == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToUInt32(result);
            }
        }

        public async Task<UInt32> GetPageCount()
        {
            var result = await ExecuteJavaScript("getPageCount");
            if (result == null || Convert.ToInt32(result) == -1)
            {
                return 0;
            }
            else
            {
                return Convert.ToUInt32(result);
            }
        }

        public async Task LoadFile(byte[] file)
        {
            await ExecuteJavaScript("loadSource", new object[] { Convert.ToBase64String(file) });
        }

        public async Task<byte[]> SaveAsJpeg(bool saveAnnotation)
        {
            JsonObject settings = new JsonObject();
            settings.Add("quality", 80);
            settings.Add("saveAnnotation", saveAnnotation);
            var result = await ExecuteJavaScript("saveCurrentToJpeg", new object[] { settings });
            return Convert.FromBase64String(result as string);
        }

        public async Task<byte[]> SaveAsPng(bool saveAnnotation)
        {
            JsonObject settings = new JsonObject();
            settings.Add("saveAnnotation", saveAnnotation);
            var result = await ExecuteJavaScript("saveCurrentToPng", new object[] { settings });
            return Convert.FromBase64String(result as string);
        }

        public async Task<byte[]> SaveAsTiff(PageOption pageOption, bool saveAnnotation)
        {
            // Set custom tag
            //const customTag1 = {
            //    id: 700,
            //    content: "Created By Dynamsoft",
            //    contentIsBase64: false,
            //}

            // Set SaveTiffSettings
            //const tiffSettings = {
            //    customTag: [customTag1],
            //    compression: "tiff/auto",
            //}// Set custom tag
            if (pageOption < PageOption.All || pageOption > PageOption.Selected)
            {
                throw new ArgumentOutOfRangeException(nameof(pageOption), "Invalid page option");
            }
            JsonObject settings = new JsonObject();
            settings.Add("saveAnnotation", saveAnnotation);

            string functionName = "saveCurrentAsTiff";
            switch (pageOption)
            {
                case PageOption.All:
                    functionName = "saveAllAsTiff";
                    break;
                case PageOption.Current:
                    functionName = "saveCurrentAsTiff";
                    break;
                case PageOption.Selected:
                    functionName = "saveSelectedAsTiff";
                    break;
            }

            var result = await ExecuteJavaScript(functionName, new object[] { settings });
            return Convert.FromBase64String(result as string);
        }

        public async Task<byte[]> SaveAsPdf(PageOption pageOption, PdfPageType pdfPageType,
            SaveAnnotationMode saveAnnotationMode, string password)
        {
            //const pdfSettings = {
            //    author: "Dynamsoft",
            //    compression: "pdf/jpeg",
            //    pageType: "page/a4",
            //    creator: "DDV",
            //    creationDate: "D:20230101085959-08'00'",
            //    keyWords: "samplepdf",
            //    modifiedDate: "D:20230101090101-08'00'",
            //    producer: "Dynamsoft Document Viewer",
            //    subject: "SamplePdf",
            //    title: "SamplePdf",
            //    version: "1.5",
            //    quality: 90,
            //    password: "dynamsoft",
            //    saveAnnotation: "annotation",
            //    imageScaleFactor: 1,
            //};
            if (pageOption < PageOption.All || pageOption > PageOption.Selected)
            {
                throw new ArgumentOutOfRangeException(nameof(pageOption), "Invalid page option");
            }
            if (pdfPageType < PdfPageType.PageDefault || pdfPageType > PdfPageType.PageLegalReverse)
            {
                throw new ArgumentOutOfRangeException(nameof(pdfPageType), "Invalid page type");
            }
            if (saveAnnotationMode < SaveAnnotationMode.None || saveAnnotationMode > SaveAnnotationMode.Flatten)
            {
                throw new ArgumentOutOfRangeException(nameof(saveAnnotationMode), "Invalid save annotation mode");
            }
            JsonObject settings = new JsonObject();
            settings.Add("pageType", PdfPageTypeHelper.ToStringValue(pdfPageType));
            settings.Add("saveAnnotation", SaveAnnotationModeHelper.ToStringValue(saveAnnotationMode));
            settings.Add("password", password);
            settings.Add("quality", 80);
            string functionName = "saveCurrentAsPdf";
            switch (pageOption)
            {
                case PageOption.All:
                    functionName = "saveAllAsPdf";
                    break;
                case PageOption.Current:
                    functionName = "saveCurrentAsPdf";
                    break;
                case PageOption.Selected:
                    functionName = "saveSelectedAsPdf";
                    break;
            }
            var result = await ExecuteJavaScript(functionName, new object[] { settings });
            return Convert.FromBase64String(result as string);
        }

        public async Task<string> ReadBarcode(byte[] bytes, string templateName = "coverage")
        {
            Ensure.ArgumentNotNull(bytes, nameof(bytes));

            var ret = await DWTClient.DocumentProcessClient.ReadBarcodeByArray(bytes, templateName);
            //var st = SimpleJson.DeserializeObject(ret);
            //var result = await ExecuteJavaScript("clearAnnotations", new object[] { });
            //result = await ExecuteJavaScript("setBarcodeResult", new object[] { st });

            return ret;
        }
    }

    public class DocumentSavedEventArgs : EventArgs
    {
        public byte[] Content { get; }
        public OutputFormat Format { get; set; }

        public DocumentSavedEventArgs(byte[] content, OutputFormat format)
        {
            Content = content;
            Format = format;
        }
    }

    public enum AnnotationMode
    {
        Rectangle = 0,
        Ellipse,
        Line,
        Polyline,
        Ink,
        TextBox,
        TextTypewriter,
        Stamp
    }

    public enum PageOption
    {
        All = 0,
        Current,
        Selected
    }

    public enum PdfPageType
    {
        PageDefault,
        PageA4,
        PageA4Reverse,
        PageA3,
        PageA3Reverse,
        PageLetter,
        PageLetterReverse,
        PageLegal,
        PageLegalReverse
    }

    public static class PdfPageTypeHelper
    {
        private static readonly Dictionary<PdfPageType, string> EnumToStringMap = new Dictionary<PdfPageType, string>
        {
            { PdfPageType.PageDefault, "page/default" },
            { PdfPageType.PageA4, "page/a4" },
            { PdfPageType.PageA4Reverse, "page/a4reverse" },
            { PdfPageType.PageA3, "page/a3" },
            { PdfPageType.PageA3Reverse, "page/a3reverse" },
            { PdfPageType.PageLetter, "page/letter" },
            { PdfPageType.PageLetterReverse, "page/letterreverse" },
            { PdfPageType.PageLegal, "page/legal" },
            { PdfPageType.PageLegalReverse, "page/legalreverse" }
        };

        public static string ToStringValue(PdfPageType pageType)
        {
            return EnumToStringMap[pageType];
        }

        public static PdfPageType FromStringValue(string value)
        {
            return EnumToStringMap.FirstOrDefault(x => x.Value == value).Key;
        }

        public static string[] ToStringArray()
        {
            return EnumToStringMap.Values.ToArray();
        }
    }

    public enum SaveAnnotationMode
    {
        None,
        Image,
        Annotation,
        Flatten
    }

    public static class SaveAnnotationModeHelper
    {
        private static readonly Dictionary<SaveAnnotationMode, string> EnumToStringMap = new Dictionary<SaveAnnotationMode, string>
        {
            { SaveAnnotationMode.None, "none" },
            { SaveAnnotationMode.Image, "image" },
            { SaveAnnotationMode.Annotation, "annotation" },
            { SaveAnnotationMode.Flatten, "flatten" }
        };

        public static string ToStringValue(SaveAnnotationMode annotation)
        {
            return EnumToStringMap[annotation];
        }

        public static SaveAnnotationMode FromStringValue(string value)
        {
            return EnumToStringMap.FirstOrDefault(x => x.Value == value).Key;
        }

        public static string[] ToStringArray()
        {
            return EnumToStringMap.Values.ToArray();
        }
    }

}
