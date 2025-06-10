using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using DynamicWebTWAIN.Service;
using Dynamsoft.DocumentViewer;
namespace WpfDemo
{
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
                _webView.CoreWebView2.ServerCertificateErrorDetected += (sender, args) =>
                {
                    // WARNING: This will ignore all SSL errors. Use only for development/testing.
                    args.Action = Microsoft.Web.WebView2.Core.CoreWebView2ServerCertificateErrorAction.AlwaysAllow;
                };
            }

            public async Task<string?> ExecuteJavaScriptAsync(string script)
            {
                string? result = null;
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

        static MainWindow()
        {
            int index = System.Reflection.Assembly.GetExecutingAssembly().Location.IndexOf("WpfDemo");
            if (index != -1)
            {
                strCurrentDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, index);
                imageDirectory = strCurrentDirectory + @"WpfDemo\Images\";
                strTessdataDirectory = strCurrentDirectory + @"WpfDemo\Tessdata\"; // Initialize strTessdataDirectory  
                mSettingsPath = strCurrentDirectory + @"WpfDemo\Settings\settings.json"; // Initialize mSettingsPath  
            }
            else
            {
                index = System.Reflection.Assembly.GetExecutingAssembly().Location.LastIndexOf("\\");
                if (index != -1)
                {
                    strCurrentDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, index + 1);
                }
                else
                {
                    strCurrentDirectory = Environment.CurrentDirectory + "\\";
                }
                imageDirectory = strCurrentDirectory + @"\Images\";
                strTessdataDirectory = strCurrentDirectory + @"\Tessdata\"; // Initialize strTessdataDirectory  
                mSettingsPath = strCurrentDirectory + @"\Settings\settings.json"; // Initialize mSettingsPath  
            }
        }

        public MainWindow()
        {
            InitializeComponent();
                        
            try
            {
                dpTitle.Background = new ImageBrush(new BitmapImage(new Uri(imageDirectory + "title.png", UriKind.RelativeOrAbsolute)));
                IconBitmapDecoder ibd = new IconBitmapDecoder(
                   new Uri(imageDirectory + "dnt_demo_icon.ico", UriKind.RelativeOrAbsolute),
                   BitmapCreateOptions.None,
                   BitmapCacheOption.Default);
                this.Icon = ibd.Frames[0];
            }
            catch { }
            dpTitle.MouseLeftButtonDown += new MouseButtonEventHandler(MoveWindow);

            string dynamicDotNetTwainDirectory = strCurrentDirectory;
            int index = System.Reflection.Assembly.GetExecutingAssembly().Location.IndexOf("Demos");
        }

        private Dynamsoft.DocumentViewer.JSInterop _jsInterop = null!;
        private ServiceManager _serviceManager = null!;

        public Dynamsoft.DocumentViewer.JSInterop JSInterop
        {
            get { return _jsInterop; }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await webView.EnsureCoreWebView2Async();

                JSInteropOptions options = new JSInteropOptions();
                _serviceManager = new ServiceManager();
                _serviceManager.CreateService();
                // because we load ddv page in the service, so we should make sure the service is running, so we need to set a long timeout
                // or manual create a websocket connection in js, recommend this way.
                options.ProductKey = "t0131DQEAAJ/lU28fZecBIvVDoVs4/k5Ks8uXHXt20fnA2utzW/9gEiH37ujt2ws6Fe8k2rQE845RQ+mf2YkuC/A9hMIQng8ppTmpxUpW0cZAt+oACSMQYQYijEGEKYgwARIGZjqGprG1CGcYGCRCmEDXpKUZIqyC2GFibmhoAgwGAPLTOE4=";
                _jsInterop = new Dynamsoft.DocumentViewer.JSInterop(options,
                    new WpfWebViewBridge(webView),
                    _serviceManager.Service.BaseAddress);
                await _jsInterop.EnsureInitializedAsync();
            }
            catch(Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => {
                    MessageBox.Show(ex.Message);
                });
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _jsInterop?.DWTClient?.Dispose();
            _serviceManager?.Dispose();
        }


        public static Dictionary<String, ImageBrush> icons = new Dictionary<string, ImageBrush>();
        public static readonly string imageDirectory;
        public static readonly string strCurrentDirectory;
        public static readonly string strTessdataDirectory;
        public static readonly string mSettingsPath;
        private string _mouseShape = "hand";
        private string _annotationType = "";

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Name == "btnHand" && _mouseShape == "hand")
            {
                return;
            }
            else if (btn.Name == "btnArrow" && _mouseShape == "crop")
            {
                return;
            }
            else if (btn.Name == "btnLine" && _annotationType == "line")
            {
                return;
            }
            else if (btn.Name == "btnPolyline" && _annotationType == "polyline")
            {
                return;
            }
            else if (btn.Name == "btnRectangle" && _annotationType == "rectangle")
            {
                return;
            }
            else if (btn.Name == "btnEclipse" && _annotationType == "ellipse")
            {
                return;
            }
            else if (btn.Name == "btnText" && _annotationType == "text")
            {
                return;
            }
            else if (btn.Name == "btnTextBox" && _annotationType == "textbox")
            {
                return;
            }
            else if (btn.Name == "btnInk" && _annotationType == "ink")
            {
                return;
            }
            else if (btn.Name == "btnStamp" && _annotationType == "stamp")
            {
                return;
            }

            string key = "hover/" + btn.Tag;
            if (!icons.ContainsKey(key))
            {
                try
                {
                    icons.Add(key, new ImageBrush(new BitmapImage(new Uri(imageDirectory + key + ".png", UriKind.RelativeOrAbsolute))));
                }
                catch { }
            }
            try
            {
                btn.Background = icons[key];
            }
            catch { }
        }

        private void Button_MouseLeave(object sender, MouseEventArgs? e)
        {
            Button btn = (Button)sender;

            if (btn.Name == "btnHand" && _mouseShape == "hand")
            {
                return;
            }
            else if (btn.Name == "btnArrow" && _mouseShape == "crop")
            {
                return;
            }
            else if (btn.Name == "btnLine" && _annotationType == "line")
            {
                return;
            }
            else if (btn.Name == "btnPolyline" && _annotationType == "polyline")
            {
                return;
            }
            else if (btn.Name == "btnRectangle" && _annotationType == "rectangle")
            {
                return;
            }
            else if (btn.Name == "btnEclipse" && _annotationType == "ellipse")
            {
                return;
            }
            else if (btn.Name == "btnText" && _annotationType == "text")
            {
                return;
            }
            else if (btn.Name == "btnTextBox" && _annotationType == "textbox")
            {
                return;
            }
            else if (btn.Name == "btnInk" && _annotationType == "ink")
            {
                return;
            }
            else if (btn.Name == "btnStamp" && _annotationType == "stamp")
            {
                return;
            }

            string key = "normal/" + btn.Tag;
            if (!icons.ContainsKey(key))
            {
                try
                {
                    icons.Add(key, new ImageBrush(new BitmapImage(new Uri(imageDirectory + key + ".png", UriKind.RelativeOrAbsolute))));
                }
                catch { }
            }
            try
            {
                btn.Background = icons[key];
            }
            catch { }
        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                Button_MouseLeave(button, new MouseEventArgs(Mouse.PrimaryDevice, 0));
            }

            Button btn = (Button)sender;
            if (btn.Name == "btnHand" && _mouseShape == "hand")
            {
                MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
                args.RoutedEvent = Button.PreviewMouseDownEvent;
                Button_PreviewMouseDown(sender, args);

                MouseEventArgs argsA = new MouseEventArgs(Mouse.PrimaryDevice, 0);
                argsA.RoutedEvent = Button.MouseLeaveEvent;
                Button_MouseLeave(btnArrow, argsA);
            }
            else if (btn.Name == "btnArrow" && _mouseShape == "crop")
            {
                MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
                args.RoutedEvent = Button.PreviewMouseDownEvent;
                Button_PreviewMouseDown(sender, args);

                MouseEventArgs argsA = new MouseEventArgs(Mouse.PrimaryDevice, 0);
                argsA.RoutedEvent = Button.MouseLeaveEvent;
                Button_MouseLeave(btnHand, argsA);
            }
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Button btn = (Button)sender;
                string key = "active/" + btn.Tag;
                if (!icons.ContainsKey(key))
                {
                    try
                    {
                        icons.Add(key, new ImageBrush(new BitmapImage(new Uri(imageDirectory + key + ".png", UriKind.RelativeOrAbsolute))));
                    }
                    catch { }
                }
                try
                {
                    btn.Background = icons[key];
                }
                catch { }
            }
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Button_MouseLeave(sender, null);
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MaxWindow(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
        }

        private void MinWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void Scan_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                try
                {
                    button.IsEnabled = false;

                    var scanners = await _jsInterop.DWTClient.ScannerControlClient.ScannerManager.GetScanners(DynamicWebTWAIN.RestClient.EnumDeviceTypeMask.DT_TWAINSCANNER);

                    if (scanners.Count > 0)
                    {
                        ScanWindow scanWnd = new ScanWindow(this, scanners);
                        scanWnd.Owner = this;
                        scanWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        scanWnd.ShowDialog();

                        // Ensure the main window regains focus
                        this.Activate();
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("There is no scanner!", "Scan Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(ex.Message);
                    });
                }
                finally
                {
                    button.IsEnabled = true;
                }
            }
        }

        private async void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Supported Files(*.jpg;*.jpe;*.jpeg;*.jfif;*.bmp;*.png;*.tif;*.tiff;*.pdf;*.gif)|*.jpg;*.jpe;*.jpeg;*.jfif;*.bmp;*.png;*.tif;*.tiff;*.pdf;*.gif|JPEG(*.jpg;*.jpeg;*.jpe;*.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|BMP(*.bmp)|*.bmp|PNG(*.png)|*.png|TIFF(*.tif;*.tiff)|*.tif;*.tiff|PDF(*.pdf)|*.pdf|GIF(*.gif)|*.gif";
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    foreach (String strFileName in dlg.FileNames)
                    {
                        await _jsInterop.LoadFile(File.ReadAllBytes(strFileName));
                    }
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() => {
                        MessageBox.Show(ex.Message);
                    });
                }
            }
        }
       
        private void Hand_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.SetCursorToPan();
            _mouseShape = "hand";
            _annotationType = "";
            MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
            args.RoutedEvent = Button.PreviewMouseDownEvent;
            Button_PreviewMouseDown(sender, args);

            MouseEventArgs argsA = new MouseEventArgs(Mouse.PrimaryDevice, 0);
            argsA.RoutedEvent = Button.MouseLeaveEvent;
            Button_MouseLeave(btnArrow, argsA);
            Button_MouseLeave(btnLine, argsA);
            Button_MouseLeave(btnPolyline, argsA);
            Button_MouseLeave(btnRectangle, argsA);
            Button_MouseLeave(btnEclipse, argsA);
            Button_MouseLeave(btnText, argsA);
            Button_MouseLeave(btnTextBox, argsA);
            Button_MouseLeave(btnInk, argsA);
            Button_MouseLeave(btnStamp, argsA);
        }

        private void Arrow_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.SetCursorToCrop();
            _mouseShape = "crop";
            _annotationType = "";
            MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
            args.RoutedEvent = Button.PreviewMouseDownEvent;
            Button_PreviewMouseDown(sender, args);

            MouseEventArgs argsA = new MouseEventArgs(Mouse.PrimaryDevice, 0);
            argsA.RoutedEvent = Button.MouseLeaveEvent;
            Button_MouseLeave(btnHand, argsA);
            Button_MouseLeave(btnLine, argsA);
            Button_MouseLeave(btnPolyline, argsA);
            Button_MouseLeave(btnRectangle, argsA);
            Button_MouseLeave(btnEclipse, argsA);
            Button_MouseLeave(btnText, argsA);
            Button_MouseLeave(btnTextBox, argsA);
            Button_MouseLeave(btnInk, argsA);
            Button_MouseLeave(btnStamp, argsA);
        }

        private void RotateRight_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.RotateRight(false);
        }

        private void RotateLeft_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.RotateLeft(false);
        }

        private void FitWindow_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.ZoomToFitWindow();
        }

        private void OriginalSize_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.ZoomToActualSize();
        }

        private void Crop_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.Crop(false);
        }

        private void Line_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.SetAnnotationMode(AnnotationMode.Line);
            _annotationType = "line";
            MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
            args.RoutedEvent = Button.PreviewMouseDownEvent;
            Button_PreviewMouseDown(sender, args);

            MouseEventArgs argsA = new MouseEventArgs(Mouse.PrimaryDevice, 0);
            argsA.RoutedEvent = Button.MouseLeaveEvent;
            Button_MouseLeave(btnPolyline, argsA);
            Button_MouseLeave(btnRectangle, argsA);
            Button_MouseLeave(btnEclipse, argsA);
            Button_MouseLeave(btnText, argsA);
            Button_MouseLeave(btnTextBox, argsA);
            Button_MouseLeave(btnInk, argsA);
            Button_MouseLeave(btnStamp, argsA);
        }
        private void Polyline_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.SetAnnotationMode(AnnotationMode.Polyline);
            _annotationType = "polyline";
            MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
            args.RoutedEvent = Button.PreviewMouseDownEvent;
            Button_PreviewMouseDown(sender, args);

            MouseEventArgs argsA = new MouseEventArgs(Mouse.PrimaryDevice, 0);
            argsA.RoutedEvent = Button.MouseLeaveEvent;
            Button_MouseLeave(btnLine, argsA);
            Button_MouseLeave(btnRectangle, argsA);
            Button_MouseLeave(btnEclipse, argsA);
            Button_MouseLeave(btnText, argsA);
            Button_MouseLeave(btnTextBox, argsA);
            Button_MouseLeave(btnInk, argsA);
            Button_MouseLeave(btnStamp, argsA);
        }

        private void Rectangle_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.SetAnnotationMode(AnnotationMode.Rectangle);
            _annotationType = "rectangle";
            MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
            args.RoutedEvent = Button.PreviewMouseDownEvent;
            Button_PreviewMouseDown(sender, args);

            MouseEventArgs argsA = new MouseEventArgs(Mouse.PrimaryDevice, 0);
            argsA.RoutedEvent = Button.MouseLeaveEvent;
            Button_MouseLeave(btnLine, argsA);
            Button_MouseLeave(btnPolyline, argsA);
            Button_MouseLeave(btnEclipse, argsA);
            Button_MouseLeave(btnText, argsA);
            Button_MouseLeave(btnTextBox, argsA);
            Button_MouseLeave(btnInk, argsA);
            Button_MouseLeave(btnStamp, argsA);
        }

        private void Eclipse_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.SetAnnotationMode(AnnotationMode.Ellipse);
            _annotationType = "ellipse";
            MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
            args.RoutedEvent = Button.PreviewMouseDownEvent;
            Button_PreviewMouseDown(sender, args);

            MouseEventArgs argsA = new MouseEventArgs(Mouse.PrimaryDevice, 0);
            argsA.RoutedEvent = Button.MouseLeaveEvent;
            Button_MouseLeave(btnArrow, argsA);
            Button_MouseLeave(btnLine, argsA);
            Button_MouseLeave(btnPolyline, argsA);
            Button_MouseLeave(btnRectangle, argsA);
            Button_MouseLeave(btnText, argsA);
            Button_MouseLeave(btnTextBox, argsA);
            Button_MouseLeave(btnInk, argsA);
            Button_MouseLeave(btnStamp, argsA);
        }

        private void Text_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.SetAnnotationMode(AnnotationMode.TextTypewriter);
            _annotationType = "text";
            MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
            args.RoutedEvent = Button.PreviewMouseDownEvent;
            Button_PreviewMouseDown(sender, args);

            MouseEventArgs argsA = new MouseEventArgs(Mouse.PrimaryDevice, 0);
            argsA.RoutedEvent = Button.MouseLeaveEvent;
            Button_MouseLeave(btnArrow, argsA);
            Button_MouseLeave(btnLine, argsA);
            Button_MouseLeave(btnPolyline, argsA);
            Button_MouseLeave(btnRectangle, argsA);
            Button_MouseLeave(btnEclipse, argsA);
            Button_MouseLeave(btnTextBox, argsA);
            Button_MouseLeave(btnInk, argsA);
            Button_MouseLeave(btnStamp, argsA);
        }

        private void TextBox_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.SetAnnotationMode(AnnotationMode.TextBox);
            _annotationType = "textbox";
            MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
            args.RoutedEvent = Button.PreviewMouseDownEvent;
            Button_PreviewMouseDown(sender, args);

            MouseEventArgs argsA = new MouseEventArgs(Mouse.PrimaryDevice, 0);
            argsA.RoutedEvent = Button.MouseLeaveEvent;
            Button_MouseLeave(btnArrow, argsA);
            Button_MouseLeave(btnLine, argsA);
            Button_MouseLeave(btnPolyline, argsA);
            Button_MouseLeave(btnRectangle, argsA);
            Button_MouseLeave(btnEclipse, argsA);
            Button_MouseLeave(btnText, argsA);
            Button_MouseLeave(btnInk, argsA);
            Button_MouseLeave(btnStamp, argsA);
        }

        private void Ink_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.SetAnnotationMode(AnnotationMode.Ink);
            _annotationType = "ink";
            MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
            args.RoutedEvent = Button.PreviewMouseDownEvent;
            Button_PreviewMouseDown(sender, args);

            MouseEventArgs argsA = new MouseEventArgs(Mouse.PrimaryDevice, 0);
            argsA.RoutedEvent = Button.MouseLeaveEvent;
            Button_MouseLeave(btnArrow, argsA);
            Button_MouseLeave(btnLine, argsA);
            Button_MouseLeave(btnPolyline, argsA);
            Button_MouseLeave(btnRectangle, argsA);
            Button_MouseLeave(btnEclipse, argsA);
            Button_MouseLeave(btnText, argsA);
            Button_MouseLeave(btnTextBox, argsA);
            Button_MouseLeave(btnStamp, argsA);
        }

        private void Stamp_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.SetAnnotationMode(AnnotationMode.Stamp);
            _annotationType = "stamp";
            MouseButtonEventArgs args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
            args.RoutedEvent = Button.PreviewMouseDownEvent;
            Button_PreviewMouseDown(sender, args);

            MouseEventArgs argsA = new MouseEventArgs(Mouse.PrimaryDevice, 0);
            argsA.RoutedEvent = Button.MouseLeaveEvent;
            Button_MouseLeave(btnArrow, argsA);
            Button_MouseLeave(btnLine, argsA);
            Button_MouseLeave(btnPolyline, argsA);
            Button_MouseLeave(btnRectangle, argsA);
            Button_MouseLeave(btnEclipse, argsA);
            Button_MouseLeave(btnText, argsA);
            Button_MouseLeave(btnTextBox, argsA);
            Button_MouseLeave(btnInk, argsA);
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.Undo();
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.Redo();
        }


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.DeleteCurrent();
        }

        private void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.DeleteSelected();
        }

        private void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            _jsInterop.DeleteAll();
        }      

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveWindow saveWnd = new SaveWindow(this);
                saveWnd.Owner = this;
                saveWnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                saveWnd.ShowDialog();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => {
                    MessageBox.Show(ex.Message);
                });
            }

        }
    }
}
