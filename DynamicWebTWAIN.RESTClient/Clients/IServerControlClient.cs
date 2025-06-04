using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Interface for the server control client.
    /// </summary>
    public interface IServerControlClient
    {
        /// <summary>
        /// Gets the server version client.
        /// </summary>
        IServerVersionClient Version { get; }

        /// <summary>
        /// Gets the server settings client.
        /// </summary>
        IServerSettingsClient Settings { get; }
    }
}
