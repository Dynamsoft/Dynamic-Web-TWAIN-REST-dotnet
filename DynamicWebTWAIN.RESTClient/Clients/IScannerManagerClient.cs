using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Interface for the scanner manager client.
    /// </summary>
    public interface IScannerManagerClient
    {
        /// <summary>
        /// Gets the list of scanners.
        /// </summary>
        /// <param name="typeMask"></param>
        /// <returns></returns>
        Task<IReadOnlyList<Scanner>> GetScanners(EnumDeviceTypeMask typeMask);
    }
}
