using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicWebTWAIN.RestClient
{
    public class ServerSettingsUpdate : RequestParameters
    {
        /// <summary>
        /// The server's log level.
        /// </summary>
        public int LogLevel { get; set; }
    }
}
