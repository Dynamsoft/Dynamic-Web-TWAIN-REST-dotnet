using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Interface for the scanner job client.
    /// </summary>
    public interface IScannerJobClient
    {
        /// <summary>
        /// Starts the scanner job if it is in a pending state.
        /// </summary>
        /// <returns></returns>
        Task<StringEnum<JobStatus>> StartJob();

        /// <summary>
        /// Updates the status of the scanner job.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<StringEnum<JobStatus>> UpdateJobStatus(JobStatus status);

        /// <summary>
        /// Gets the status of the scanner job.
        /// </summary>
        /// <returns></returns>
        Task<ScannerJobStatus> GetJobStatus();

        /// <summary>
        /// Make sure to call this method to delete the job and close the websocket connection
        /// </summary>
        /// <returns></returns>
        Task DeleteJob();

        /// <summary>
        /// Gets the next image from the scanner job.
        /// </summary>
        /// <returns></returns>
        Task<byte[]> GetNextImage();

        /// <summary>
        /// Gets the next image from the scanner job in the specified format.
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        Task<byte[]> GetNextImage(StringEnum<OutputFormat> Format);

        /// <summary>
        /// Gets the image by URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<byte[]> GetImageByUrl(string url);

        /// <summary>
        /// Gets the document URL for the specified output.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        Uri GetDocumentUrl(DocumentOutput output);

        /// <summary>
        /// Gets the document in the specified format.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        Task<byte[]> GetDocument(DocumentOutput output);

        /// <summary>
        /// Gets the number of pages scanned.
        /// </summary>
        /// <returns></returns>
        int GetPageCount();

        /// <summary>
        /// Event triggered when a page is scanned.
        /// </summary>
        event ScannerJobClient.PageScannedEventHandler PageScanned;

        /// <summary>
        /// Event triggered when the transfer of all pages is ended.
        /// </summary>
        event EventHandler TransferEnded;


        /// <summary>
        /// The job associated with this client.
        /// </summary>
        ScannerJob ScannerJob { get; }

        /// <summary>
        /// The API connection used for communication with the server.
        /// </summary>
        bool HasWebSocketConnection {get;}
    }
}
