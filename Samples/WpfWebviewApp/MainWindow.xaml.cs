using DynamicWebTWAIN.RestClient;
using Dynamsoft.DocumentViewer;
using System.Windows;
using System.IO;
using DynamicWebTWAIN.Service;

namespace WpfWebviewApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public class WpfWebViewBridge : IWebViewBridge
    {
        private Microsoft.Web.WebView2.Wpf.WebView2 _webView;

        public WpfWebViewBridge(Microsoft.Web.WebView2.Wpf.WebView2 webView)
        {
            _webView = webView;
        }

        public async Task<string?> ExecuteJavaScriptAsync(string script)
        {
            string? result = null; // Changed to nullable string
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                result = await _webView.ExecuteScriptAsync(script);
            });
            return result;
        }

        public void RegisterCallback(Func<string, bool> callback)
        {
            _webView.CoreWebView2.WebMessageReceived += (sender, args) =>
            {
                callback?.Invoke(args.TryGetWebMessageAsString());
            };
        }

        public async Task LoadUrlAsync(Uri url)
        {
            _webView.Source = url;
            await Task.CompletedTask;
        }
    }

    public MainWindow()
    {
        InitializeComponent();
    }

    private Dynamsoft.DocumentViewer.JSInterop _jsInterop;
    private ServiceManager _serviceManager;
    private IReadOnlyList<Scanner> _scanners;
    private string productKey = "DLS2eyJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSJ9";

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        try
        { 
            await webView.EnsureCoreWebView2Async();

            JSInteropOptions options = new JSInteropOptions();
            // @"C:\code\dev\DWT\REST\.NET\DynamicWebTWAIN.RestClient\RestService\DynamicWebTWAINService\contentFiles\any\any\dynamsoft.dwt.service.windows"
            _serviceManager = new ServiceManager(); // "dynamsoft.dwt.service"
            _serviceManager.CreateService();
            // because we load ddv page in the service, so we should make sure the service is running, so we need to set a long timeout
            // or manual create a websocket connection in js, recommend this way.
            options.ProductKey = productKey;
            _jsInterop = new Dynamsoft.DocumentViewer.JSInterop(options,
                new WpfWebViewBridge(webView),
                _serviceManager.Service.BaseAddress);
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
            Application.Current.Dispatcher.Invoke(() => {
                MessageBox.Show(ex.Message);
            });
        }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        _jsInterop.DWTClient?.Dispose();
        _serviceManager?.Dispose();
    }

    private async void btnSaveAsPdf_Click(object sender, EventArgs e)
    {
        try
        {
            var count = await _jsInterop.GetPageCount();
            if (count == 0)
            {
                MessageBox.Show("There is no images in the buffer.");
                return;
            }

            var pdf = await _jsInterop.SaveAsPdf(PageOption.All, PdfPageType.PageDefault, SaveAnnotationMode.Annotation, "");
            string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WpfWebViewAppOutput.pdf");
            File.WriteAllBytes(filePath, pdf);
            MessageBox.Show(filePath);
        }
        catch (Exception ex)
        {
            Application.Current.Dispatcher.Invoke(() => {
                MessageBox.Show(ex.Message);
            });
        }
    }

    private async void btnScanToView_Click(object sender, EventArgs e)
    {
        try
        {
            CreateScanJobOptions options = new CreateScanJobOptions();
            options.Device = _scanners[cbxSources.SelectedIndex].Device;
            options.Config = new ScannerConfiguration();
            //options.Config.XferCount = 7;
            options.Config.IfFeederEnabled = true;
            options.Config.IfDuplexEnabled = false;
            await _jsInterop.ScanImageToView(options);
        }
        catch (Exception ex)
        {
            Application.Current.Dispatcher.Invoke(() => {
                MessageBox.Show(ex.Message);
            });
        }

    }
}