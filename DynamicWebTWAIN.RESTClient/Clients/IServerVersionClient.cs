using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Interface for the server version client.
    /// </summary>
    public interface IServerVersionClient
    {
        /// <summary>
        /// Gets the server version.
        /// </summary>
        /// <returns></returns>
        Task<ServerVersion> Get();
    }
}
