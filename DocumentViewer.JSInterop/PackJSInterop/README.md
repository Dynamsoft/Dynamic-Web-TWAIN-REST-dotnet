## Dynamsoft DocumentViewer JavaScript Interop

The Dynamsoft DocumentViewer JavaScript Interop helps you integrate document capturing with a dedicated viewer in your .NET app.

It embeds [Dynamsoft Document Viewer](https://www.dynamsoft.com/document-viewer/docs/introduction/index.html) in a WebView and makes the communication between the JavaScript side and the .NET side easy with pre-wrapped functions. The document capture function is based on [Dynamic Web TWAIN](https://www.dynamsoft.com/web-twain/overview/)'s REST API.

It enables developers to create desktop or cross-platform applications to scan and digitize documents using:

* TWAIN (32-bit / 64-bit)
* WIA (Windows Image Acquisition)
* SANE (Linux)
* ICA (macOS)
* eSCL (AirScan / Mopria)

### Prerequisites

1. You need to install Dynamic Web TWAIN service or use the `Dynamsoft.DynamicWebTWAIN.Service` package to start the service in your app.
2. Get a license of Dynamic Web TWAIN: [30-day trial](https://www.dynamsoft.com/customer/license/trialLicense/?product=dwt)

### Usage

1. Add a WebView in your app, like WebView2 for WinForm/WPF and HybridWebView for MAUI.

2. Implement the `IWebViewBridge` interface for different WebViews. The following is some examples.

   MAUI HybridWebView:

   ```csharp
   public class HybridWebViewBridge : IWebViewBridge
   {
       private Microsoft.Maui.Controls.HybridWebView _webView;

       public HybridWebViewBridge(Microsoft.Maui.Controls.HybridWebView webView)
       {
           _webView = webView;

       }

       public async Task<string> ExecuteJavaScriptAsync(string script)
       {
           string result = null;
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

           _webView.DefaultFile = url.OriginalString;

           await Task.CompletedTask;
       }
   }
   ```
   
   WPF WebView2:
   
   ```csharp
   public class WpfWebViewBridge : IWebViewBridge
   {
       private Microsoft.Web.WebView2.Wpf.WebView2 _webView;

       public WpfWebViewBridge(Microsoft.Web.WebView2.Wpf.WebView2 webView)
       {
           _webView = webView;
       }

       public async Task<string> ExecuteJavaScriptAsync(string script)
       {
           string result = null;
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
   ```
   
3. Copy the web page.

   For WebView2, the web page already exists in the service's folder and we can load it via URL. 
   
   For HybridWebView, you need to copy the web page to the `Resources/raw/ddv` folder. You can also add the following to your project to use the files packed in the service package.

   ```xml
   <!--update the path based on your setup-->
   <Import Project="..\..\DynamicWebTWAIN.Service\runtimes.targets" />
   ```
   
4. Create an instance of JSInterop.

   ```csharp
   private JSInterop _jsInterop = null;
   JSInteropOptions options = new JSInteropOptions();
   options.ProductKey = "ProductKey";
   _jsInterop = new JSInterop(options,
       new WpfWebViewBridge(webView)
       new Uri("https://127.0.0.1:18623")); //URL of the Dynamic Web TWAIN service
   await _jsInterop.EnsureInitializedAsync();
   ```
   
5. Scan documents into the viewer.

   ```csharp
   CreateScanJobOptions options = new CreateScanJobOptions();
   options.AutoRun = false;
   var scannerJob = await _jsInterop.CreateScanToViewJob(options);
   await _jsInterop.StartJob(scannerJob);
   ```
   
6. Save the documents as PDF.

   ```csharp
   PageOption pageOption = PageOption.All;
   PdfPageType pdfPageType = PdfPageType.PageDefault;
   SaveAnnotationMode annotationMode = SaveAnnotationMode.None;
   byte[] pdfContent = await _jsInterop.SaveAsPdf(pageOption,pdfPageType,annotationMode,"");
   ```





