using DynamicWebTWAIN.RestClient;
using DynamicWebTWAIN.Service;
using Dynamsoft.DocumentViewer;
using Microsoft.Web.WebView2.Core;

namespace WinFormsApp
{
    public class WinFormsWebViewBridge : IWebViewBridge
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 _webView;

        public WinFormsWebViewBridge(Microsoft.Web.WebView2.WinForms.WebView2 webView)
        {
            _webView = webView;
        }

        public async Task<string> ExecuteJavaScriptAsync(string script)
        {
            if (_webView is Control control && control.InvokeRequired)
            {
                string result = null;
                control.Invoke(new Action(async () =>
                {
                    result = await _webView.ExecuteScriptAsync(script);
                }));
                return result; // Ensure a value is returned after invocation
            }
            else
            {
                return await _webView.ExecuteScriptAsync(script);
            }
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

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private JSInterop _jsInterop;
        private ServiceManager _serviceManager;
        private async void Window_Loaded(object sender, EventArgs e)
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
                options.ProductKey = "t0131DQEAAJ/lU28fZecBIvVDoVs4/k5Ks8uXHXt20fnA2utzW/9gEiH37ujt2ws6Fe8k2rQE845RQ+mf2YkuC/A9hMIQng8ppTmpxUpW0cZAt+oACSMQYQYijEGEKYgwARIGZjqGprG1CGcYGCRCmEDXpKUZIqyC2GFibmhoAgwGAPLTOE4=";
                _jsInterop = new JSInterop(options,
                    new WinFormsWebViewBridge(webView),
                    _serviceManager.Service.BaseAddress);
                await _jsInterop.EnsureInitializedAsync();
            }
            catch (Exception ex)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => MessageBox.Show(ex.Message)));
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Window_Closing(object sender, FormClosingEventArgs e)
        {
            _jsInterop.DWTClient?.Dispose();
            _serviceManager?.Dispose();
        }


        private async void btnSaveAsPdf_Click(object sender, EventArgs e)
        {
            try
            {
                var pdf = await _jsInterop.SaveAsPdf(PageOption.All, PdfPageType.PageDefault, SaveAnnotationMode.Annotation, "");
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WinFormsWebViewAppOutput.pdf");
                File.WriteAllBytes(filePath, pdf);
                MessageBox.Show(filePath);
            }
            catch (Exception ex)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => MessageBox.Show(ex.Message)));
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private async void btnScanToView_Click(object sender, EventArgs e)
        {
            try
            {
                CreateScanJobOptions options = new CreateScanJobOptions();
                options.Config = new ScannerConfiguration();
                options.Config.XferCount = 7;
                options.Config.IfFeederEnabled = true;
                options.Config.IfDuplexEnabled = true;
                await _jsInterop.ScanImageToView(options);
            }
            catch (Exception ex)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => MessageBox.Show(ex.Message)));
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }
            
        }

        
    }
}
