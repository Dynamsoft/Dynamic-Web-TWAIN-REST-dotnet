// Copyright (c) 2017 GitHub, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of 
// this software and associated documentation files (the "Software"), to deal in 
// the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// original code from https://github.com/octokit/octokit.net

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DynamicWebTWAIN.RestClient.Internal;

namespace DynamicWebTWAIN.RestClient
{
    // NOTE: Every request method must go through the `RunRequest` code path. So if you need to add a new method
    // ensure it goes through there. :)
    /// <summary>
    /// A connection for making HTTP requests against URI endpoints.
    /// </summary>
    public class Connection : IConnection
    {
        static readonly Uri _defaultGitHubApiUrl = DWTClient.DWTApiUrl;
        static readonly string _uuid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        readonly JsonHttpPipeline _jsonPipeline;
        internal IHttpClient _httpClient;
        
        internal Connection()
        {
            _httpClient = new HttpClientAdapter(HttpMessageHandlerFactory.CreateDefault);
            _jsonPipeline = new JsonHttpPipeline(new SimpleJsonSerializer());

            _httpClient.SetRequestTimeout(TimeSpan.FromMilliseconds(1800000)); // default is 1800 secs
            _httpClient.AddHttpHeader("Origin", string.Format("dynamic-web-twain.net://{0}", _uuid));
        }

        /// <summary>
        /// Creates a new connection instance used to make requests of the DWT Rest API.
        /// </summary>
        /// <param name="baseAddress">
        /// The address to point this client to such as https://127.0.0.1:18623 instance
        /// </param>
        public Connection(Uri baseAddress)
            : this()
        {
            Ensure.ArgumentNotNull(baseAddress, nameof(baseAddress));

            if (!baseAddress.IsAbsoluteUri)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "The base address '{0}' must be an absolute URI",
                        baseAddress), nameof(baseAddress));
            }

            BaseAddress = baseAddress;
        }
        public Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return SendData<T>(uri.ApplyParameters(parameters), HttpMethod.Get, null, null, null, CancellationToken.None);
        }

        /// <summary>
        /// Performs an asynchronous HTTP GET request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="parameters"></param>
        /// <param name="accepts"></param>
        /// <returns></returns>
        public Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return SendData<T>(uri.ApplyParameters(parameters), HttpMethod.Get, null, accepts, null, CancellationToken.None);
        }

        /// <summary>
        /// Performs an asynchronous HTTP GET request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="parameters"></param>
        /// <param name="accepts"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters, string accepts, CancellationToken cancellationToken)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return SendData<T>(uri.ApplyParameters(parameters), HttpMethod.Get, null, accepts, null, cancellationToken);
        }

        /// <summary>
        /// Performs an asynchronous HTTP GET request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="parameters"></param>
        /// <param name="accepts"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="preprocessResponseBody"></param>
        /// <returns></returns>
        public Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters, string accepts, CancellationToken cancellationToken, Func<object, object> preprocessResponseBody)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return SendData<T>(uri.ApplyParameters(parameters), HttpMethod.Get, null, accepts, null, cancellationToken, null, preprocessResponseBody);
        }

        /// <summary>
        /// Performs an asynchronous HTTP GET request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<IApiResponse<T>> Get<T>(Uri uri, TimeSpan timeout)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return SendData<T>(uri, HttpMethod.Get, null, null, null, timeout, CancellationToken.None);
        }

        /// <summary>
        /// Performs an asynchronous HTTP GET request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="parameters">Querystring parameters for the request</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        /// <remarks>The <see cref="IResponse.Body"/> property will be <c>null</c> if the <paramref name="uri"/> points to a directory instead of a file</remarks>
        public Task<IApiResponse<byte[]>> GetRaw(Uri uri, IDictionary<string, string> parameters)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return GetRaw(new Request
            {
                Method = HttpMethod.Get,
                BaseAddress = BaseAddress,
                Endpoint = uri.ApplyParameters(parameters)
            });
        }

        /// <inheritdoc/>
        public Task<IApiResponse<byte[]>> GetRaw(Uri uri, IDictionary<string, string> parameters, TimeSpan timeout)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return GetRaw(new Request
            {
                Method = HttpMethod.Get,
                BaseAddress = BaseAddress,
                Endpoint = uri.ApplyParameters(parameters),
                Timeout = timeout
            });
        }

        /// <inheritdoc/>
        public Task<IApiResponse<Stream>> GetRawStream(Uri uri, IDictionary<string, string> parameters)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return GetRawStream(new Request
            {
                Method = HttpMethod.Get,
                BaseAddress = BaseAddress,
                Endpoint = uri.ApplyParameters(parameters)
            });
        }

        /// <summary>
        /// Performs an asynchronous HTTP PATCH request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public Task<IApiResponse<T>> Patch<T>(Uri uri, object body)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            return SendData<T>(uri, HttpVerb.Patch, body, null, null, CancellationToken.None);
        }

        /// <summary>
        /// Performs an asynchronous HTTP PATCH request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <param name="accepts"></param>
        /// <returns></returns>
        public Task<IApiResponse<T>> Patch<T>(Uri uri, object body, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));
            Ensure.ArgumentNotNull(accepts, nameof(accepts));

            return SendData<T>(uri, HttpVerb.Patch, body, accepts, null, CancellationToken.None);
        }

        /// <summary>
        /// Performs an asynchronous HTTP POST request.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        public async Task<HttpStatusCode> Post(Uri uri, CancellationToken cancellationToken = default)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var response = await SendData<object>(uri, HttpMethod.Post, null, null, null, cancellationToken).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP POST request.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <param name="accepts"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> Post(Uri uri, object body, string accepts, CancellationToken cancellationToken = default)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var response = await SendData<object>(uri, HttpMethod.Post, body, accepts, null, cancellationToken).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP POST request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IApiResponse<T>> Post<T>(Uri uri, CancellationToken cancellationToken = default)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return SendData<T>(uri, HttpMethod.Post, null, null, null, cancellationToken);
        }

        /// <summary>
        /// Performs an asynchronous HTTP POST request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <param name="accepts"></param>
        /// <param name="contentType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType, CancellationToken cancellationToken = default)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            return SendData<T>(uri, HttpMethod.Post, body, accepts, contentType, cancellationToken);
        }

        /// <summary>
        /// Performs an asynchronous HTTP POST request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <param name="accepts"></param>
        /// <param name="contentType"></param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IApiResponse<T>> Post<T>(
            Uri uri,
            object body,
            string accepts,
            string contentType,
            IDictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            return SendData<T>(uri.ApplyParameters(parameters), HttpMethod.Post, body, accepts, contentType, cancellationToken);
        }

        /// <summary>
        /// Performs an asynchronous HTTP POST request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <param name="accepts"></param>
        /// <param name="contentType"></param>
        /// <param name="timeout"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            return SendData<T>(uri, HttpMethod.Post, body, accepts, contentType, timeout, cancellationToken);
        }

        /// <summary>
        /// Performs an asynchronous HTTP POST request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <param name="accepts"></param>
        /// <param name="contentType"></param>
        /// <param name="baseAddress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType, Uri baseAddress, CancellationToken cancellationToken = default)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            return SendData<T>(uri, HttpMethod.Post, body, accepts, contentType, cancellationToken, baseAddress: baseAddress);
        }

        /// <summary>
        /// Performs an asynchronous HTTP PUT request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public Task<IApiResponse<T>> Put<T>(Uri uri, object body)
        {
            return SendData<T>(uri, HttpMethod.Put, body, null, null, CancellationToken.None);
        }


        /// <summary>
        /// Performs an asynchronous HTTP PUT request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <param name="twoFactorAuthenticationCode"></param>
        /// <returns></returns>
        public Task<IApiResponse<T>> Put<T>(Uri uri, object body, string twoFactorAuthenticationCode)
        {
            return SendData<T>(uri,
                HttpMethod.Put,
                body,
                null,
                null,
                CancellationToken.None);
        }

        /// <summary>
        /// Performs an asynchronous HTTP PUT request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <param name="twoFactorAuthenticationCode"></param>
        /// <param name="accepts"></param>
        /// <returns></returns>
        public Task<IApiResponse<T>> Put<T>(Uri uri, object body, string twoFactorAuthenticationCode, string accepts)
        {
            return SendData<T>(uri,
                HttpMethod.Put,
                body,
                accepts,
                null,
                CancellationToken.None);
        }

        /// <summary>
        /// Performs an asynchronous HTTP request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <param name="body"></param>
        /// <param name="accepts"></param>
        /// <param name="contentType"></param>
        /// <param name="timeout"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="baseAddress"></param>
        /// <param name="preprocessResponseBody"></param>
        /// <returns></returns>
        Task<IApiResponse<T>> SendData<T>(
            Uri uri,
            HttpMethod method,
            object body,
            string accepts,
            string contentType,
            TimeSpan timeout,
            CancellationToken cancellationToken,
            Uri baseAddress = null, 
            Func<object, object> preprocessResponseBody = null)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.GreaterThanZero(timeout, nameof(timeout));

            var request = new Request
            {
                Method = method,
                BaseAddress = baseAddress ?? BaseAddress,
                Endpoint = uri,
                Timeout = timeout
            };

            return SendDataInternal<T>(body, accepts, contentType, cancellationToken, request, preprocessResponseBody);
        }

        /// <summary>
        /// Performs an asynchronous HTTP request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <param name="body"></param>
        /// <param name="accepts"></param>
        /// <param name="contentType"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="baseAddress"></param>
        /// <param name="preprocessResponseBody"></param>
        /// <returns></returns>
        Task<IApiResponse<T>> SendData<T>(
            Uri uri,
            HttpMethod method,
            object body,
            string accepts,
            string contentType,
            CancellationToken cancellationToken,
            Uri baseAddress = null,
            Func<object, object> preprocessResponseBody = null)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var request = new Request
            {
                Method = method,
                BaseAddress = baseAddress ?? BaseAddress,
                Endpoint = uri
            };

            return SendDataInternal<T>(body, accepts, contentType, cancellationToken, request, preprocessResponseBody);
        }

        /// <summary>
        /// Performs an asynchronous HTTP request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="body"></param>
        /// <param name="accepts"></param>
        /// <param name="contentType"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="request"></param>
        /// <param name="preprocessResponseBody"></param>
        /// <returns></returns>
        Task<IApiResponse<T>> SendDataInternal<T>(object body, string accepts, string contentType, CancellationToken cancellationToken, 
            Request request, Func<object, object> preprocessResponseBody)
        {
            if (!string.IsNullOrEmpty(accepts))
            {
                request.Headers["Accept"] = accepts;
            }

            if (body != null)
            {
                request.Body = body;
                request.ContentType = contentType;
            }

            return Run<T>(request, cancellationToken, preprocessResponseBody);
        }

        /// <summary>
        /// Performs an asynchronous HTTP PATCH request.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        public async Task<HttpStatusCode> Patch(Uri uri)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var request = new Request
            {
                Method = HttpVerb.Patch,
                BaseAddress = BaseAddress,
                Endpoint = uri
            };
            var response = await Run<object>(request, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP PATCH request.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        public async Task<HttpStatusCode> Patch(Uri uri, object body)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            var response = await SendData<object>(uri, new HttpMethod("PATCH"), body, null, null, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP PATCH request.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="accepts">Specifies accept response media type</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        public async Task<HttpStatusCode> Patch(Uri uri, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(accepts, nameof(accepts));

            var response = await SendData<object>(uri, new HttpMethod("PATCH"), null, accepts, null, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP PATCH request.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accept response media type</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        public async Task<HttpStatusCode> Patch(Uri uri, object body, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));
            Ensure.ArgumentNotNull(accepts, nameof(accepts));

            var response = await SendData<object>(uri, new HttpMethod("PATCH"), body, accepts, null, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP PUT request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        public async Task<HttpStatusCode> Put(Uri uri)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var request = new Request
            {
                Method = HttpMethod.Put,
                BaseAddress = BaseAddress,
                Endpoint = uri
            };
            var response = await Run<object>(request, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP PUT request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        public async Task<HttpStatusCode> Put(Uri uri, object body)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            var response = await SendData<object>(uri, HttpMethod.Put, body, null, null, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        public async Task<HttpStatusCode> Delete(Uri uri)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var request = new Request
            {
                Method = HttpMethod.Delete,
                BaseAddress = BaseAddress,
                Endpoint = uri
            };
            var response = await Run<object>(request, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="twoFactorAuthenticationCode">Two Factor Code</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        public async Task<HttpStatusCode> Delete(Uri uri, string twoFactorAuthenticationCode)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var response = await SendData<object>(uri, HttpMethod.Delete, null, null, null, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        public async Task<HttpStatusCode> Delete(Uri uri, object data)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(data, nameof(data));

            var request = new Request
            {
                Method = HttpMethod.Delete,
                Body = data,
                BaseAddress = BaseAddress,
                Endpoint = uri
            };
            var response = await Run<object>(request, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accept response media type</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        public async Task<HttpStatusCode> Delete(Uri uri, object data, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(accepts, nameof(accepts));

            var response = await SendData<object>(uri, HttpMethod.Delete, data, accepts, null, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request.
        /// </summary>
        /// <typeparam name="T">The API resource's type.</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        public Task<IApiResponse<T>> Delete<T>(Uri uri, object data)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(data, nameof(data));

            return SendData<T>(uri, HttpMethod.Delete, data, null, null, CancellationToken.None);
        }

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accept response media type</param>
        public Task<IApiResponse<T>> Delete<T>(Uri uri, object data, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(accepts, nameof(accepts));

            return SendData<T>(uri, HttpMethod.Delete, data, accepts, null, CancellationToken.None);
        }

        /// <summary>
        /// Base address for the connection.
        /// </summary>
        public Uri BaseAddress { get; private set; }

        /// <summary>
        /// Performs an asynchronous HTTP GET request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        async Task<IApiResponse<byte[]>> GetRaw(IRequest request)
        {
            var response = await RunRequest(request, CancellationToken.None).ConfigureAwait(false);

            if (response.Body is Stream stream)
            {
                return new ApiResponse<byte[]>(response, await StreamToByteArray(stream));
            }

            return new ApiResponse<byte[]>(response, response.Body as byte[]);
        }

        /// <summary>
        /// Performs an asynchronous HTTP GET request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        async Task<IApiResponse<Stream>> GetRawStream(IRequest request)
        {
            var response = await RunRequest(request, CancellationToken.None).ConfigureAwait(false);
            
            return new ApiResponse<Stream>(response, response.Body as Stream);
        }

        /// <summary>
        /// Converts a stream to a byte array.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        async Task<byte[]> StreamToByteArray(Stream stream)
        {
            if (stream is MemoryStream memoryStream)
            {
                return memoryStream.ToArray();                
            }
            
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Performs an asynchronous HTTP request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="preprocessResponseBody"></param>
        /// <returns></returns>
        async Task<IApiResponse<T>> Run<T>(IRequest request, CancellationToken cancellationToken, Func<object, object> preprocessResponseBody = null)
        {
            _jsonPipeline.SerializeRequest(request);
            var response = await RunRequest(request, cancellationToken, preprocessResponseBody).ConfigureAwait(false);

            if (typeof(T) == typeof(string))
            {
                return new ApiResponse<T>(response, (T)(object)(response.Body as string));
            }

            return _jsonPipeline.DeserializeResponse<T>(response);
        }

        // THIS IS THE METHOD THAT EVERY REQUEST MUST GO THROUGH!
        async Task<IResponse> RunRequest(IRequest request, CancellationToken cancellationToken, Func<object, object> preprocessResponseBody = null)
        {
            var response = await _httpClient.Send(request, cancellationToken, preprocessResponseBody).ConfigureAwait(false);
            HandleErrors(response);
            return response;
        }

        /// <summary>
        /// Maps HTTP status codes to exceptions.
        /// </summary>
        static readonly Dictionary<HttpStatusCode, Func<IResponse, Exception>> _httpExceptionMap =
            new Dictionary<HttpStatusCode, Func<IResponse, Exception>>
            {
                { HttpStatusCode.Forbidden, GetExceptionForForbidden },
                { HttpStatusCode.NotFound, response => new NotFoundException(response) },
            };

        /// <summary>
        /// Handles errors that occur during the request.
        /// </summary>
        /// <param name="response"></param>
        /// <exception cref="ApiException"></exception>
        static void HandleErrors(IResponse response)
        {
            Func<IResponse, Exception> exceptionFunc;
            if (_httpExceptionMap.TryGetValue(response.StatusCode, out exceptionFunc))
            {
                throw exceptionFunc(response);
            }

            if ((int)response.StatusCode >= 400)
            {
                throw new ApiException(response);
            }
        }

        /// <summary>
        /// Gets the exception for a forbidden response.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        static Exception GetExceptionForForbidden(IResponse response)
        {
            return new ForbiddenException(response);
        }

        /// <summary>
        /// Sets the timeout for the connection between the client and the server.
        /// </summary>
        /// <param name="timeout">The Timeout value</param>
        public void SetRequestTimeout(TimeSpan timeout)
        {
            _httpClient.SetRequestTimeout(timeout);
        }

        /// <summary>
        /// Adds a custom HTTP header to the request.
        /// </summary>
        /// <param name="headerName"></param>
        /// <param name="headerValue"></param>
        public void AddHttpHeader(string headerName, string headerValue)
        {
            Ensure.ArgumentNotNull(headerName, nameof(headerName));
            Ensure.ArgumentNotNull(headerValue, nameof(headerValue));

            _httpClient.AddHttpHeader(headerName, headerValue);
        }

        /// <summary>
        /// Gets a custom HTTP header from the request.
        /// </summary>
        /// <param name="headerName"></param>
        /// <returns></returns>
        public string GetHttpHeader(string headerName)
        {
            return _httpClient.GetHttpHeader(headerName);
        }


        /// <summary>
        /// Remove a custom HTTP header.
        /// </summary>
        /// <param name="headerName"></param>
        /// <returns></returns>
        public void RemoveHttpHeader(string headerName)
        {
            _httpClient.RemoveHttpHeader(headerName);
        }
        

        /// <summary>
        /// Serializes the given object to a JSON string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Serialize(object value)
        {
            return _jsonPipeline.Serialize(value);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes the connection and releases any resources it holds.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }
        }
    }
}
