﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// A connection for making API requests against URI endpoints.
    /// Provides type-friendly convenience methods that wrap <see cref="IConnection"/> methods.
    /// </summary>
    public interface IApiConnection
    {
        /// <summary>
        /// The underlying connection.
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// Gets the API resource at the specified URI.
        /// </summary>
        /// <typeparam name="T">Type of the API resource to get.</typeparam>
        /// <param name="uri">URI of the API resource to get</param>
        /// <returns>The API resource.</returns>
        /// <exception cref="ApiException">Thrown when an API error occurs.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
        Task<T> Get<T>(Uri uri);

        /// <summary>
        /// Gets the API resource at the specified URI.
        /// </summary>
        /// <typeparam name="T">Type of the API resource to get.</typeparam>
        /// <param name="uri">URI of the API resource to get</param>
        /// <param name="parameters">Parameters to add to the API request</param>
        /// <returns>The API resource.</returns>
        /// <exception cref="ApiException">Thrown when an API error occurs.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get",
            Justification = "It's fiiiine. It's fine. Trust us.")]
        Task<T> Get<T>(Uri uri, IDictionary<string, string> parameters);

        /// <summary>
        /// Gets the API resource at the specified URI.
        /// </summary>
        /// <typeparam name="T">Type of the API resource to get.</typeparam>
        /// <param name="uri">URI of the API resource to get</param>
        /// <param name="parameters">Parameters to add to the API request</param>
        /// <param name="accepts">Accept header to use for the API request</param>
        /// <returns>The API resource.</returns>
        /// <exception cref="ApiException">Thrown when an API error occurs.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get",
            Justification = "It's fiiiine. It's fine. Trust us.")]
        Task<T> Get<T>(Uri uri, IDictionary<string, string> parameters, string accepts);

        /// <summary>
        /// Gets the raw content of the API resource at the specified URI.
        /// </summary>
        /// <param name="uri">URI of the API resource to get</param>
        /// <param name="parameters">Parameters to add to the API request</param>
        /// <returns>The API resource's raw content or <c>null</c> if the <paramref name="uri"/> points to a directory.</returns>
        /// <exception cref="ApiException">Thrown when an API error occurs.</exception>
        Task<byte[]> GetRaw(Uri uri, IDictionary<string, string> parameters);

        /// <summary>
        /// Gets the raw stream of the API resource at the specified URI.
        /// </summary>
        /// <param name="uri">URI of the API resource to get</param>
        /// <param name="parameters">Parameters to add to the API request</param>
        /// <returns>The API resource's raw stream or <c>null</c> if the <paramref name="uri"/> points to a directory.</returns>
        /// <exception cref="ApiException">Thrown when an API error occurs.</exception>
        Task<Stream> GetRawStream(Uri uri, IDictionary<string, string> parameters);

        /// <summary>
        /// Creates a new API resource in the list at the specified URI.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
        /// <returns><seealso cref="HttpStatusCode"/>Representing the received HTTP response</returns>
        /// <exception cref="ApiException">Thrown when an API error occurs.</exception>
        Task Post(Uri uri, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new API resource in the list at the specified URI.
        /// </summary>
        /// <typeparam name="T">The API resource's type.</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
        /// <returns>The created API resource.</returns>
        /// <exception cref="ApiException">Thrown when an API error occurs.</exception>
        Task<T> Post<T>(Uri uri, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new API resource in the list at the specified URI.
        /// </summary>
        /// <typeparam name="T">The API resource's type.</typeparam>
        /// <param name="uri">URI of the API resource to get</param>
        /// <param name="data">Object that describes the new API resource; this will be serialized and used as the request's body</param>
        /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
        /// <returns>The created API resource.</returns>
        /// <exception cref="ApiException">Thrown when an API error occurs.</exception>
        Task<T> Post<T>(Uri uri, object data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new API resource in the list at the specified URI.
        /// </summary>
        /// <typeparam name="T">The API resource's type.</typeparam>
        /// <param name="uri">URI of the API resource to get</param>
        /// <param name="data">Object that describes the new API resource; this will be serialized and used as the request's body</param>
        /// <param name="accepts">Accept header to use for the API request</param>
        /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
        /// <returns>The created API resource.</returns>
        /// <exception cref="ApiException">Thrown when an API error occurs.</exception>
        Task<T> Post<T>(Uri uri, object data, string accepts, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new API resource in the list at the specified URI.
        /// </summary>
        /// <typeparam name="T">The API resource's type.</typeparam>
        /// <param name="uri">URI of the API resource to get</param>
        /// <param name="data">Object that describes the new API resource; this will be serialized and used as the request's body</param>
        /// <param name="accepts">Accept header to use for the API request</param>
        /// <param name="contentType">Content type of the API request</param>
        /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
        /// <returns>The created API resource.</returns>
        /// <exception cref="ApiException">Thrown when an API error occurs.</exception>
        Task<T> Post<T>(Uri uri, object data, string accepts, string contentType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new API resource in the list at the specified URI.
        /// </summary>
        /// <typeparam name="T">The API resource's type.</typeparam>
        /// <param name="uri">URI of the API resource to get</param>
        /// <param name="data">Object that describes the new API resource; this will be serialized and used as the request's body</param>
        /// <param name="accepts">Accept header to use for the API request</param>
        /// <param name="contentType">Content type of the API request</param>
        /// <param name="timeout">Timeout for the request</param>
        /// <param name="cancellationToken">An optional token to monitor for cancellation requests</param>
        /// <returns>The created API resource.</returns>
        /// <exception cref="ApiException">Thrown when an API error occurs.</exception>
        Task<T> Post<T>(Uri uri, object data, string accepts, string contentType, TimeSpan timeout, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates or replaces the API resource at the specified URI
        /// </summary>
        /// <param name="uri">URI of the API resource to put</param>
        /// <returns>A <see cref="Task"/> for the request's execution.</returns>
        Task Put(Uri uri);

        /// <summary>
        /// Creates or replaces the API resource at the specified URI
        /// </summary>
        /// <param name="uri">URI of the API resource to put</param>
        /// <param name="data">Object that describes the API resource; this will be serialized and used as the request's body</param>
        /// <returns>A <see cref="Task"/> for the request's execution.</returns>
        Task Put(Uri uri, object data);

        /// <summary>
        /// Creates or replaces the API resource at the specified URI.
        /// </summary>
        /// <typeparam name="T">The API resource's type.</typeparam>
        /// <param name="uri">URI of the API resource to create or replace</param>
        /// <param name="data">Object that describes the API resource; this will be serialized and used as the request's body</param>
        /// <returns>The created API resource.</returns>
        /// <exception cref="ApiException">Thrown when an API error occurs.</exception>
        Task<T> Put<T>(Uri uri, object data);

        /// <summary>
        /// Creates or replaces the API resource at the specified URI.
        /// </summary>
        /// <typeparam name="T">The API resource's type.</typeparam>
        /// <param name="uri">URI of the API resource to create or replace</param>
        /// <param name="data">Object that describes the API resource; this will be serialized and used as the request's body</param>
        /// <param name="accepts">Accept header to use for the API request</param>
        /// <returns>The created API resource.</returns>
        /// <exception cref="ApiException">Thrown when an API error occurs.</exception>
        Task<T> Put<T>(Uri uri, object data, string accepts);


        /// <summary>
        /// Updates the API resource at the specified URI.
        /// </summary>
        /// <param name="uri">URI of the API resource to patch</param>
        /// <returns>A <see cref="Task"/> for the request's execution.</returns>
        Task Patch(Uri uri);

        /// <summary>
        /// Updates the API resource at the specified URI.
        /// </summary>
        /// <param name="uri">URI of the API resource to patch</param>
        /// <param name="data">Object that describes the API resource; this will be serialized and used as the request's body</param>
        /// <returns>A <see cref="Task"/> for the request's execution.</returns>
        Task Patch(Uri uri, object data);

        /// <summary>
        /// Updates the API resource at the specified URI.
        /// </summary>
        /// <param name="uri">URI of the API resource to patch</param>
        /// <param name="accepts">Accept header to use for the API request</param>
        /// <returns>A <see cref="Task"/> for the request's execution.</returns>
        Task Patch(Uri uri, string accepts);

        /// <summary>
        /// Updates the API resource at the specified URI.
        /// </summary>
        /// <param name="uri">URI of the API resource to patch</param>
        /// <param name="data">Object that describes the API resource; this will be serialized and used as the request's body</param>
        /// <param name="accepts">Accept header to use for the API request</param>
        /// <returns>A <see cref="Task"/> for the request's execution.</returns>
        Task Patch(Uri uri, object data, string accepts);

        /// <summary>
        /// Updates the API resource at the specified URI.
        /// </summary>
        /// <typeparam name="T">The API resource's type.</typeparam>
        /// <param name="uri">URI of the API resource to update</param>
        /// <param name="data">Object that describes the API resource; this will be serialized and used as the request's body</param>
        /// <returns>The updated API resource.</returns>
        /// <exception cref="ApiException">Thrown when an API error occurs.</exception>
        Task<T> Patch<T>(Uri uri, object data);

        /// <summary>
        /// Updates the API resource at the specified URI.
        /// </summary>
        /// <typeparam name="T">The API resource's type.</typeparam>
        /// <param name="uri">URI of the API resource to update</param>
        /// <param name="data">Object that describes the API resource; this will be serialized and used as the request's body</param>
        /// <param name="accepts">Accept header to use for the API request</param>
        /// <returns>The updated API resource.</returns>
        /// <exception cref="ApiException">Thrown when an API error occurs.</exception>
        Task<T> Patch<T>(Uri uri, object data, string accepts);

        /// <summary>
        /// Deletes the API object at the specified URI.
        /// </summary>
        /// <param name="uri">URI of the API resource to delete</param>
        /// <returns>A <see cref="Task"/> for the request's execution.</returns>
        Task Delete(Uri uri);

        /// <summary>
        /// Deletes the API object at the specified URI.
        /// </summary>
        /// <param name="uri">URI of the API resource to delete</param>
        /// <param name="data">Object that describes the API resource; this will be serialized and used as the request's body</param>
        /// <returns>A <see cref="Task"/> for the request's execution.</returns>
        Task Delete(Uri uri, object data);

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accept response media type</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        Task Delete(Uri uri, object data, string accepts);

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request.
        /// </summary>
        /// <typeparam name="T">The API resource's type.</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        Task<T> Delete<T>(Uri uri, object data);

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The API resource's type.</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="accepts">Specifies accept response media type</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        Task<T> Delete<T>(Uri uri, string accepts);

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The API resource's type.</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accept response media type</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        Task<T> Delete<T>(Uri uri, object data, string accepts);
    }
}
