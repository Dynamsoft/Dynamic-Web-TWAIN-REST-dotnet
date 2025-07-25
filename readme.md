# Dynamic Web TWAIN REST .NET

This repo contains the packages for scanning documents using [Dynamic Web TWAIN](https://www.dynamsoft.com/web-twain/overview/?utm_source=github)'s [REST API](https://www.dynamsoft.com/web-twain/docs/extended-usage/restful-api.html?utm_source=github).

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

## Requirements

* .NET Standard 2.0 or greater
* .NET 4.6.1 (Desktop / Server) or greater

To open the samples, you need Visual Studio 2022 and .NET 9+.

## Installation

You need to reference the projects and add them as dependencies.

If you need to copy the service files and web resources (for MAUI) in the service package to your project, you need to add the following to your project.

```xml
<!--update the path based on your setup-->
<Import Project="..\..\DynamicWebTWAIN.Service\runtimes.targets" />
```

## Usage

### Scan Documents

1. Create a REST client instance. You need to specify the service's IP and the license key of Dynamic Web TWAIN ([apply for a 30-day trial](https://www.dynamsoft.com/customer/license/trialLicense/?product=dwt&utm_source=github)).

   ```csharp
   var address = "https://127.0.0.1:18623";
   var license = "LICENSE-KEY";
   var client = new DWTClient(new Uri(address), license);
   ```
   
   This requires you to install Dynamic Web TWAIN service beforehand ([download](#web-twain-service-installers)).
   
   You can also embed the service in your app by using the service package (no installation of service is required using this way). Currently, only Windows is supported.
   
   ```csharp
   var serviceManager = new ServiceManager();
   serviceManager.CreateService(); //create a service instance
   var address = serviceManager.Service.BaseAddress; //get the address for the REST client to use
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
   var job = await client.ScannerControlClient.ScannerJobs.CreateJob(options);
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
           break;
       }
       else
       {
           //process the image bytes
       }
   } while (true);
   ```


### Use the Document Viewer

We can embed [Dynamsoft Document Viewer](https://www.dynamsoft.com/document-viewer/docs/introduction/index.html?utm_source=github) in a WebView to view and edit the scanned document images and save the images as PDF.


1. Add a WebView in your app, like WebView2 for WinForm/WPF and HybridWebView for MAUI.

2. Implement the `IWebViewBridge` interface for different WebViews. The following are some examples.

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
   
   For HybridWebView, you need to copy the web page to the `Resources/raw/ddv` folder. If you've added the following to your project, it will copy the web page packed in the service package to your app's root.

   ```xml
   <!--update the path based on your setup-->
   <Import Project="..\..\DynamicWebTWAIN.Service\runtimes.targets" />
   ```
   
4. Create an instance of JSInterop. You need to use a license which is for both Dynamic Web TWAIN and Dynamsoft Document Viewer ([apply for a 30-day trial](https://www.dynamsoft.com/customer/license/trialLicense?product=dwtddv&utm_source=github)).

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
   
6. Save the documents as a PDF.

   ```csharp
   PageOption pageOption = PageOption.All;
   PdfPageType pdfPageType = PdfPageType.PageDefault;
   SaveAnnotationMode annotationMode = SaveAnnotationMode.None;
   byte[] pdfContent = await _jsInterop.SaveAsPdf(pageOption,pdfPageType,annotationMode,"");
   ```

## Samples

* [WPFDemo](./Samples/WpfDemo/): a full-featured demo in WPF with various image editing, PDF annotation and saving options.
* [MAUIDemo](./Samples/MAUIDemo/): a full-featured demo in MAUI that is mainly designed for the mobile platform. 
* [MAUIHybridApp](./Samples/MauiHybridApp/): a basic MAUI sample that scans document images into a viewer.
* [WPFWebViewApp](./Samples/WpfWebviewApp/): a basic WPF sample that scans document images into a viewer.
* [WinFormsApp](./Samples/WinFormsApp/): a basic WinForms sample that scans document images into a viewer.

The samples all use the Document Viewer in a WebView.

## Links

* [Dynamsoft Document Viewer Documentation](https://www.dynamsoft.com/document-viewer/docs/introduction/index.htm?utm_source=github)
* [RESTful API Documentation](https://www.dynamsoft.com/web-twain/docs/extended-usage/restful-api.html?utm_source=github)

## Web TWAIN Service Installers

Dynamic Web TWAIN service 19.1 is the minimum required version.


| Platform      | Download Link |
| ------------- | --------------- |
| Windows       | [Dynamic-Web-TWAIN-Service-Setup.msi](https://demo.dynamsoft.com/DWT/DWTResources/dist/DynamicWebTWAINServiceSetup.msi)       |
| macOS         | [Dynamic-Web-TWAIN-Service-Setup.pkg](https://demo.dynamsoft.com/DWT/DWTResources/dist/DynamicWebTWAINServiceSetup.pkg)        |
| Linux         | [Dynamic-Web-TWAIN-Service-Setup.deb](https://demo.dynamsoft.com/DWT/DWTResources/dist/DynamicWebTWAINServiceSetup.deb) <br/> [Dynamic-Web-TWAIN-Service-Setup-arm64.deb](https://demo.dynamsoft.com/DWT/DWTResources/dist/DynamicWebTWAINServiceSetup-arm64.deb) <br/> [Dynamic-Web-TWAIN-Service-Setup-mips64el.deb](https://demo.dynamsoft.com/DWT/DWTResources/dist/DynamicWebTWAINServiceSetup-mips64el.deb) <br/> [Dynamic-Web-TWAIN-Service-Setup.rpm](https://demo.dynamsoft.com/DWT/DWTResources/dist/DynamicWebTWAINServiceSetup.rpm)|
