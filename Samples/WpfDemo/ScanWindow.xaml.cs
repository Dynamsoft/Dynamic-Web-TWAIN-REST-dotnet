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
using System.Windows.Shapes;
using DynamicWebTWAIN.RestClient;
using Microsoft.Win32;

namespace WpfDemo
{
    /// <summary>
    /// Interaction logic for ScanWindow.xaml
    /// </summary>
    public partial class ScanWindow : Window
    {
        private TextBox? m_TotalImageTextBox = null;

        public void SetTotalImageTextBox(TextBox tbx)
        {
            m_TotalImageTextBox = tbx;
        }

        private TextBox? m_CurrentImageTextBox = null;

        public void SetCurrentImageTextBox(TextBox tbx)
        {
            m_CurrentImageTextBox = tbx;
        }

        private readonly IReadOnlyList<Scanner> _scanners;
        private readonly MainWindow _mainWindow;

        public ScanWindow(MainWindow mainWindow, IReadOnlyList<Scanner> scanners)
        {
            if (mainWindow == null) throw new ArgumentNullException(nameof(mainWindow));
            if (scanners == null) throw new ArgumentNullException(nameof(scanners));

            _mainWindow = mainWindow;
            _scanners = scanners;

            InitializeComponent();
            try
            {
                lbScan.Background = new ImageBrush(new BitmapImage(new Uri(MainWindow.imageDirectory + @"normal\scan_now.png", UriKind.RelativeOrAbsolute)));

                foreach (var scanner in scanners)
                {
                    cbxSources.Items.Add(scanner.Name);
                }

                cbxSources.SelectedIndex = 0;
            }
            catch { }
            this.Closing += new System.ComponentModel.CancelEventHandler(ScanWindow_Closing);
        }

        void ScanWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private async void lbScan_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lbScan.IsEnabled == false)
            {
                Console.WriteLine("Scan button is disabled");
                return;
            }
            string key = "active/" + "scan_now";
            if (!MainWindow.icons.ContainsKey(key))
            {
                try
                {
                    MainWindow.icons.Add(key, new ImageBrush(new BitmapImage(new Uri(MainWindow.imageDirectory + key + ".png", UriKind.RelativeOrAbsolute))));
                }
                catch { }
            }
            try
            {
                lbScan.Background = MainWindow.icons[key];
            }
            catch { }

            try
            {
                lbScan.IsEnabled = false;

                CreateScanJobOptions options = new CreateScanJobOptions();
                options.AutoRun = false;
                options.Device = _scanners[cbxSources.SelectedIndex].Device;
                options.Config = new ScannerConfiguration();
                options.Config.IfShowUI = ckbShowUI.IsChecked ?? false; // Fix for CS8629
                options.Config.IfFeederEnabled = ckbADF.IsChecked ?? false; // Fix for CS8629
                options.Config.IfDuplexEnabled = ckbDuplex.IsChecked ?? false; // Fix for CS8629
                options.Config.IfDisableSourceAfterAcquire = true;
                if (rbBW.IsChecked ?? false) // Fix for CS8629
                {
                    options.Config.PixelType = EnumDWT_PixelType.TWPT_BW;
                }
                else if (rbGrey.IsChecked ?? false) // Fix for CS8629
                {
                    options.Config.PixelType = EnumDWT_PixelType.TWPT_GRAY;
                }
                else if (rbColorful.IsChecked ?? false) // Fix for CS8629
                {
                    options.Config.PixelType = EnumDWT_PixelType.TWPT_RGB;
                }

                await _mainWindow.JSInterop.ScanImageToView(options);
            }
            catch (Exception exp)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(exp.Message, "Scan error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                lbScan.IsEnabled = true;
            }
        }

        private void lbScan_MouseEnter(object sender, MouseEventArgs e)
        {
            string key = "hover/" + "scan_now";
            if (!MainWindow.icons.ContainsKey(key))
            {
                try
                {
                    MainWindow.icons.Add(key, new ImageBrush(new BitmapImage(new Uri(MainWindow.imageDirectory + key + ".png", UriKind.RelativeOrAbsolute))));
                }
                catch { }
            }
            try
            {
                lbScan.Background = MainWindow.icons[key];
            }
            catch { }
        }

        private void lbScan_MouseLeave(object sender, MouseEventArgs e)
        {
            string key = "normal/" + "scan_now";
            if (!MainWindow.icons.ContainsKey(key))
            {
                try
                {
                    MainWindow.icons.Add(key, new ImageBrush(new BitmapImage(new Uri(MainWindow.imageDirectory + key + ".png", UriKind.RelativeOrAbsolute))));
                }
                catch { }
            }
            try
            {
                lbScan.Background = MainWindow.icons[key];
            }
            catch { }
        }
    }
}
