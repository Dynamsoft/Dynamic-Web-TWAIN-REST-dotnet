using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Interface for the scanner jobs client.
    /// </summary>
    public interface IScannerJobsClient
    {
        /// <summary>
        /// Creates a new scanner job.
        /// </summary>
        /// <returns></returns>
        Task<IScannerJobClient> CreateJob();

        /// <summary>
        /// Creates a new scanner job with the specified options.
        /// </summary>
        /// <param name="createScanJobOptions"></param>
        /// <returns></returns>
        Task<IScannerJobClient> CreateJob(CreateScanJobOptions createScanJobOptions);

    }
}
