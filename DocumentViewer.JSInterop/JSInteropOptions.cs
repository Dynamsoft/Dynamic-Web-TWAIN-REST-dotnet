using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dynamsoft.DocumentViewer
{
    public class JSInteropOptions
    {
        public string ProductKey { get; set; }
        /// <summary>
        /// The UI config name passed to the web page.
        /// </summary>
        public string UIConfig { get; set; } = null;

        /// <summary>
        /// The default site url is in the service, if you want to use your own site, please set it.
        /// </summary>
        public string SiteUrl { get; set; } = null;

        // for HybridWebView, MessageType should be "__RawMessage"
        public string MessageType { get; set; } = null;

    }
}
