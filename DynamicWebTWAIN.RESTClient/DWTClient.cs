using DynamicWebTWAIN.RestClient;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;

namespace DynamicWebTWAIN.RestClient
{
    public class DWTClient
    {
        /// <summary>
        /// The base address for the DWT API
        /// </summary>
        public static readonly Uri DWTApiUrl = new Uri("https://127.0.0.1:18623/");

        /// <summary>
        /// Create a new instance of the DWT API client pointing to
        /// https://127.0.0.1:18623
        /// </summary>
        public DWTClient(string productKey)
            : this(DWTApiUrl, productKey)
        {
        }

        public DWTClient(Uri baseAddress, string productKey)
        {
            Ensure.ArgumentNotNull(productKey, nameof(productKey));

            Connection = new Connection(baseAddress);
            
            Connection.AddHttpHeader(HttpHeaderName.LICENSE_KEY, productKey);

            var apiConnection = new ApiConnection(Connection);
            ServerControlClient = new ServerControlClient(apiConnection);
            ScannerControlClient = new ScannerControlClient(apiConnection);
            DocumentProcessClient = new DocumentProcessClient(apiConnection);
            DocumentManagerClient = new DocumentManagerClient(apiConnection);
        }

        /// <summary>
        /// Sets the timeout for the connection between the client and the server.
        /// Useful to set a specific timeout for lengthy operations, such as uploading release assets
        /// </summary>
        /// <remarks>
        /// See more information here: https://technet.microsoft.com/library/system.net.http.httpclient.timeout(v=vs.110).aspx
        /// </remarks>
        /// <param name="timeout">The Timeout value</param>
        public void SetRequestTimeout(TimeSpan timeout)
        {
            Connection.SetRequestTimeout(timeout);
        }

        /// <summary>
        /// Adds a custom HTTP header to the request.
        /// </summary>
        /// <param name="headerName"></param>
        /// <param name="headerValue"></param>
        public void AddHttpHeader(string headerName, string headerValue)
        {
            Connection.AddHttpHeader(headerName, headerValue);
        }

        /// <summary>
        /// The base address of the DWT API.
        /// </summary>
        public Uri BaseAddress
        {
            get { return Connection.BaseAddress; }
        }

        /// <summary>
        /// Provides a client connection to make rest requests to HTTP endpoints.
        /// </summary>
        public IConnection Connection { get; private set; }

        /// <summary>
        /// Provides server-related APIs.
        /// </summary>
        public IServerControlClient ServerControlClient {get; private set; }

        /// <summary>
        /// Provides scanning-related APIs.
        /// </summary>
        public IScannerControlClient ScannerControlClient { get; private set; }

        /// <summary>
        /// Provides document processing-related APIs.
        /// </summary>
        public IDocumentProcessClient DocumentProcessClient { get; private set; }

        /// <summary>
        /// Provides document-related APIs.
        /// </summary>
        public IDocumentManagerClient DocumentManagerClient { get; private set; }

        /// <summary>
        /// Disposes the connection to the DWT API.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes the connection to the DWT API.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Connection?.Dispose();
            }
        }
    }
}
