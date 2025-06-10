using DynamicWebTWAIN.RestClient;
using Dynamsoft.DocumentViewer;
using System.Diagnostics;
using DynamicWebTWAIN.Service;

namespace DWT_REST_MAUI
{
    public class SelectedFile {
        public byte[]? imageBytes = null;
    }

    public class HybridWebViewBridge : IWebViewBridge
    {
        private Microsoft.Maui.Controls.HybridWebView _webView;

        public HybridWebViewBridge(Microsoft.Maui.Controls.HybridWebView webView)
        {
            _webView = webView;

        }

        public async Task<string?> ExecuteJavaScriptAsync(string script)
        {
            string? result = null;
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                result = await _webView.EvaluateJavaScriptAsync(script);
            });
            return result;
        }

        public void RegisterCallback(Func<string, bool> callback)
        {
            _webView.RawMessageReceived += (sender, args) =>
            {
                callback?.Invoke(args.Message);
            };
        }

        public async Task LoadUrlAsync(Uri url)
        {
            var tcs = new TaskCompletionSource();
            _webView.HybridRoot = "ddv";
            _webView.DefaultFile = url.OriginalString;
            await Task.CompletedTask;
        }

    }
    [QueryProperty(nameof(LicenseChanged), "licenseChanged")]
    public partial class MainPage : ContentPage
    {
        private ServiceManager _serviceManager;

        private Dynamsoft.DocumentViewer.JSInterop _jsInterop;
        private IScannerJobClient? scannerJob;
        private Boolean isDesktop;
        private string productKey = "DLS2eyJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSJ9";
        public static string defaultAddress = "https://127.0.0.1:18623";
        string licenseChanged;

        public string LicenseChanged
        {
            get => licenseChanged;
            set
            {
                licenseChanged = value;
                if (licenseChanged == "true")
                {
                    AskWhetherToReload();
                }
            }
        }

        public MainPage()
        {
#if WINDOWS
            isDesktop = true;
            _serviceManager = new ServiceManager(); // "dynamsoft.dwt.service"
            _serviceManager.CreateService();
            defaultAddress = _serviceManager.Service.BaseAddress.ToString();
#else
            isDesktop = false;
#endif
            InitializeComponent();
            webView.SetInvokeJavaScriptTarget(this);
            InitViewer();
            RequestCameraPermission();
        }

        private async void RequestCameraPermission() {
            PermissionStatus status = await Permissions.RequestAsync<Permissions.Camera>();
        }

        private async void AskWhetherToReload() {
            bool answer = await DisplayAlert("Question?", "License is changed. Do you want to reload the page to make it effective?", "Yes", "No");
            if (answer) {
                Debug.WriteLine("reset viewer");
                ResetViewer();
            }
            var currentPage = Shell.Current.CurrentState.Location.OriginalString;
            try
            {
                Debug.WriteLine($"{currentPage}");
                await Shell.Current.GoToAsync($"//{currentPage}"); //clear params
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
            }
        }

        private async void ResetViewer() { 
            await webView.EvaluateJavaScriptAsync("location.reload();");
            InitViewer();
        }

        private async void InitViewer()
        {
            try
            {
                Dynamsoft.DocumentViewer.JSInteropOptions options = new JSInteropOptions();
                // because we load ddv page in the service, so we should make sure the service is running, so we need to set a long timeout
                // or manual create a websocket connection in js, recommend this way.
                options.ProductKey = Preferences.Get("License", productKey);
                if (options.ProductKey == "")
                {
                    options.ProductKey = productKey;
                }
                if (isDesktop)
                {
                    options.SiteUrl = "index.html";
                }
                else {
                    options.SiteUrl = "mobile.html";
                    options.UIConfig = "mobile-default";
                }
                
                options.MessageType = "__RawMessage";

                var bridge = new HybridWebViewBridge(webView);

                if (isDesktop) {
                    _jsInterop = new Dynamsoft.DocumentViewer.JSInterop(options,
                        bridge,
                        _serviceManager.Service.BaseAddress);
                }
                else {
                    _jsInterop = new Dynamsoft.DocumentViewer.JSInterop(options,
                        bridge,
                        new Uri(defaultAddress));
                }

                await _jsInterop.EnsureInitializedAsync();
                Func<string, bool> callback = (name) =>
                {
                    if (name.Contains("loadFile"))
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            _ = PickAndShow();
                        });
                    }
                    return true;
                };
                bridge.RegisterCallback(callback);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                if (ex.Message != null)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Alert", ex.Message, "OK");
                    });
                }
            }
        }

        public async Task<SelectedFile?> PickAndShow()
        {
            SelectedFile? ret = null;
            try
            {
                PickOptions options = new()
                {
                    PickerTitle = "Please select a PDF/image file"
                };
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    if (result.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) || 
                        result.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    {
                        using var stream = await result.OpenReadAsync();
                        var bytes = await StreamToBytesAsync(stream);
                        await _jsInterop.LoadFile(bytes);
                        
                        ret = new SelectedFile();
                        ret.imageBytes = bytes;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return ret;
        }
                

        public static async Task<byte[]> StreamToBytesAsync(Stream stream)
        {
            if (stream == null) return null;

            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private async void OnActionItemClicked(object sender, EventArgs args)
        {
            string result = await DisplayActionSheet("Select an action", "Cancel", null, "Scan with scanner", "Scan with camera", 
                "Edit", "Settings", "Save as PDF", "Save selected as PNG", 
                "Open a local file");

            if (result == "Scan with scanner")
            {
                StartDocumentScanning();
            }
            else if (result == "Scan with camera")
            {
                TakePhoto();
            }
            else if (result == "Edit")
            {
                if (isDesktop) {
                    await _jsInterop.WebView.ExecuteJavaScriptAsync("invokeJavaScript('showFullFeatureEditor','[]');");
                }
                else {
                    await _jsInterop.WebView.ExecuteJavaScriptAsync("showEditor();");
                }
            }
            else if (result == "Settings")
            {
                await Shell.Current.GoToAsync("SettingsPage");
            }
            else if (result == "Save as PDF")
            {
                SaveAsPDF();
            }
            else if (result == "Save selected as PNG") 
            {
                SaveAsPNG();
            }
            else if (result == "Open a local file")
            {
                await PickAndShow();
            }
        }

        public async void TakePhoto()
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    // save the file into local storage
                    string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    using Stream sourceStream = await photo.OpenReadAsync();
                    var bytes = await StreamToBytesAsync(sourceStream);
                    await _jsInterop.LoadFile(bytes);
                }
            }
        }

        private async void StartDocumentScanning() {
            var canceled = false;
            Func<object> cancelEvent = () =>
            {
                canceled = true;
                Navigation.PopModalAsync();
                CancelScanning();
                return "";
            };
            var page = new ProgressPage();
            page.RegisterCallback(cancelEvent);
            await Navigation.PushModalAsync(page);
            Debug.WriteLine("start scanning");
            await ScanDocument();
            if (!canceled)
            {
                await Navigation.PopModalAsync();
            }
        }

        private async void SaveAsPDF() {
            var canceled = false;
            Func<object> cancelEvent = () =>
            {
                canceled = true;
                Navigation.PopModalAsync();
                return "";
            };
            var page = new ProgressPage();
            page.RegisterCallback(cancelEvent);
            await Navigation.PushModalAsync(page);
            try
            {
                PageOption pageOption = PageOption.All; // Default to "Save Current Page"
                PdfPageType pdfPageType = PdfPageType.PageDefault;
                SaveAnnotationMode annotationMode = SaveAnnotationMode.None;
                byte[] pdfContent = await _jsInterop.SaveAsPdf(pageOption,pdfPageType,annotationMode,"");
                if (canceled) {
                    return;
                }
                if (pdfContent.Length > 0) {
                    string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "out.pdf");
                    await using (var fileStream = new FileStream(targetFile, FileMode.Create, FileAccess.Write))
                    {
                        await fileStream.WriteAsync(pdfContent, 0, pdfContent.Length);
                        await Share.Default.RequestAsync(new ShareFileRequest
                        {
                            Title = "Share PDF file",
                            File = new ShareFile(targetFile)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message != null)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Alert", ex.Message, "OK");
                    });
                }
            }
            if (!canceled)
            {
                await Navigation.PopModalAsync();
            }
        }

        private async void SaveAsPNG()
        {
            try
            {
                byte[] bytes = await _jsInterop.SaveAsPng(false);
                if (bytes.Length > 0)
                {
                    string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "out.png");
                    await using (var fileStream = new FileStream(targetFile, FileMode.Create, FileAccess.Write))
                    {
                        await fileStream.WriteAsync(bytes, 0, bytes.Length);
                        await Share.Default.RequestAsync(new ShareFileRequest
                        {
                            Title = "Share PNG file",
                            File = new ShareFile(targetFile)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message != null)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Alert", ex.Message, "OK");
                    });
                }
            }

        }

        private void CancelScanning() {
            scannerJob?.DeleteJob();
        }

        private async Task<bool> ScanDocument()
        {
            Debug.WriteLine("ScanDocument");
            try
            {
                var license = Preferences.Get("License", productKey);
                if (license == "") {
                    license = productKey;
                }
                var IPAddress = Preferences.Get("IP", defaultAddress);
                var client = new DWTClient(new Uri(IPAddress), license);
                _jsInterop.DWTClient = client;
                var DPI = Preferences.Get("DPI", 150);
                var colorMode = Preferences.Get("ColorMode", "Color");
                var scannerName = Preferences.Get("Scanner", "");
                var autoFeeder = Preferences.Get("AutoFeeder", false);
                var duplex = Preferences.Get("Duplex", false);

                CreateScanJobOptions options = new CreateScanJobOptions();
                options.AutoRun = false;
                options.RequireWebsocket = false;
                options.Config = new ScannerConfiguration();
                options.Config.XferCount = 7;
                options.Config.Resolution = DPI;
                options.Config.IfFeederEnabled = autoFeeder;
                options.Config.IfDuplexEnabled = duplex;
                Debug.WriteLine(colorMode);
                if (colorMode == "Color")
                {
                    Debug.WriteLine("scan in color");
                    options.Config.PixelType = EnumDWT_PixelType.TWPT_RGB;
                }
                else if (colorMode == "BlackWhite")
                {
                    Debug.WriteLine("scan in black white");
                    options.Config.PixelType = EnumDWT_PixelType.TWPT_BW;
                }
                else if (colorMode == "Grayscale")
                {
                    Debug.WriteLine("scan in grayscale");
                    options.Config.PixelType = EnumDWT_PixelType.TWPT_GRAY;
                }
                if (!string.IsNullOrEmpty(scannerName))
                {
                    var scanners = await _jsInterop.DWTClient.ScannerControlClient.ScannerManager.GetScanners(EnumDeviceTypeMask.DT_TWAINSCANNER | EnumDeviceTypeMask.DT_WIATWAINSCANNER);
                    foreach (var scanner in scanners)
                    {
                        if (scanner.Name == scannerName)
                        {
                            options.Device = scanner.Device;
                        }
                    }
                }
                scannerJob = await _jsInterop.CreateScanToViewJob(options);
                await _jsInterop.StartJob(scannerJob);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                if (ex.Message != null)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Alert", ex.Message, "OK");
                    });
                }
            }
            return true;
        }
    }
}
