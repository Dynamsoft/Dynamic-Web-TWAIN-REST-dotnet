﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicWebTWAIN.RestClient.Internal
{
    /// <summary>
    /// Generic Http client. Useful for those who want to swap out System.Net.HttpClient with something else.
    /// </summary>
    /// <remarks>
    /// Most folks won't ever need to swap this out. But if you're trying to run this on Windows Phone, you might.
    /// </remarks>
    public class HttpClientAdapter : IHttpClient
    {
        readonly HttpClient _http;

        public const string RedirectCountKey = "RedirectCount";

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientAdapter"/> class.
        /// </summary>
        /// <param name="getHandler"></param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public HttpClientAdapter(Func<HttpMessageHandler> getHandler)
        {
            Ensure.ArgumentNotNull(getHandler, nameof(getHandler));

            _http = new HttpClient(new RedirectHandler { InnerHandler = getHandler() });
        }

        /// <summary>
        /// Sends the specified request and returns a response.
        /// </summary>
        /// <param name="request">A <see cref="IRequest"/> that represents the HTTP request</param>
        /// <param name="cancellationToken">Used to cancel the request</param>
        /// <param name="preprocessResponseBody">Function to preprocess HTTP response prior to deserialization (can be null)</param>
        /// <returns>A <see cref="Task" /> of <see cref="IResponse"/></returns>
        public async Task<IResponse> Send(IRequest request, CancellationToken cancellationToken, Func<object, object> preprocessResponseBody = null)
        {
            Ensure.ArgumentNotNull(request, nameof(request));

            var cancellationTokenForRequest = GetCancellationTokenForRequest(request, cancellationToken);

            using (var requestMessage = BuildRequestMessage(request))
            {
                var responseMessage = await SendAsync(requestMessage, cancellationTokenForRequest).ConfigureAwait(false);

                return await BuildResponse(responseMessage, preprocessResponseBody).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the cancellation token for the request. If the request has a timeout, it will create a linked cancellation token
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        static CancellationToken GetCancellationTokenForRequest(IRequest request, CancellationToken cancellationToken)
        {
            var cancellationTokenForRequest = cancellationToken;

            if (request.Timeout != TimeSpan.Zero)
            {
                var timeoutCancellation = new CancellationTokenSource(request.Timeout);
                var unifiedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellation.Token);

                cancellationTokenForRequest = unifiedCancellationToken.Token;
            }
            return cancellationTokenForRequest;
        }

        /// <summary>
        /// Build the response from the response message
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <param name="preprocessResponseBody"></param>
        /// <returns></returns>
        protected virtual async Task<IResponse> BuildResponse(HttpResponseMessage responseMessage, Func<object, object> preprocessResponseBody)
        {
            Ensure.ArgumentNotNull(responseMessage, nameof(responseMessage));

            object responseBody = null;
            string contentType = null;

            // We added support for downloading images,zip-files and application/octet-stream.
            // Let's constrain this appropriately.
            var binaryContentTypes = new[] {
                "application/zip" ,
                "application/x-gzip" ,
                "application/octet-stream" ,
                "application/pdf"};

            var content = responseMessage.Content;
            if (content != null)
            {
                contentType = GetContentMediaType(content);

                if (contentType != null && (contentType.StartsWith("image/") || binaryContentTypes
                        .Any(item => item.Equals(contentType, StringComparison.OrdinalIgnoreCase))))
                {
                    responseBody = await content.ReadAsStreamAsync().ConfigureAwait(false);
                }
                else
                {
                    responseBody = await content.ReadAsStringAsync().ConfigureAwait(false);
                    content.Dispose();
                }

                if (!(preprocessResponseBody is null))
                    responseBody = preprocessResponseBody(responseBody);
            }

            var responseHeaders = responseMessage.Headers.ToDictionary(h => h.Key, h => h.Value.First());

            return new Response(
                responseMessage.StatusCode,
                responseBody,
                responseHeaders,
                contentType);
        }

        /// <summary>
        /// Build the request message from the request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual HttpRequestMessage BuildRequestMessage(IRequest request)
        {
            Ensure.ArgumentNotNull(request, nameof(request));
            HttpRequestMessage requestMessage = null;
            try
            {
                var fullUri = new Uri(request.BaseAddress, request.Endpoint);
                requestMessage = new HttpRequestMessage(request.Method, fullUri);

                foreach (var header in request.Headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
                var httpContent = request.Body as HttpContent;
                if (httpContent != null)
                {
                    requestMessage.Content = httpContent;
                }

                var body = request.Body as string;
                if (body != null)
                {
                    requestMessage.Content = new StringContent(body, Encoding.UTF8, request.ContentType);
                }

                var bodyStream = request.Body as Stream;
                if (bodyStream != null)
                {
                    requestMessage.Content = new StreamContent(bodyStream);
                    requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(request.ContentType);
                }
            }
            catch (Exception)
            {
                if (requestMessage != null)
                {
                    requestMessage.Dispose();
                }
                throw;
            }

            return requestMessage;
        }

        /// <summary>
        /// Gets the content media type from the http content
        /// </summary>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        static string GetContentMediaType(HttpContent httpContent)
        {
            if (httpContent.Headers?.ContentType != null)
            {
                return httpContent.Headers.ContentType.MediaType;
            }

            // Issue #2898 - Bad "zip" Content-Type coming from Blob Storage for artifacts
            if (httpContent.Headers?.TryGetValues("Content-Type", out var contentTypeValues) == true
                && contentTypeValues.FirstOrDefault() == "zip")
            {
                return "application/zip";
            }
            
            return null;
        }

        /// <summary>
        /// Disposes the HttpClientAdapter and releases any resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the HttpClientAdapter and releases any resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_http != null) _http.Dispose();
            }
        }

        /// <summary>
        /// Sends the specified request and returns a response.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Clone the request/content in case we get a redirect
            var clonedRequest = await CloneHttpRequestMessageAsync(request).ConfigureAwait(false);

            // Send initial response
            var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            // Need to determine time on client computer as soon as possible.
            var receivedTime = DateTimeOffset.Now;
            // Since Properties are stored as objects, serialize to HTTP round-tripping string (Format: r)
            // Resolution is limited to one-second, matching the resolution of the HTTP Date header

            // Can't redirect without somewhere to redirect to.
            if (response.Headers.Location == null)
            {
                return response;
            }

            // Don't redirect if we exceed max number of redirects
            var redirectCount = 0;
            if (request.Properties.Keys.Contains(RedirectCountKey))
            {
                redirectCount = (int)request.Properties[RedirectCountKey];
            }
            if (redirectCount > 3)
            {
                throw new InvalidOperationException("The redirect count for this request has been exceeded. Aborting.");
            }

            if (response.StatusCode == HttpStatusCode.MovedPermanently
                        || response.StatusCode == HttpStatusCode.Redirect
                        || response.StatusCode == HttpStatusCode.Found
                        || response.StatusCode == HttpStatusCode.SeeOther
                        || response.StatusCode == HttpStatusCode.TemporaryRedirect
                        || (int)response.StatusCode == 308)
            {
                if (response.StatusCode == HttpStatusCode.SeeOther)
                {
                    clonedRequest.Content = null;
                    clonedRequest.Method = HttpMethod.Get;
                }

                // Increment the redirect count
                clonedRequest.Properties[RedirectCountKey] = ++redirectCount;

                // Set the new Uri based on location header
                clonedRequest.RequestUri = response.Headers.Location;

                // Clear authentication if redirected to a different host
                if (string.Compare(clonedRequest.RequestUri.Host, request.RequestUri.Host, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    clonedRequest.Headers.Authorization = null;
                }

                // Send redirected request
                response = await SendAsync(clonedRequest, cancellationToken).ConfigureAwait(false);
            }

            return response;
        }

        /// <summary>
        /// Clones the HttpRequestMessage and its content
        /// </summary>
        /// <param name="oldRequest"></param>
        /// <returns></returns>
        public static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage oldRequest)
        {
            var newRequest = new HttpRequestMessage(oldRequest.Method, oldRequest.RequestUri);

            // Copy the request's content (via a MemoryStream) into the cloned object
            var ms = new MemoryStream();
            if (oldRequest.Content != null)
            {
                await oldRequest.Content.CopyToAsync(ms).ConfigureAwait(false);
                ms.Position = 0;
                newRequest.Content = new StreamContent(ms);

                // Copy the content headers
                if (oldRequest.Content.Headers != null)
                {
                    foreach (var h in oldRequest.Content.Headers)
                    {
                        newRequest.Content.Headers.Add(h.Key, h.Value);
                    }
                }
            }

            newRequest.Version = oldRequest.Version;

            foreach (var header in oldRequest.Headers)
            {
                newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            foreach (var property in oldRequest.Properties)
            {
                newRequest.Properties.Add(property);
            }

            return newRequest;
        }

        /// <summary>
        /// Set the GitHub Api request timeout.
        /// </summary>
        /// <param name="timeout">The Timeout value</param>
        public void SetRequestTimeout(TimeSpan timeout)
        {
            _http.Timeout = timeout;
        }

        /// <summary>
        /// Adds a custom HTTP header to the request.
        /// </summary>
        /// <param name="headerName"></param>
        /// <param name="headerValue"></param>
        public void AddHttpHeader(string headerName, string headerValue)
        {
            _http.DefaultRequestHeaders.Remove(headerName);
            _http.DefaultRequestHeaders.Add(headerName, headerValue);
        }

        /// <summary>
        /// Gets a custom HTTP header from the request.
        /// </summary>
        /// <param name="headerName"></param>
        /// <returns></returns>
        public string GetHttpHeader(string headerName)
        {
            if (_http.DefaultRequestHeaders.TryGetValues(headerName, out var values))
            {
                return values.FirstOrDefault();
            }   
            return string.Empty;
        }

        /// <summary>
        /// Remove a custom HTTP header.
        /// </summary>
        /// <param name="headerName"></param>
        public void RemoveHttpHeader(string headerName)
        {
            _http.DefaultRequestHeaders.Remove(headerName);
        }
    }

    /// <summary>
    /// Redirect handler for HttpClient
    /// </summary>
    internal class RedirectHandler : DelegatingHandler
    {
    }
}
