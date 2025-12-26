using DynamicWebTWAIN.RestClient;
using DynamicWebTWAIN.Service;

namespace ConsoleApp;

class Program
{
    private static string productKey = "DLS2eyJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSJ9";
    private static int totalScannedPages = 0;

    static async Task Main(string[] args)
    {
        ServiceManager? serviceManager = null;
        DWTClient? dwtClient = null;
        string? documentId = null;
        Scanner? selectedScanner = null;

        try
        {
            Console.WriteLine("=== Dynamic Web TWAIN Console Application ===\n");

            // Initialize service
            Console.WriteLine("Initializing service...");
            serviceManager = new ServiceManager();
            serviceManager.CreateService();
            
            dwtClient = new DWTClient(serviceManager.Service.BaseAddress, productKey);
            Console.WriteLine("Service initialized successfully!\n");

            // List all scanners
            Console.WriteLine("Getting scanner list...");
            var scanners = await dwtClient.ScannerControlClient.ScannerManager.GetScanners(EnumDeviceTypeMask.DT_TWAINSCANNER);
            
            if (scanners == null || scanners.Count == 0)
            {
                Console.WriteLine("No scanners found.");
                return;
            }

            // Create document
            Console.WriteLine("Creating document...");
            CreateDocumentOptions docOptions = new CreateDocumentOptions();
            docOptions.Name = "ScannedDocument_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var document = await dwtClient.DocumentManagerClient.CreateDocument(docOptions);

            if (document == null)
            {
                Console.WriteLine("Failed to create document.");
                return;
            }

            documentId = document.Uid;
            Console.WriteLine($"Document created successfully, ID: {documentId}\n");

            // Main menu loop
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n=== Main Menu ===");
                Console.WriteLine($"Current Scanner: {(selectedScanner != null ? selectedScanner.Name : "Not selected")}");
                Console.WriteLine($"Total scanned pages: {totalScannedPages}");
                Console.WriteLine("1. Select Source");
                Console.WriteLine("2. Scan");
                Console.WriteLine("3. Save as PDF");
                Console.WriteLine("4. Exit");
                Console.Write("\nSelect option: ");
                
                string? choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        selectedScanner = await SelectSource(scanners);
                        break;
                    case "2":
                        if (selectedScanner == null)
                        {
                            Console.WriteLine("\nPlease select a scanner first (Option 1).");
                        }
                        else
                        {
                            await ScanDocument(dwtClient, selectedScanner, documentId);
                        }
                        break;
                    case "3":
                        await SaveDocumentAsPDF(dwtClient, documentId);
                        break;
                    case "4":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError occurred: {ex.Message}");
            Console.WriteLine($"Details: {ex.StackTrace}");
        }
        finally
        {
            // Cleanup resources
            dwtClient?.Dispose();
            serviceManager?.Dispose();
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }

    private static async Task<Scanner?> SelectSource(IReadOnlyList<Scanner> scanners)
    {
        try
        {
            Console.WriteLine("\n--- Select Scanner Source ---");
            Console.WriteLine($"\nFound {scanners.Count} scanner(s):");
            for (int i = 0; i < scanners.Count; i++)
            {
                Console.WriteLine($"  [{i}] {scanners[i].Name}");
            }

            Console.Write("\nSelect scanner index (default 0): ");
            string? input = Console.ReadLine();
            int selectedIndex = 0;
            if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int index) && index >= 0 && index < scanners.Count)
            {
                selectedIndex = index;
            }

            var selectedScanner = scanners[selectedIndex];
            Console.WriteLine($"Selected: {selectedScanner.Name}");
            
            return selectedScanner;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error selecting scanner: {ex.Message}");
            return null;
        }
    }

    private static async Task ScanDocument(DWTClient dwtClient, Scanner scanner, string documentId)
    {
        try
        {
            Console.WriteLine("\n--- Scanning ---");
            
            // Configure scan job
            CreateScanJobOptions options = new CreateScanJobOptions();
            options.AutoRun = false;
            options.Device = scanner.Device;
            options.Config = new ScannerConfiguration();
            options.Config.IfShowUI = false;
            options.Config.IfFeederEnabled = false;
            options.Config.IfDuplexEnabled = false;
            options.Config.IfDisableSourceAfterAcquire = true;
            options.Config.PixelType = EnumDWT_PixelType.TWPT_RGB;

            // Create scan job
            var jobClient = await dwtClient.ScannerControlClient.ScannerJobs.CreateJob(options);

            // Scan listener
            var tcs = new TaskCompletionSource<bool>();
            var scannedCount = 0;
            var processingCount = 0;
            var lockObj = new object();

            jobClient.PageScanned += async (sender, e) =>
            {
                lock (lockObj) 
                { 
                    processingCount++; 
                    scannedCount++;
                    Console.WriteLine($"Processing page {scannedCount}...");
                }

                try
                {
                    await dwtClient.DocumentManagerClient.AddImageToDocument(documentId, e.Url);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing image: {ex.Message}");
                }
                finally
                {
                    lock (lockObj) { processingCount--; }
                }
            };

            jobClient.TransferEnded += async (sender, e) =>
            {
                // Wait for all pages to complete processing
                for (int i = 0; i < 100; i++)
                {
                    lock (lockObj)
                    {
                        if (processingCount == 0) break;
                    }
                    await Task.Delay(100);
                }
                tcs.SetResult(true);
            };

            // Start scanning
            Console.WriteLine("Starting scan, please place document in scanner...");
            await jobClient.StartJob();

            // Wait for scan completion
            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(120)));
            
            if (completedTask != tcs.Task)
            {
                Console.WriteLine("\nScan timeout.");
                return;
            }

            totalScannedPages += scannedCount;
            Console.WriteLine($"\nScan completed! Pages scanned in this session: {scannedCount}");
            Console.WriteLine($"Total scanned pages: {totalScannedPages}");

            // Cleanup
            await jobClient.DeleteJob();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during scanning: {ex.Message}");
        }
    }

    private static async Task SaveDocumentAsPDF(DWTClient dwtClient, string documentId)
    {
        try
        {
            if (totalScannedPages == 0)
            {
                Console.WriteLine("\nNo pages scanned yet. Please scan documents first.");
                return;
            }

            Console.WriteLine("\n--- Saving as PDF ---");
            Console.WriteLine("Generating PDF file...");
            
            byte[] pdfBlob = await dwtClient.DocumentManagerClient.SaveDocumentAsPDF(documentId);
            
            if (pdfBlob == null || pdfBlob.Length == 0)
            {
                Console.WriteLine("Failed to generate PDF.");
                return;
            }

            // Save to desktop
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"ScannedDocument_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = Path.Combine(desktopPath, fileName);
            
            File.WriteAllBytes(filePath, pdfBlob);
            
            Console.WriteLine($"\n✓ PDF saved successfully!");
            Console.WriteLine($"  File location: {filePath}");
            Console.WriteLine($"  File size: {pdfBlob.Length / 1024.0:F2} KB");
            Console.WriteLine($"  Total pages: {totalScannedPages}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving PDF: {ex.Message}");
        }
    }
}
