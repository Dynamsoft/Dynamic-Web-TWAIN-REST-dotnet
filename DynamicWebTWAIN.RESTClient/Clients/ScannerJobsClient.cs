using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Interface for the scanner jobs client.
    /// </summary>
    public class ScannerJobsClient : ApiClient, IScannerJobsClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScannerJobsClient"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection.</param>
        public ScannerJobsClient(IApiConnection apiConnection) : base(apiConnection)
        {
            Ensure.ArgumentNotNull(apiConnection, nameof(apiConnection));
        }

        /// <summary>
        /// Creates a new scanner job.
        /// </summary>
        /// <returns></returns>
        [ManualRoute("POST", "/api/device/scanners/jobs")]
        public Task<IScannerJobClient> CreateJob()
        {
            var createScanJobOptions = new CreateScanJobOptions
            {
            };
            return CreateJob(createScanJobOptions);
        }

        /// <summary>
        /// Creates a new scanner job with the specified options.
        /// </summary>
        /// <param name="createScanJobOptions"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        [ManualRoute("POST", "/api/device/scanners/jobs")]
        public async Task<IScannerJobClient> CreateJob(CreateScanJobOptions createScanJobOptions)
        {
            Ensure.ArgumentNotNull(createScanJobOptions, nameof(createScanJobOptions));
            var job = await ApiConnection.Post<ScannerJob>(ApiUrls.ScannerJobs(), createScanJobOptions);
            var client = new ScannerJobClient(ApiConnection, job);
            try
            {
                await client.EnsureInitializedAsync();
            }
            catch (Exception ex)
            {
                await client.DeleteJob();
                throw new ApiException("Failed to initialize the scanner job client.", ex);
            }
            return client;
        }
    }
}
