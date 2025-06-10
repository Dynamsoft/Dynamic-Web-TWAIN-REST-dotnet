using DynamicWebTWAIN.RestClient.Internal;
using System.IO;
using System.Xml.Linq;

namespace DynamicWebTWAIN.RestClient
{
    public class CreateDocumentPageOptions : RequestParameters
    {
        /// <summary>
        /// Unique identifier for the document.
        /// </summary>
        [Parameter(Key = "uid")]
        public string Uid { get; private set; }

        /// <summary>
        /// document password
        /// </summary>
        public string Password { get; set; } = null;


        /// <summary>
        /// the password of the uploaded document, e.g.pdf
        /// </summary>
        public string SourcePassword { get; set; } = null;


        public byte[] imageBytes { get; set; } = null;

    }
}
