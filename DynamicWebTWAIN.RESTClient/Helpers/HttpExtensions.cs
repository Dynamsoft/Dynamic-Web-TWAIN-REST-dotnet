using System.Threading;
using System.Threading.Tasks;
using DynamicWebTWAIN.RestClient.Internal;

namespace DynamicWebTWAIN.RestClient
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Sends an HTTP request and returns the response.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Task<IResponse> Send(this IHttpClient httpClient, IRequest request)
        {
            Ensure.ArgumentNotNull(httpClient, nameof(httpClient));
            Ensure.ArgumentNotNull(request, nameof(request));

            return httpClient.Send(request, CancellationToken.None);
        }

        /// <summary>
        /// Gets a value that indicates whether the HTTP response was successful.
        /// </summary>
        public static bool IsSuccessStatusCode(this IResponse response)
        {
            Ensure.ArgumentNotNull(response, nameof(response));
            return (int) response.StatusCode >= 200 && (int) response.StatusCode <= 299;
        }
    }
}