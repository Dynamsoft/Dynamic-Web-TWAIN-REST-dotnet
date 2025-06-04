using DynamicWebTWAIN.RestClient.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicWebTWAIN.RestClient
{
    public class CreateScanJobOptions : RequestParameters
    {
        /// <summary>
        /// Default value true - run the scan job immediately after creation.
        /// If false, the job gets created but does not run.
        /// </summary>
        [Parameter(Key = "autoRun")]
        public bool? AutoRun { get; set; } = false;

        /// <summary>
        /// Default value 15 - communication time-out with scanner for an operation in seconds.
        /// </summary>
        [Parameter(Key = "scannerFailureTimeout")]
        public int? ScannerFailureTimeout { get; set; }

        /// <summary>
        /// Time-out for the scan job in seconds. Default value is 0, which never times out.
        /// </summary>
        [Parameter(Key = "jobTimeout")]
        public int? JobTimeout { get; set; }

        /// <summary>
        /// JSON string of scanner info in Device.device returned by GET /device/scanners.
        /// </summary>
        [Parameter(Key = "device")]
        public string Device { get; set; } = null;

        /// <summary>
        /// Further scanner configurations, applied in the following order: settings -> caps -> config.
        /// </summary>
        [Parameter(Key = "config")]
        public ScannerConfiguration Config { get; set; } = null;

        /// <summary>
        /// Scanner capability setting.
        /// </summary>
        [Parameter(Key = "caps")]
        public CapabilitiesUpdate Caps { get; set; } = null;

        /// <summary>
        /// TWAIN-specific scanner settings, returned by /device/scanners/twain/settings.
        /// </summary>
        [Parameter(Key = "settings")]
        public string Settings { get; set; } = null;

        /// <summary>
        /// When true, detects if the feeder is loaded on TWAIN scanners with supported drivers.
        /// Default value is false.
        /// </summary>
        [Parameter(Key = "checkFeederLoaded")]
        public bool? CheckFeederLoaded { get; set; }

        /// <summary>
        /// When true, the scanner will be opened in a separate thread.
        /// </summary>
        [Parameter(Key = "requireWebsocket")]
        public bool RequireWebsocket { get; set; } = true;

        /// <summary>
        /// When true, the scanner relative UI will bring to top.
        /// </summary>
        [Parameter(Key = "requestFocusForScanningUI")]
        public bool RequestFocusForScanningUI { get; set; } = true;
    }
}
