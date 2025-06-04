using DynamicWebTWAIN.RestClient.Internal;

namespace DynamicWebTWAIN.RestClient
{
    public class ScannerJob : RequestParameters
    {
        /// <summary>
        /// Unique identifier for the job.
        /// </summary>
        [Parameter(Key = "jobuid")]
        public string Jobuid { get; private set; }

        /// <summary>
        /// Current status of the job.
        /// </summary>
        [Parameter(Key = "status")]
        public StringEnum<JobStatus> Status { get; private set; }

        /// <summary>
        /// Scanner associated with the job.
        /// </summary>
        [Parameter(Key = "scanner")]
        public Scanner Scanner { get; private set; }

        /// <summary>
        /// Websocket protocol details for the job.
        /// </summary>
        [Parameter(Key = "protocol")]
        public WebsocketProtocol Protocol { get; private set; } = null;
    }

    public class WebsocketProtocol : RequestParameters
    {
        [Parameter(Key = "callback")]
        public WebsocketCallback Callback { get; private set; } = null;

        [Parameter(Key = "jobuid")]
        public string Jobuid { get; private set; } = null;

        [Parameter(Key = "websocket")]
        public Websocket Websocket { get; private set; } = null;
    }

    public class WebsocketCallback : RequestParameters
    {
        [Parameter(Key = "method")]
        public string Method { get; private set; } = null;

        [Parameter(Key = "url")]
        public string Url { get; private set; } = null;

    }

    public class Websocket : RequestParameters
    {
        /// <summary>
        /// Protocol used for the websocket.
        /// </summary>
        [Parameter(Key = "protocol")]
        public string Protocol { get; private set; } = null;

        /// <summary>
        /// Websocket server address.
        /// </summary>
        [Parameter(Key = "server")]
        public string Server { get; private set; } = null;

        /// <summary>
        /// Websocket response details.
        /// </summary>
        [Parameter(Key = "response")]
        public WebsocketResponse Response { get; private set; } = null;
    }

    public class WebsocketResponse : RequestParameters
    {
        [Parameter(Key = "jobuid")]
        public string Jobuid { get; private set; } = null;

        [Parameter(Key = "method")]
        public string Method { get; private set; } = null;
    }
}
