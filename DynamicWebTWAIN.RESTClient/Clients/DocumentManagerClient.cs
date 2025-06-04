using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Interface for the scanner manager client.
    /// </summary>
    public class DocumentManagerClient : ApiClient, IDocumentManagerClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentManagerClient"/> class.
        /// </summary>
        /// <param name="apiConnection"></param>
        public DocumentManagerClient(IApiConnection apiConnection) : base(apiConnection)
        {
            Ensure.ArgumentNotNull(apiConnection, nameof(apiConnection));
        }

        /// <summary>
        /// Create a new document
        /// </summary>
        /// <param name="createDocumentOptions"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        [ManualRoute("POST", "/api/storage/documents")]
        public Task<Document> CreateDocument(CreateDocumentOptions createDocumentOptions)
        {
            Ensure.ArgumentNotNull(createDocumentOptions, nameof(createDocumentOptions));
            return ApiConnection.Post<Document>(ApiUrls.Docs(), createDocumentOptions);
        }


        /// <summary>
        /// retrive document info.
        /// </summary>
        /// <param name="documentuid"></param>
        /// <returns></returns>
        [ManualRoute("GET", "/api/storage/documents/{documentuid}")]
        public Task<Document> GetDocument(string documentuid)
        {
            Ensure.ArgumentNotNull(documentuid, nameof(documentuid));
            return GetDocument(documentuid, null);
        }

        /// <summary>
        /// retrive document info.
        /// </summary>
        /// <param name="documentuid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [ManualRoute("GET", "/api/storage/documents/{documentuid}")]
        public async Task<Document> GetDocument(string documentuid, string password)
        {
            Ensure.ArgumentNotNull(documentuid, nameof(documentuid));

            if (!String.IsNullOrEmpty(password))
            {
                ApiConnection.Connection.AddHttpHeader(HttpHeaderName.DOC_PASSWORD, password);
            }

            var query = new Dictionary<string, string>
            {
                { "documentuid", documentuid }
            };

            var ret = await ApiConnection.Get<Document>(ApiUrls.Docs(), query);
            
            ApiConnection.Connection.RemoveHttpHeader(HttpHeaderName.DOC_PASSWORD);

            return ret;
        }


        /// <summary>
        /// delete the document.
        /// </summary>
        /// <param name="documentuid"></param>
        /// <returns></returns>
        [ManualRoute("DELETE", "/api/storage/documents/{documentuid}")]
        public Task DeleteDocument(string documentuid)
        {
            Ensure.ArgumentNotNull(documentuid, nameof(documentuid));
            return DeleteDocument(documentuid, null);
        }

        /// <summary>
        /// delete the document.
        /// </summary>
        /// <param name="documentuid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [ManualRoute("DELETE", "/api/storage/documents/{documentuid}")]
        public async Task DeleteDocument(string documentuid, string password)
        {
            Ensure.ArgumentNotNull(documentuid, nameof(documentuid));

            if (!String.IsNullOrEmpty(password))
            {
                ApiConnection.Connection.AddHttpHeader(HttpHeaderName.DOC_PASSWORD, password);
            }

            var query = new Dictionary<string, string>
            {
                { "documentuid", documentuid }
            };

            await ApiConnection.Delete(ApiUrls.Docs(), query);

            ApiConnection.Connection.RemoveHttpHeader(HttpHeaderName.DOC_PASSWORD);
        }

    }
}
