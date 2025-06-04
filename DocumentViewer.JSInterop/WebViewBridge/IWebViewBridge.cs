using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dynamsoft.DocumentViewer
{
    public interface IWebViewBridge
    {
        Task<string> ExecuteJavaScriptAsync(string script);
        void RegisterCallback(Func<string, bool> callback);

        Task LoadUrlAsync(Uri baseAddress);
    }

    public interface IBlazorWebViewBridge : IWebViewBridge
    {
        string AppName { get; set; }

        string CallbackName { get; set; }
    }
}

//webview-bridge.js
//(function() {
//    // Unified function to send messages to .NET
//    window.sendMessageToDotNet = function(message) {
//        if (window.chrome && window.chrome.webview)
//        {
//            // For WinForms and WPF (WebView2)
//            window.chrome.webview.postMessage(message);
//        }
//        else if (window.DotNet)
//        {
//            // For Blazor
//            DotNet.invokeMethodAsync('YourAssemblyName', 'ReceiveMessageFromJs', message);
//        }
//        else if (window.external && typeof window.external.notify === 'function')
//        {
//            // For MAUI, Xamarin, Avalonia, and Uno
//            window.external.notify(message);
//        }
//        else
//        {
//            console.error("Unsupported platform or WebView environment.");
//        }
//    }
//    ;

//    // Example usage: Call this function to send a message to .NET
//    // window.sendMessageToDotNet("Hello from JavaScript!");
//})();

//class Program
//{
//    static async Task Main(string[] args)
//    {
//        // Example usage for each platform
//        Console.WriteLine("Choose a platform: WinForms, WPF, Blazor, MAUI, Xamarin, Avalonia, Uno");
//        string platform = Console.ReadLine();

//        IWebViewBridge webViewBridge = null;

//        switch (platform)
//        {
//            case "WinForms":
//                var winFormsWebView = new Microsoft.Web.WebView2.WinForms.WebView2();
//                webViewBridge = new WinFormsWebViewBridge(winFormsWebView);
//                break;

//            case "WPF":
//                var wpfWebView = new Microsoft.Web.WebView2.Wpf.WebView2();
//                webViewBridge = new WpfWebViewBridge(wpfWebView);
//                break;

//            case "Blazor":
//                dynamic blazorWebView = null; // Replace with actual Blazor WebView instance
//                webViewBridge = new BlazorWebViewBridge(blazorWebView);
//                break;

//            case "MAUI":
//                var mauiWebView = new Microsoft.Maui.Controls.WebView();
//                webViewBridge = new MauiWebViewBridge(mauiWebView);
//                break;

//            case "Xamarin":
//                var xamarinWebView = new Xamarin.Forms.WebView();
//                webViewBridge = new XamarinWebViewBridge(xamarinWebView);
//                break;

//            case "Avalonia":
//                var avaloniaWebView = new Avalonia.Controls.WebView();
//                webViewBridge = new AvaloniaWebViewBridge(avaloniaWebView);
//                break;

//            case "Uno":
//                var unoWebView = new Windows.UI.Xaml.Controls.WebView();
//                webViewBridge = new UnoWebViewBridge(unoWebView);
//                break;

//            default:
//                Console.WriteLine("Invalid platform.");
//                return;
//        }

//        // Register a callback
//        webViewBridge.RegisterCallback(message =>
//        {
//            Console.WriteLine($"Callback received: {message}");
//            return true;
//        });

//        // Execute JavaScript
//        string script = "document.title = 'Hello from .NET';";
//        string result = await webViewBridge.ExecuteJavaScriptAsync(script);
//        Console.WriteLine($"JavaScript execution result: {result}");
//    }
//}

//// WinForms implementation
//public class WinFormsWebViewBridge : IWebViewBridge
//{
//    private Microsoft.Web.WebView2.WinForms.WebView2 _webView;

//    public WinFormsWebViewBridge(Microsoft.Web.WebView2.WinForms.WebView2 webView)
//    {
//        _webView = webView;
//    }

//    public async Task<string> ExecuteJavaScriptAsync(string script)
//    {
//        return await _webView.ExecuteScriptAsync(script);
//    }

//    public void RegisterCallback(Func<object, string, bool> callback)
//    {
//        _webView.CoreWebView2.WebMessageReceived += (sender, args) =>
//        {
//            string message = args.TryGetWebMessageAsString();
//            callback?.Invoke(message);
//        };
//    }
//public async Task LoadUrlAsync(Uri url)
//{
//    _webView.Source = new Uri(url);
//    await Task.CompletedTask;
//}
//}

//// WPF implementation
//public class WpfWebViewBridge : IWebViewBridge
//{
//    private Microsoft.Web.WebView2.Wpf.WebView2 _webView;

//    public WpfWebViewBridge(Microsoft.Web.WebView2.Wpf.WebView2 webView)
//    {
//        _webView = webView;
//    }

//    public async Task<string> ExecuteJavaScriptAsync(string script)
//    {
//        return await _webView.ExecuteScriptAsync(script);
//    }

//    public void RegisterCallback(Func<object, string, bool> callback)
//    {
//        _webView.CoreWebView2.WebMessageReceived += (sender, args) =>
//        {
//            string message = args.TryGetWebMessageAsString();
//            callback?.Invoke(message);
//        };
//    }
//public async Task LoadUrlAsync(Uri url)
//{
//    _webView.Source = new Uri(url);
//    await Task.CompletedTask;
//}
//}

//// Blazor implementation
//public class BlazorWebViewBridge : IWebViewBridge
//{
//    private Microsoft.JSInterop.IJSRuntime _jsRuntime;

//    public BlazorWebViewBridge(Microsoft.JSInterop.IJSRuntime jsRuntime)
//    {
//        _jsRuntime = jsRuntime;
//    }

//    public async Task<string> ExecuteJavaScriptAsync(string script)
//    {
//        return await _jsRuntime.InvokeAsync<string>("eval", script);
//    }

//    public void RegisterCallback(Func<object, string, bool> callback)
//    {
//        // Use JSRuntime to invoke .NET methods from JavaScript
//        Microsoft.JSInterop.JSInvokableAttribute.RegisterCallback(callback);
//    }
//[JSInvokable]
//public static void ReceiveMessageFromJs(string message)
//{
//    Console.WriteLine($"Message from JavaScript: {message}");
//}

//public async Task LoadUrlAsync(Uri url)
//{
//    // Use JavaScript interop to navigate to the URL
//    await _jsRuntime.InvokeVoidAsync("window.location.replace", url);
//}
//}
// javascript code
//function sendMessageToDotNet(message)
//{
//    DotNet.invokeMethodAsync('YourAssemblyName', 'ReceiveMessageFromJs', message);
//}
//@inject IJSRuntime JSRuntime

//<button @onclick="SendMessageToJs">Send Message to JS</button>

//@code {
//    private BlazorWebViewBridge _webViewBridge;

//protected override void OnInitialized()
//{
//    _webViewBridge = new BlazorWebViewBridge(JSRuntime);
//    _webViewBridge.RegisterCallback(message =>
//    {
//        Console.WriteLine($"Message from JavaScript: {message}");
//        return true;
//    });

//    // Pass a reference of the bridge to JavaScript
//    JSRuntime.InvokeVoidAsync("setDotNetReference", DotNetObjectReference.Create(_webViewBridge));
//}

//private async Task SendMessageToJs()
//{
//    await _webViewBridge.ExecuteJavaScriptAsync("sendMessageToDotNet('Hello from JavaScript');");
//}
//}


//// MAUI implementation
//public class MauiWebViewBridge : IWebViewBridge
//{
//    private Microsoft.Maui.Controls.WebView _webView;

//    public MauiWebViewBridge(Microsoft.Maui.Controls.WebView webView)
//    {
//        _webView = webView;
//    }

//    public Task<string> ExecuteJavaScriptAsync(string script)
//    {
//        var tcs = new TaskCompletionSource<string>();
//        _webView.Eval(script);
//        tcs.SetResult("JavaScript executed");
//        return tcs.Task;
//    }

//    public void RegisterCallback(Func<object, string, bool> callback)
//    {
//        _webView.Navigating += (sender, args) =>
//        {
//            if (args.Url.StartsWith("callback:"))
//            {
//                string message = args.Url.Substring("callback:".Length);
//                callback?.Invoke(message);
//                args.Cancel = true;
//            }
//        };
//    }
//public async Task LoadUrlAsync(Uri url)
//{
//    _webView.Source = url;
//    await Task.CompletedTask;
//}
//}

//// Xamarin implementation
//public class XamarinWebViewBridge : IWebViewBridge
//{
//    private Xamarin.Forms.WebView _webView;

//    public XamarinWebViewBridge(Xamarin.Forms.WebView webView)
//    {
//        _webView = webView;
//    }

//    public Task<string> ExecuteJavaScriptAsync(string script)
//    {
//        var tcs = new TaskCompletionSource<string>();
//        _webView.Eval(script);
//        tcs.SetResult("JavaScript executed");
//        return tcs.Task;
//    }

//    public void RegisterCallback(Func<object, string, bool> callback)
//    {
//        _webView.Navigating += (sender, args) =>
//        {
//            if (args.Url.StartsWith("callback:"))
//            {
//                string message = args.Url.Substring("callback:".Length);
//                callback?.Invoke(message);
//                args.Cancel = true;
//            }
//        };
//    }
//public async Task LoadUrlAsync(Uri url)
//{
//    _webView.Source = url;
//    await Task.CompletedTask;
//}
//}

//// Avalonia implementation
//public class AvaloniaWebViewBridge : IWebViewBridge
//{
//    private Avalonia.Controls.WebView _webView;

//    public AvaloniaWebViewBridge(Avalonia.Controls.WebView webView)
//    {
//        _webView = webView;
//    }

//    public Task<string> ExecuteJavaScriptAsync(string script)
//    {
//        var tcs = new TaskCompletionSource<string>();
//        _webView.NavigateToString($"<script>{script}</script>");
//        tcs.SetResult("JavaScript executed");
//        return tcs.Task;
//    }

//    public void RegisterCallback(Func<object, string, bool> callback)
//    {
//        _webView.NavigationStarting += (sender, args) =>
//        {
//            if (args.Uri.ToString().StartsWith("callback:"))
//            {
//                string message = args.Uri.ToString().Substring("callback:".Length);
//                callback?.Invoke(message);
//                args.Cancel = true;
//            }
//        };
//    }
//public async Task LoadUrlAsync(Uri url)
//{
//    _webView.Source = url;
//    await Task.CompletedTask;
//}
//}

//// Uno implementation
//public class UnoWebViewBridge : IWebViewBridge
//{
//    private Windows.UI.Xaml.Controls.WebView _webView;

//    public UnoWebViewBridge(Windows.UI.Xaml.Controls.WebView webView)
//    {
//        _webView = webView;
//    }

//    public async Task<string> ExecuteJavaScriptAsync(string script)
//    {
//        return await _webView.InvokeScriptAsync("eval", new[] { script });
//    }

//    public void RegisterCallback(Func<object, string, bool> callback)
//    {
//        _webView.NavigationStarting += (sender, args) =>
//        {
//            if (args.Uri.ToString().StartsWith("callback:"))
//            {
//                string message = args.Uri.ToString().Substring("callback:".Length);
//                callback?.Invoke(message);
//                args.Cancel = true;
//            }
//        };
//    }
//public async Task LoadUrlAsync(Uri url)
//{
//    _webView.Navigate(new Uri(url));
//    await Task.CompletedTask;
//}
//}
//}
