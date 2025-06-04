using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Interface for the scanner control client.
    /// </summary>
    public class ScannerControlClient : ApiClient, IScannerControlClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScannerControlClient"/> class.
        /// </summary>
        /// <param name="apiConnection"></param>
        public ScannerControlClient(IApiConnection apiConnection) : base(apiConnection)
        {
            Ensure.ArgumentNotNull(apiConnection, nameof(apiConnection));

            ScannerManager = new ScannerManagerClient(apiConnection);
            ScannerJobs = new ScannerJobsClient(apiConnection);
        }

        /// <summary>
        /// Gets the scanner manager client.
        /// </summary>
        public IScannerManagerClient ScannerManager { get; private set; }

        /// <summary>
        /// Gets the scanner jobs client.
        /// </summary>
        public IScannerJobsClient ScannerJobs { get; private set; }
    }
}
