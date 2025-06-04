using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Interface for the server version client.
    /// </summary>
    public class ServerVersionClient : ApiClient, IServerVersionClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerVersionClient"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection.</param>
        public ServerVersionClient(IApiConnection apiConnection) : base(apiConnection)
        {
            Ensure.ArgumentNotNull(apiConnection, nameof(apiConnection));
        }

        /// <summary>
        /// Returns the server version.
        /// </summary>
        [ManualRoute("GET", "/api/server/version")]
        public Task<ServerVersion> Get()
        {
            return ApiConnection.Get<ServerVersion>(ApiUrls.Version());
        }

    }
}
