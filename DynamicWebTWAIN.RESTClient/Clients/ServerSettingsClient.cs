using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Interface for the server settings client.
    /// </summary>
    public class ServerSettingsClient : ApiClient, IServerSettingsClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSettingsClient"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection.</param>
        public ServerSettingsClient(IApiConnection apiConnection) : base(apiConnection)
        {
            Ensure.ArgumentNotNull(apiConnection, nameof(apiConnection));
        }

        /// <summary>
        /// Returns the server settings.
        /// </summary>
        [ManualRoute("GET", "/api/server")]
        public Task<ServerSettings> Get()
        {
            return ApiConnection.Get<ServerSettings>(ApiUrls.Settings());
        }

        /// <summary>
        /// Update the server settings.
        /// </summary>
        [ManualRoute("PATCH", "/api/server")]
        public Task SetLogLevel(int logLevel)
        {
            var settings = new ServerSettingsUpdate
            {
                LogLevel = logLevel
            };

            return Update(settings);
        }

        /// <summary>
        /// Update the server settings.
        /// </summary>
        [ManualRoute("PATCH", "/api/server")]
        public Task<ServerSettings> Update(ServerSettingsUpdate settings)
        {
            return ApiConnection.Patch<ServerSettings>(ApiUrls.Settings(), settings);
        }
    }
}
