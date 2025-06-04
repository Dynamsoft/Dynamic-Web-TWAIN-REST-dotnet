using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Interface for the server settings client.
    /// </summary>
    public interface IServerSettingsClient
    {
        /// <summary>
        /// Gets the server settings.
        /// </summary>
        /// <returns></returns>
        Task<ServerSettings> Get();

        /// <summary>
        /// Sets the log level.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        Task SetLogLevel(int logLevel);

        /// <summary>
        /// Update the server settings.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        Task<ServerSettings> Update(ServerSettingsUpdate settings);
    }
}
