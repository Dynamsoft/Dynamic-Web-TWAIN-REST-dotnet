using System;
using System.Collections.Generic;
using System.Net.Http;

namespace DynamicWebTWAIN.RestClient.Internal
{
    /// <summary>
    /// Interface for the request object.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Gets or sets the request body.
        /// </summary>
        object Body { get; set; }

        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        Dictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets or sets the request method.
        /// </summary>
        HttpMethod Method { get; }

        /// <summary>
        /// Gets or sets the request parameters.
        /// </summary>
        Dictionary<string, string> Parameters { get; }

        /// <summary>
        /// Gets or sets the request query.
        /// </summary>
        Uri BaseAddress { get; }

        /// <summary>
        /// Gets or sets the request endpoint.
        /// </summary>
        Uri Endpoint { get; }

        /// <summary>
        /// Gets or sets the request timeout.
        /// </summary>
        TimeSpan Timeout { get; }

        /// <summary>
        /// Gets or sets the request content type.
        /// </summary>
        string ContentType { get; }
    }
}
