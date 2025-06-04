using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace DynamicWebTWAIN.RestClient.Internal
{
    /// <summary>
    /// Represents a generic HTTP response
    /// </summary>
    internal class Response : IResponse
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Response"/> class.
        /// </summary>
        [Obsolete("Use the constructor with maximum parameters to avoid shortcuts")]
        public Response() : this(new Dictionary<string, string>())
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="headers"></param>
        [Obsolete("Use the constructor with maximum parameters to avoid shortcuts")]
        public Response(IDictionary<string, string> headers)
        {
            Ensure.ArgumentNotNull(headers, nameof(headers));

            Headers = new ReadOnlyDictionary<string, string>(headers);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="body"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        public Response(HttpStatusCode statusCode, object body, IDictionary<string, string> headers, string contentType)
        {
            Ensure.ArgumentNotNull(headers, nameof(headers));

            StatusCode = statusCode;
            Body = body;
            Headers = new ReadOnlyDictionary<string, string>(headers);
            ContentType = contentType;
        }

        /// <inheritdoc />
        public object Body { get; private set; }
        /// <summary>
        /// Information about the API.
        /// </summary>
        public IReadOnlyDictionary<string, string> Headers { get; private set; }
        /// <summary>

        /// <summary>
        /// The response status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }
        /// <summary>
        /// The content type of the response.
        /// </summary>
        public string ContentType { get; private set; }
    }
}
