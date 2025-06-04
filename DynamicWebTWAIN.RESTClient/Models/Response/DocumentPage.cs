using DynamicWebTWAIN.RestClient;
using DynamicWebTWAIN.RestClient.Internal;

namespace DynamicWebTWAIN.RestClient
{
    public class DocumentPageMetaData : RequestParameters
    {
        /// <summary>
        /// Unique identifier for the document.
        /// </summary>
        [Parameter(Key = "uid")]
        public string Uid { get; private set; }


        /// <summary>
        /// </summary>
        [Parameter(Key = "mimeType")]
        public string MimeType { get; private set; }


        /// <summary>
        /// </summary>
        [Parameter(Key = "rotation")]
        public int Rotation { get; private set; }

        /// <summary>
        /// </summary>
        [Parameter(Key = "annotations")]
        public string Annotations { get; private set; }

        /// <summary>
        /// </summary>
        [Parameter(Key = "customData")]
        public string CustomData { get; private set; }

    }

    public class DocumentPage : RequestParameters
    {
        /// <summary>
        /// Unique identifier for the document.
        /// </summary>
        [Parameter(Key = "uid")]
        public string Uid { get; private set; }


        /// <summary>
        /// </summary>
        [Parameter(Key = "metaData")]
        public string DocumentPageMetaData { get; private set; }
    }

}
