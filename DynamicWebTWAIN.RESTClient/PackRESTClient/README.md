## Dynamic Web TWAIN Rest Client

This is the .NET wrapper of [Dynamic Web TWAIN](https://www.dynamsoft.com/web-twain/overview/)'s REST API. It enables developers to create desktop or cross-platform applications to scan and digitize documents using:

* TWAIN (32-bit / 64-bit)
* WIA (Windows Image Acquisition)
* SANE (Linux)
* ICA (macOS)
* eSCL (AirScan / Mopria)

### Prerequisites

1. You need to install Dynamic Web TWAIN service or use the `Dynamsoft.DynamicWebTWAIN.Service` package to start the service in your app.
2. Get a license of Dynamic Web TWAIN: [30-day trial](https://www.dynamsoft.com/customer/license/trialLicense/?product=dwt)

### Quickstart

```csharp
var client = new DWTClient(new Uri(IpAddress), LicenseKey);
var scanners = await client.ScannerControlClient.ScannerManager.GetScanners(EnumDeviceTypeMask.DT_WIATWAINSCANNER | EnumDeviceTypeMask.DT_TWAINSCANNER);
if (scanners.Count>0) {
    CreateScanJobOptions options = new CreateScanJobOptions();
    options.AutoRun = false;
    options.Device = scanners[0].Device;
    options.Config = new ScannerConfiguration();
    options.Config.IfFeederEnabled = true;
    options.Config.IfDuplexEnabled = true;
    var job = await DWTClient.ScannerControlClient.ScannerJobs.CreateJob(options);
    await job.StartJob();
    do
    {
        var result = await job.GetNextImage();
        if (result == null)
        {
            await job.DeleteJob();
        }
        else
        {
            //process the image bytes
        }
    } while (true);
}
```

