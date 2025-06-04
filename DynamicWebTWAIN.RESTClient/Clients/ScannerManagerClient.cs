using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Interface for the scanner manager client.
    /// </summary>
    public class ScannerManagerClient : ApiClient, IScannerManagerClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScannerManagerClient"/> class.
        /// </summary>
        /// <param name="apiConnection"></param>
        public ScannerManagerClient(IApiConnection apiConnection) : base(apiConnection)
        {
            Ensure.ArgumentNotNull(apiConnection, nameof(apiConnection));
        }

        /// <summary>
        /// Gets the list of scanners.
        /// </summary>
        /// <param name="typeMask"></param>
        /// <returns></returns>
        [ManualRoute("GET", "/api/device/scanners")]
        public Task<IReadOnlyList<Scanner>> GetScanners(EnumDeviceTypeMask typeMask)
        {
            var query = new Dictionary<string, string>
            {
                { "type", Convert.ToInt32(typeMask).ToString() }
            };
            return ApiConnection.Get<IReadOnlyList<Scanner>>(ApiUrls.Scanners(), query);
        }

    }
}
