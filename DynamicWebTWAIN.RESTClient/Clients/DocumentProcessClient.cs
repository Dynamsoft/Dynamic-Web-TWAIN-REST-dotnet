using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Client for document processing operations.
    /// </summary>
    public class DocumentProcessClient : ApiClient, IDocumentProcessClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentProcessClient"/> class.
        /// </summary>
        /// <param name="apiConnection"></param>
        public DocumentProcessClient(IApiConnection apiConnection)
            : base(apiConnection)
        {
            Ensure.ArgumentNotNull(apiConnection, nameof(apiConnection));
        }

        /// <summary>
        /// Check if the image is blank.
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        [ManualRoute("POST", "/api/process/check-blank")]
        public Task<bool> IsBlankPage(string imageUrl)
        {
            Ensure.ArgumentNotNull(imageUrl, nameof(imageUrl));

            var settings = new CheckBlankSettings
            {
                MinBlockHeight = 20,
                MaxBlockHeight = 30,
            };

            return IsBlankPage(imageUrl, settings);
        }


        /// <summary>
        /// Check if the image is blank with custom settings.
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        [ManualRoute("POST", "/api/process/check-blank")]
        public async Task<bool> IsBlankPage(string imageUrl, CheckBlankSettings settings)
        {
            Ensure.ArgumentNotNull(imageUrl, nameof(imageUrl));
            Ensure.ArgumentNotNull(settings, nameof(settings));

            var request = new
            {
                source = imageUrl,
                settings = settings,
            };

            var response = await ApiConnection.Post<string>(ApiUrls.CheckBlankImage(), request);
            JsonObject obj = SimpleJson.DeserializeObject(response) as JsonObject;
            if (obj != null)
            {
                bool result = false;
                obj.TryGetValue("result", out object resultObj);
                if (resultObj != null)
                {
                    result = Convert.ToBoolean(resultObj);
                }
                return result;
            }

            // impossible run here
            return false;
        }


        /// <summary>
        /// Read barcode from the image.
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        [ManualRoute("POST", "/api/process/read-barcode")]
        public Task<string> ReadBarcode(string imageUrl)
        {
            Ensure.ArgumentNotNull(imageUrl, nameof(imageUrl));

            return ReadBarcode(imageUrl, "coverage");
        }

        /// <summary>
        /// Read barcode from the image with custom template.
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="templateNameOrContent"></param>
        /// <returns></returns>
        [ManualRoute("POST", "/api/process/read-barcode")]
        public Task<string> ReadBarcode(string imageUrl, string templateNameOrContent)
        {
            Ensure.ArgumentNotNull(imageUrl, nameof(imageUrl));
            Ensure.ArgumentNotNull(templateNameOrContent, nameof(templateNameOrContent));

            var request = new
            {
                source = imageUrl,
                settings = templateNameOrContent,
            };

            return ApiConnection.Post<string>(ApiUrls.ReadBarcode(), request);
        }


        /// <summary>
        /// Read barcode from the image bytes with custom template.
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <returns></returns>
        [ManualRoute("POST", "/api/process/read-barcode")]
        public Task<string> ReadBarcodeByArray(byte[] imageBytes) {
            return ReadBarcodeByArray(imageBytes, "coverage");
        }

        /// <summary>
        /// Read barcode from the image bytes with custom template.
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <param name="templateNameOrContent"></param>
        /// <returns></returns>
        [ManualRoute("POST", "/api/process/read-barcode")]
        public Task<string> ReadBarcodeByArray(byte[] imageBytes, string templateNameOrContent)
        {
            Ensure.ArgumentNotNull(imageBytes, nameof(imageBytes));
            Ensure.ArgumentNotNull(templateNameOrContent, nameof(templateNameOrContent));

            string imageType = null;
            string imageName = null;

            if (imageBytes.Length > 12) {
                if (imageBytes[0] == 0xFF && imageBytes[1] == 0xD8)
                {
                    // jpeg
                    imageType = "image/jpeg";
                    imageName = "DynamicTWAIN.jpg";
                }
                else if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50
                    && imageBytes[2] == 0x4E && imageBytes[3] == 0x47
                    && imageBytes[4] == 0x0D && imageBytes[5] == 0x0A
                    && imageBytes[6] == 0x1A && imageBytes[7] == 0x0A)
                {
                    // png
                    imageType = "image/png";
                    imageName = "DynamicTWAIN.png";
                }
            }

            if (imageType == null) {
                throw new InvalidOperationException("Invalid image type");
            }


            var requestBody = new MultipartFormDataContent
            {
                { new StringContent(templateNameOrContent), "\"settings\"" }
            };

            var stream = new ByteArrayContent(imageBytes);
            stream.Headers.Add("Content-Type", imageType);
            requestBody.Add(stream, "\"stream\"", "\"" + imageName + "\"");

            return ApiConnection.Post<string>(ApiUrls.ReadBarcode(), requestBody);
        }

    }
}

