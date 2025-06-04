using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Interface for the scanner control client.
    /// </summary>
    public interface IScannerControlClient
    {
        /// <summary>
        /// Gets the scanner manager client.
        /// </summary>
        IScannerManagerClient ScannerManager { get; }

        /// <summary>
        /// Gets the scanner jobs client.
        /// </summary>
        IScannerJobsClient ScannerJobs { get; }
    }
}
