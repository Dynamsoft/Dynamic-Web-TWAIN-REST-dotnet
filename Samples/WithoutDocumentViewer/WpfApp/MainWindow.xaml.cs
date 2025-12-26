using DynamicWebTWAIN.RestClient;
using System.Windows;
using System.IO;
using DynamicWebTWAIN.Service;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;

namespace WpfWebviewApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private DWTClient? _dwtClient;
    private ServiceManager? _serviceManager;
    private IReadOnlyList<Scanner>? _scanners;
    private string productKey = "DLS2eyJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSJ9";
    private ObservableCollection<BitmapImage> _scannedImages = new ObservableCollection<BitmapImage>();
    private string _documentId;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _serviceManager = new ServiceManager();
            _serviceManager.CreateService();
            
            // Use DWTClient directly without JSInterop
            _dwtClient = new DWTClient(_serviceManager.Service.BaseAddress, productKey);

            _scanners = await _dwtClient.ScannerControlClient.ScannerManager.GetScanners(EnumDeviceTypeMask.DT_TWAINSCANNER);

            cbxSources.Items.Clear();
            foreach (var scanner in _scanners)
            {
                cbxSources.Items.Add(scanner.Name);
            }
            if (cbxSources.Items.Count > 0)
                cbxSources.SelectedIndex = 0;

            thumbnailList.ItemsSource = _scannedImages;

            // Create a new document
            CreateDocumentOptions docOptions = new CreateDocumentOptions();
            docOptions.Name = "ScannedDocument_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

            var document = await _dwtClient.DocumentManagerClient.CreateDocument(docOptions);

            if (document == null)
            {
                MessageBox.Show("Failed to create document.");
                return;
            }

            _documentId = document.Uid;

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Initialization error: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _dwtClient?.Dispose();
        _serviceManager?.Dispose();
    }

    private async void btnScanToView_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_dwtClient == null || _scanners == null)
            {
                MessageBox.Show("System not initialized.");
                return;
            }

            if (cbxSources.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a scanner first.");
                return;
            }

            // Create scan job options
            CreateScanJobOptions options = new CreateScanJobOptions();
            options.AutoRun = false;  // Use event-driven scanning mode
            options.Device = _scanners[cbxSources.SelectedIndex].Device;
            options.Config = new ScannerConfiguration();
            options.Config.IfShowUI = false;
            options.Config.IfFeederEnabled = false;
            options.Config.IfDuplexEnabled = false;
            options.Config.IfDisableSourceAfterAcquire = true;
            options.Config.PixelType = EnumDWT_PixelType.TWPT_RGB; // Color scanning
            
            // Create scan job
            var jobClient = await _dwtClient.ScannerControlClient.ScannerJobs.CreateJob(options);
            
            
            // Use TaskCompletionSource to wait for scanning completion
            var tcs = new TaskCompletionSource<bool>();
            var scannedImages = new List<byte[]>();
            var processingCount = 0;
            var lockObj = new object();

            // Subscribe to PageScanned event
            jobClient.PageScanned += async (sender, e) =>
            {
                lock (lockObj) { processingCount++; }
                try
                {
                    var imageData = await jobClient.GetImageByUrl(e.Url);
                    scannedImages.Add(imageData);

                    await _dwtClient.DocumentManagerClient.AddImageToDocument(_documentId, e.Url);
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Error getting scanned image: {ex.Message}", "Scan error");
                    });
                }
                finally
                {
                    lock (lockObj) { processingCount--; }
                }
            };

            // Subscribe to TransferEnded event
            jobClient.TransferEnded += async (sender, e) =>
            {
                // Wait for all PageScanned handlers to complete
                for (int i = 0; i < 100; i++) // Max wait 10 seconds
                {
                    lock (lockObj)
                    {
                        if (processingCount == 0) break;
                    }
                    await Task.Delay(100);
                }
                tcs.SetResult(true);
            };
            
            // Start scan job
            await jobClient.StartJob();

            // Wait for scan completion (with timeout)
            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(120)));
            
            if (completedTask != tcs.Task)
            {
                MessageBox.Show("Scan timeout.", "Scan error");
            }
            else
            {
                // Add all scanned images to display list
                if (scannedImages.Count > 0)
                {
                    // Append new scanned images without clearing existing ones
                    int firstNewImageIndex = _scannedImages.Count;
                    
                    foreach (var imageData in scannedImages)
                    {
                        var bitmapImage = BytesToBitmapImage(imageData);
                        _scannedImages.Add(bitmapImage);
                    }
                    
                    // Select the first newly scanned image
                    if (_scannedImages.Count > 0)
                    {
                        mainImage.Source = _scannedImages[firstNewImageIndex];
                        thumbnailList.SelectedIndex = firstNewImageIndex;
                    }
                    
                    //MessageBox.Show($"Successfully scanned {scannedImages.Count} image(s).\nTotal images: {_scannedImages.Count}");
                }
                else
                {
                    MessageBox.Show("No images were scanned.", "Scan Info");
                }
            }
            
            // Cleanup
           // await jobClient.DeleteJob();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Scan error: {ex.Message}\n{ex.StackTrace}");
        }
    }



    private async void btnSaveAsPdf_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_dwtClient == null)
            {
                MessageBox.Show("System not initialized.");
                return;
            }

            if (_scannedImages.Count == 0)
            {
                MessageBox.Show("No images to save. Please scan images first.");
                return;
            }

            // Call SaveDocumentAsPDF to get PDF blob (equivalent to: let blob = await response.blob())
            byte[] pdfBlob = await _dwtClient.DocumentManagerClient.SaveDocumentAsPDF(_documentId);
            
            if (pdfBlob == null || pdfBlob.Length == 0)
            {
                MessageBox.Show("Failed to get PDF blob data.");
                return;
            }
            
            // Save blob as local PDF file
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"ScannedDocument_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            File.WriteAllBytes(filePath, pdfBlob);
            
            MessageBox.Show($"PDF saved successfully!\nLocation: {filePath}\nTotal pages: {_scannedImages.Count}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Save PDF error: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void ThumbnailList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (thumbnailList.SelectedIndex >= 0 && thumbnailList.SelectedIndex < _scannedImages.Count)
        {
            mainImage.Source = _scannedImages[thumbnailList.SelectedIndex];
        }
    }

    private BitmapImage BytesToBitmapImage(byte[] imageBytes)
    {
        var bitmapImage = new BitmapImage();
        using (var stream = new MemoryStream(imageBytes))
        {
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
        }
        return bitmapImage;
    }
}
