using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;

namespace DynamicWebTWAIN.RestClient.Internal
{
    /// <summary>
    /// Factory class for creating default HttpMessageHandler instances.
    /// </summary>
    public static class HttpMessageHandlerFactory
    {
        /// <summary>
        /// Creates a default HttpMessageHandler instance.
        /// </summary>
        /// <returns></returns>
        public static HttpMessageHandler CreateDefault()
        {
            return CreateDefault(null);
        }

        /// <summary>
        /// Creates a default HttpMessageHandler instance with the specified proxy.
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "proxy")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static HttpMessageHandler CreateDefault(IWebProxy proxy)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                ServerCertificateCustomValidationCallback = delegate { return true; }
            };

            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            if (handler.SupportsProxy && proxy != null)
            {
                handler.UseProxy = true;
                handler.Proxy = proxy;
            }

            return handler;
        }
    }
}
