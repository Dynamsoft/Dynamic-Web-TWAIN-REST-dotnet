using System.Net.Http;

namespace DynamicWebTWAIN.RestClient.Internal
{
    internal static class HttpVerb
    {
        static readonly HttpMethod patch = new HttpMethod("PATCH");

        internal static HttpMethod Patch
        {
            get { return patch; }
        }
    }
}
