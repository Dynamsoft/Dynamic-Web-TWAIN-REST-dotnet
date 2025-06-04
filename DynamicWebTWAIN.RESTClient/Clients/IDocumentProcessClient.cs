using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Interface for the document process client.
    /// </summary>
    public interface IDocumentProcessClient
    {
        /// <summary>
        /// Check if the image is blank.
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        Task<bool> IsBlankPage(string imageUrl);

        /// <summary>
        /// Check if the image is blank with custom settings.
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        Task<bool> IsBlankPage(string imageUrl, CheckBlankSettings settings);

        /// <summary>
        /// Read a barcode from the image URL.
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        Task<string> ReadBarcode(string imageUrl);

        /// <summary>
        /// Read a barcode from the image URL with a specific template.
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="templateNameOrContent"></param>
        /// <returns></returns>
        Task<string> ReadBarcode(string imageUrl, string templateNameOrContent);


        /// <summary>
        /// Read barcode from the image bytes with custom template.
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <returns></returns>
        Task<string> ReadBarcodeByArray(byte[] imageBytes);

        /// <summary>
        /// Read barcode from the image bytes with custom template.
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <param name="templateNameOrContent"></param>
        /// <returns></returns>
        Task<string> ReadBarcodeByArray(byte[] imageBytes, string templateNameOrContent);
    }
}
