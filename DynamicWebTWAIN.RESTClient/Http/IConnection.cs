﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicWebTWAIN.RestClient.Internal;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// A connection for making HTTP requests against URI endpoints.
    /// </summary>
    public interface IConnection : IDisposable
    {

        /// <summary>
        /// Performs an asynchronous HTTP GET request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="parameters">Querystring parameters for the request</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        /// <remarks>The <see cref="IResponse.Body"/> property will be <c>null</c> if the <paramref name="uri"/> points to a directory instead of a file</remarks>
        Task<IApiResponse<byte[]>> GetRaw(Uri uri, IDictionary<string, string> parameters);

        /// <summary>
        /// Performs an asynchronous HTTP GET request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="parameters">Querystring parameters for the request</param>
        /// <param name="timeout">The Timeout value</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        /// <remarks>The <see cref="IResponse.Body"/> property will be <c>null</c> if the <paramref name="uri"/> points to a directory instead of a file</remarks>
        Task<IApiResponse<byte[]>> GetRaw(Uri uri, IDictionary<string, string> parameters, TimeSpan timeout);

        /// <summary>
        /// Performs an asynchronous HTTP GET request that expects a <seealso cref="IResponse"/> containing raw data.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="parameters">Querystring parameters for the request</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        /// <remarks>The <see cref="IResponse.Body"/> property will be <c>null</c> if the <paramref name="uri"/> points to a directory instead of a file</remarks>
        Task<IApiResponse<Stream>> GetRawStream(Uri uri, IDictionary<string, string> parameters);

        /// <summary>
        /// Performs an asynchronous HTTP GET request.
        /// Attempts to map the response to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="parameters">Querystring parameters for the request</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters);

        /// <summary>
        /// Performs an asynchronous HTTP GET request.
        /// Attempts to map the response to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="parameters">Querystring parameters for the request</param>
        /// <param name="accepts">Specifies accepted response media types.</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
        Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters, string accepts);

        /// <summary>
        /// Performs an asynchronous HTTP GET request.
        /// Attempts to map the response to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="parameters">Querystring parameters for the request</param>
        /// <param name="accepts">Specifies accepted response media types.</param>
        /// <param name="cancellationToken">A token used to cancel the Get request</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
        Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters, string accepts, CancellationToken cancellationToken);

        /// <summary>
        /// Performs an asynchronous HTTP GET request.
        /// Attempts to map the response to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="parameters">Querystring parameters for the request</param>
        /// <param name="accepts">Specifies accepted response media types.</param>
        /// <param name="cancellationToken">A token used to cancel the Get request</param>
        /// <param name="preprocessResponseBody">Function to preprocess HTTP response prior to deserialization (can be null)</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
        Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters, string accepts, CancellationToken cancellationToken, Func<object, object> preprocessResponseBody);

        /// <summary>
        /// Performs an asynchronous HTTP GET request.
        /// Attempts to map the response to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="timeout">Expiration time of the request</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
        Task<IApiResponse<T>> Get<T>(Uri uri, TimeSpan timeout);

        /// <summary>
        /// Performs an asynchronous HTTP PATCH request.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<HttpStatusCode> Patch(Uri uri);

        /// <summary>
        /// Performs an asynchronous HTTP PATCH request.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<HttpStatusCode> Patch(Uri uri, object body);

        /// <summary>
        /// Performs an asynchronous HTTP PATCH request.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="accepts">Specifies accepted response media types.</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<HttpStatusCode> Patch(Uri uri, string accepts);

        /// <summary>
        /// Performs an asynchronous HTTP PATCH request.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accept response media type</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<HttpStatusCode> Patch(Uri uri, object body, string accepts);

        /// <summary>
        /// Performs an asynchronous HTTP PATCH request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<IApiResponse<T>> Patch<T>(Uri uri, object body);

        /// <summary>
        /// Performs an asynchronous HTTP PATCH request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accepted response media types.</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<IApiResponse<T>> Patch<T>(Uri uri, object body, string accepts);

        /// <summary>
        /// Performs an asynchronous HTTP POST request.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        Task<HttpStatusCode> Post(Uri uri, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs an asynchronous HTTP POST request.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accepted response media types.</param>
        /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        Task<HttpStatusCode> Post(Uri uri, object body, string accepts, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs an asynchronous HTTP POST request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<IApiResponse<T>> Post<T>(Uri uri, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs an asynchronous HTTP POST request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accepted response media types.</param>
        /// <param name="contentType">Specifies the media type of the request body</param>
        /// <param name="parameters">Extra parameters for authentication.</param>
        /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<IApiResponse<T>> Post<T>(
            Uri uri,
            object body,
            string accepts,
            string contentType,
            IDictionary<string, string> parameters = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs an asynchronous HTTP POST request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accepted response media types.</param>
        /// <param name="contentType">Specifies the media type of the request body</param>
        /// <param name="twoFactorAuthenticationCode">Two Factor Authentication Code</param>
        /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs an asynchronous HTTP POST request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accepted response media types.</param>
        /// <param name="contentType">Specifies the media type of the request body</param>
        /// <param name="timeout"></param>
        /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType, TimeSpan timeout, CancellationToken cancellationToken = default);


        /// <summary>
        /// Performs an asynchronous HTTP POST request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <remarks>
        /// We have one case where we need to override the BaseAddress. This overload is for that case.
        /// https://developer.github.com/v3/oauth/#web-application-flow
        /// </remarks>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accepted response media types.</param>
        /// <param name="contentType">Specifies the media type of the request body</param>
        /// <param name="baseAddress">Allows overriding the base address for a post.</param>
        /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType, Uri baseAddress, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs an asynchronous HTTP PUT request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The body of the request</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<IApiResponse<T>> Put<T>(Uri uri, object body);


        /// <summary>
        /// Performs an asynchronous HTTP PUT request using the provided two factor authentication code.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <param name="twoFactorAuthenticationCode">Two factory authentication code to use</param>
        /// <param name="accepts">Specifies accepted response media types.</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        Task<IApiResponse<T>> Put<T>(Uri uri, object body, string accepts);


        /// <summary>
        /// Performs an asynchronous HTTP PUT request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        Task<HttpStatusCode> Put(Uri uri);

        /// <summary>
        /// Performs an asynchronous HTTP PUT request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        Task<HttpStatusCode> Put(Uri uri, object body);

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        Task<HttpStatusCode> Delete(Uri uri);


        /// <summary>
        /// Performs an asynchronous HTTP DELETE request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        Task<HttpStatusCode> Delete(Uri uri, object data);

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accept response media type</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        Task<HttpStatusCode> Delete(Uri uri, object data, string accepts);

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request.
        /// </summary>
        /// <typeparam name="T">The API resource's type.</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        Task<IApiResponse<T>> Delete<T>(Uri uri, object data);

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accept response media type</param>
        Task<IApiResponse<T>> Delete<T>(Uri uri, object data, string accepts);

        /// <summary>
        /// Base address for the connection.
        /// </summary>
        Uri BaseAddress { get; }

        /// <summary>
        /// Sets the timeout for the connection between the client and the server.
        /// </summary>
        /// <param name="timeout">The Timeout value</param>
        void SetRequestTimeout(TimeSpan timeout);

        /// <summary>
        /// Adds a custom HTTP header to the request.
        /// </summary>
        /// <param name="headerName"></param>
        /// <param name="headerValue"></param>
        void AddHttpHeader(string headerName, string headerValue);

        /// <summary>
        /// Gets a custom HTTP header from the request.
        /// </summary>
        /// <param name="headerName"></param>
        /// <returns></returns>
        string GetHttpHeader(string headerName);

        /// <summary>
        /// Remove a custom HTTP header from the request.
        /// </summary>
        /// <param name="headerName"></param>
        /// <returns></returns>
        void RemoveHttpHeader(string headerName);

        /// <summary>
        /// Gets the HTTP headers from the request.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string Serialize(object value);
    }
}
