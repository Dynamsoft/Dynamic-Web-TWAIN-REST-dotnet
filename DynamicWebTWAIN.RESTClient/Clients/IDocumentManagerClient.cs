using System.Collections.Generic;
using System.Threading.Tasks;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Interface for the scanner manager client.
    /// </summary>
    public interface IDocumentManagerClient
    {
        /// <summary>
        /// Create a new document
        /// </summary>
        /// <param name="createDocumentOptions"></param>
        /// <returns></returns>
        Task<Document> CreateDocument(CreateDocumentOptions createDocumentOptions);


        /// <summary>
        /// retrive document info.
        /// </summary>
        /// <param name="documentuid"></param>
        /// <returns></returns>
        Task<Document> GetDocument(string documentuid);


        /// <summary>
        /// delete the document.
        /// </summary>
        /// <param name="documentuid"></param>
        /// <returns></returns>
        Task DeleteDocument(string documentuid);

        /// <summary>
        /// delete the document.
        /// </summary>
        /// <param name="documentuid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task DeleteDocument(string documentuid, string password);

    }
}
