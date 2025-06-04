using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Interface for the server control client.
    /// </summary>
    public class ServerControlClient : ApiClient, IServerControlClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerControlClient"/> class.
        /// </summary>
        /// <param name="apiConnection">The API connection.</param>
        public ServerControlClient(IApiConnection apiConnection) : base(apiConnection)
        {
            Ensure.ArgumentNotNull(apiConnection, nameof(apiConnection));

            Version = new ServerVersionClient(apiConnection);
            Settings = new ServerSettingsClient(apiConnection);
        }

        /// <summary>
        /// Gets the server version client.
        /// </summary>
        public IServerVersionClient Version { get; private set; }

        /// <summary>
        /// Gets the server settings client.
        /// </summary>
        public IServerSettingsClient Settings { get; private set; }
    }
}
