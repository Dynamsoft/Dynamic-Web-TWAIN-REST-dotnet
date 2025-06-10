using DynamicWebTWAIN.RestClient;
using DynamicWebTWAIN.Service;
using Dynamsoft.DocumentViewer;

namespace MauiHybridWebViewApp
{
    public class WpfWebViewBridge : IWebViewBridge
    {
        private Microsoft.Maui.Controls.HybridWebView _webView;

        public WpfWebViewBridge(Microsoft.Maui.Controls.HybridWebView webView)
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
                if (args.Message != null) // Ensure args.Message is not null
                {
                    callback?.Invoke(args.Message);
                }
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

    public partial class MainPage : ContentPage
    {
        private Dynamsoft.DocumentViewer.JSInterop _jsInterop;
        private ServiceManager _serviceManager;
        private IReadOnlyList<Scanner> _scanners;
        private string productKey = "DLS2eyJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSJ9";

        public MainPage()
        {
            InitializeComponent();
            webView.SetInvokeJavaScriptTarget(this);

            // Initialize non-nullable fields to avoid CS8618
            _jsInterop = null!;
            _serviceManager = null!;
        }

        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                JSInteropOptions options = new JSInteropOptions();
                options.ProductKey = productKey;

#if WINDOWS || MACCATALYST
                options.SiteUrl = "index.html";
#else
                options.SiteUrl = "mobile.html";
                options.UIConfig = "mobile-default";
#endif
                options.MessageType = "__RawMessage";

                var bridge = new WpfWebViewBridge(webView);

#if WINDOWS || MACCATALYST
                _serviceManager = new ServiceManager(); // "dynamsoft.dwt.service"
                _serviceManager.CreateService();
                _jsInterop = new Dynamsoft.DocumentViewer.JSInterop(options,
                    bridge,
                    _serviceManager.Service.BaseAddress);
#else
                _jsInterop = new Dynamsoft.DocumentViewer.JSInterop(options,
                    bridge,
                    _serviceManager.Service.BaseAddress);
#endif
                await _jsInterop.EnsureInitializedAsync();

                _scanners = await _jsInterop.DWTClient.ScannerControlClient.ScannerManager.GetScanners(DynamicWebTWAIN.RestClient.EnumDeviceTypeMask.DT_TWAINSCANNER);

                foreach (var scanner in _scanners)
                {
                    cbxSources.Items.Add(scanner.Name);
                }

                cbxSources.SelectedIndex = 0;
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

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _jsInterop.DWTClient?.Dispose();
            _serviceManager?.Dispose();
        }

        private async void btnSaveAsPdf_Click(object sender, EventArgs e)
        {
            try
            {
                var pdf = await _jsInterop.SaveAsPdf(PageOption.All, PdfPageType.PageDefault, SaveAnnotationMode.Annotation, "");
                string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MauiHybridWebViewAppOutput.pdf");
                File.WriteAllBytes(filePath, pdf);
                await DisplayAlert("Save", filePath, "OK");
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

        private async void btnScanToView_Click(object sender, EventArgs e)
        {
            try
            {
                CreateScanJobOptions options = new CreateScanJobOptions();
                options.Device = _scanners[cbxSources.SelectedIndex].Device;
                options.AutoRun = false;
                options.RequireWebsocket = true;
#if ANDROID || IOS
                // default we use self-signed certificate, so we need to set this to false
                options.RequireWebsocket = false;
#endif
                options.Config = new ScannerConfiguration();
                //options.Config.XferCount = 7;
                options.Config.IfFeederEnabled = true;
                options.Config.IfDuplexEnabled = false;
                await _jsInterop.ScanImageToView(options);
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
    }

}
