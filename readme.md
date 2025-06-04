# Dynamic Web TWAIN REST .NET

This repo contains the packages for scanning documents using [Dynamic Web TWAIN](https://www.dynamsoft.com/web-twain/overview/)'s [REST API](https://www.dynamsoft.com/web-twain/docs/extended-usage/restful-api.html).

There are four packages:

* **DynamicWebTWAIN.RESTClient**: the .NET wrapper of the REST API.
* **DynamicWebTWAIN.Service**: a package to help you embed Dynamic Web TWAIN service in your app without the need to use its installer. The service is used to access document scanners and provide the REST API for document scanning.
* **DynamicWebTWAIN.ServiceFinder**: a package to help you find running Dynamic Web TWAIN services in a local network.
* **DocumentViewer.JSInterop**: a package to help you integrate document capturing with a dedicated viewer in your .NET app using WebView with extra features like PDF loading and saving.

The whole solution enables developers to create desktop or cross-platform applications to scan and digitize documents using the following scanning protocols:

* TWAIN (32-bit / 64-bit)
* WIA (Windows Image Acquisition)
* SANE (Linux)
* ICA (macOS)
* eSCL (AirScan / Mopria)

## Installation

You need to reference the projects and add them as dependencies.

If you need to copy the web resources in the service package to your .NET MAUI project as well. You need to add the following to your project.

```xml
<!--update the path based on your setup-->
<Import Project="..\..\DynamicWebTWAIN.Service\runtimes.targets" />
```

## Usage

### Scan Documents

1. Create a REST client instance. You need to specify the service's IP and the license key of Dynamic Web TWAIN ([apply for a 30-day trial](https://www.dynamsoft.com/customer/license/trialLicense/?product=dwt)).

   ```csharp
   var address = "https://127.0.0.1:18623";
   var license = "LICENSE-KEY";
   var client = new DWTClient(new Uri(address), license);
   ```
   
   This requires you to install Dynamic Web TWAIN service beforehand (install the service by visiting [the online demo](https://demo.dynamsoft.com/web-twain/)).
   
   You can also embed the service in your app by using the service package. Currently, only Windows is supported.
   
   ```csharp
   var serviceManager = new ServiceManager();
   serviceManager.CreateService();
   serviceManager.Service.BaseAddress; #get the address for the REST client to use
   ```

2. List connected scanners.

   ```csharp
   var scanners = await client.ScannerControlClient.ScannerManager.GetScanners(EnumDeviceTypeMask.DT_WIATWAINSCANNER | EnumDeviceTypeMask.DT_TWAINSCANNER);
   ```

3. Start a job to scan documents using a scanner.

   ```csharp
   CreateScanJobOptions options = new CreateScanJobOptions();
   options.AutoRun = false;
   options.Device = scanners[0].Device;
   options.Config = new ScannerConfiguration();
   options.Config.IfFeederEnabled = true;
   options.Config.IfDuplexEnabled = true;
   var job = await DWTClient.ScannerControlClient.ScannerJobs.CreateJob(options);
   await job.StartJob();
   ```
   
4. Retrieve the scanned document images one by one.


   ```csharp
   do
   {
       byte[] result = await job.GetNextImage();
       if (result == null)
       {
           await job.DeleteJob();
       }
       else
       {
           //process the image bytes
       }
   } while (true);
   ```


### Use the Document Viewer

We can embed Dynamsoft Document Viewer in a WebView to view and edit the scanned document images and save the images as PDF.


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

   For WebView2, if you are using the service package, the web page already exists in the service's [folder](./DynamicWebTWAIN.Service/PackService/content/common/dynamsoft.dwt.service/app/site/default/) and we can load it via URL. If you installed the service via the installers, you need to copy the web page to the service's `app/site/default/` folder.
   
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

## Samples

* [WPF Demo](./Samples/WpfDemo/): a full-featured demo in WPF with various image editing, PDF annotation and saving options.
* [MAUI Demo](./Samples/MAUIDemo/): a full-featured demo in MAUI which is mainly designed for the mobile platform. 
* [MAUI Simple](./Samples/MauiHybridApp/): a basic MAUI sample which scans document images into a viewer
* [WPF Simple](./Samples/WpfWebviewApp/): a basic WPF sample which scans document images into a viewer
* [WinForms](./Samples/WinFormsApp/): a basic WinForms sample which scans document images into a viewer

The samples all use the Document Viewer in a WebView.

## Links

* [Dynamsoft Document Viewer Documentation](https://www.dynamsoft.com/document-viewer/docs/introduction/index.html)
* [RESTful API Documentation](https://www.dynamsoft.com/web-twain/docs/extended-usage/restful-api.html)

