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
        /// Add image to document
        /// </summary>
        /// <param name="createDocumentOptions"></param>
        /// <returns></returns>
        Task<Document> AddImageToDocument(string documentuid, string strData);
       
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

        /// <summary>
        /// retrive document content in PDF format as blob.
        /// </summary>
        /// <param name="documentuid"></param>
        /// <returns>PDF blob data</returns>
        Task<byte[]> SaveDocumentAsPDF(string documentuid);

        /// <summary>
        /// retrive document content in PDF format as blob.
        /// </summary>
        /// <param name="documentuid"></param>
        /// <param name="password"></param>
        /// <returns>PDF blob data</returns>
        Task<byte[]> SaveDocumentAsPDF(string documentuid, string password);

    }
}
