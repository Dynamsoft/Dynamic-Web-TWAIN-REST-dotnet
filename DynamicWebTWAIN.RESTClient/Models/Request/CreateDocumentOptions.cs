using DynamicWebTWAIN.RestClient.Internal;

namespace DynamicWebTWAIN.RestClient
{
    public class CreateDocumentOptions : RequestParameters
    {
        /// <summary>
        /// document password
        /// </summary>
        [Parameter(Key = "password")]
        public string Password { get; set; } = null;

        /// <summary>
        /// document name
        [Parameter(Key = "name")]
        public string Name { get; set; } = null;

        /// <summary>
        /// document description
        /// </summary>
        [Parameter(Key = "description")]
        public string Description { get; set; } = null;

        /// <summary>
        /// only valid when authorization is enabled. private: only the creator can access the document. public: everyone can access the document.
        /// </summary>
        [Parameter(Key = "scope")]
        public string Scope { get; set; } = null;

    }
}
