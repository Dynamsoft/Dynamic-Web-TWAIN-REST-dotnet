using DynamicWebTWAIN.RestClient.Internal;
using System.Collections.Generic;

namespace DynamicWebTWAIN.RestClient
{
    public class Document : RequestParameters
    {
        /// <summary>
        /// Unique identifier for the document.
        /// </summary>
        [Parameter(Key = "uid")]
        public string Uid { get; private set; }

        public IReadOnlyList<DocumentPage> pages { get; private set; }
    }

}
