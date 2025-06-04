using DynamicWebTWAIN.RestClient.Internal;

namespace DynamicWebTWAIN.RestClient
{
    public class DocumentOutput : RequestParameters
    {
        /// <summary>
        /// Output mime type.
        /// Default value: application/pdf
        /// </summary>
        [Parameter(Key = "type")]
        public StringEnum<OutputFormat> Format { get; set; }

        /// <summary>
        /// Compression quality (0 to 100). Higher is more compression.
        /// Only valid for PDF jpeg/jpeg2000 compression method.
        /// Default value: 80
        /// </summary>
        [Parameter(Key = "quality")]
        public int? Quality { get; set; }

        /// <summary>
        /// Comma-separated page identifiers (indices or UIDs).
        /// Default value: '' (all pages for image/tiff or application/pdf, first page for image/png or image/jpeg).
        /// </summary>
        [Parameter(Key = "pages")]
        public string Pages { get; set; } = null;

        /// <summary>
        /// Author name.
        /// Default value: ''
        /// </summary>
        [Parameter(Key = "author")]
        public string Author { get; set; } = null;

        /// <summary>
        /// Compression type for PDF and TIFF.
        /// Default value: 0 (auto-compression).
        /// </summary>
        [Parameter(Key = "compression")]
        public int? Compression { get; set; }

        /// <summary>
        /// Standard dimensions for the page.
        /// Default value: 0 (Page_Default).
        /// </summary>
        [Parameter(Key = "pageType")]
        public int? PageType { get; set; }

        /// <summary>
        /// Creator name.
        /// Default value: ''
        /// </summary>
        [Parameter(Key = "creator")]
        public string Creator { get; set; } = null;

        /// <summary>
        /// Creation date.
        /// Default value: ''
        /// </summary>
        [Parameter(Key = "creationDate")]
        public string CreationDate { get; set; } = null;

        /// <summary>
        /// Key words.
        /// Default value: ''
        /// </summary>
        [Parameter(Key = "keyWords")]
        public string KeyWords { get; set; } = null;

        /// <summary>
        /// Modification date.
        /// Default value: ''
        /// </summary>
        [Parameter(Key = "modifiedDate")]
        public string ModifiedDate { get; set; } = null;

        /// <summary>
        /// Producer name.
        /// Default value: ''
        /// </summary>
        [Parameter(Key = "producer")]
        public string Producer { get; set; } = null;

        /// <summary>
        /// Subject.
        /// Default value: ''
        /// </summary>
        [Parameter(Key = "subject")]
        public string Subject { get; set; } = null;

        /// <summary>
        /// Title.
        /// Default value: ''
        /// </summary>
        [Parameter(Key = "title")]
        public string Title { get; set; } = null;

        /// <summary>
        /// Version.
        /// Default value: '1.5'
        /// </summary>
        [Parameter(Key = "version")]
        public string Version { get; set; } = null;

        /// <summary>
        /// PDF file encryption password.
        /// Default value: '' (file is unencrypted by default).
        /// </summary>
        [Parameter(Key = "password")]
        public string Password { get; set; } = null;

        /// <summary>
        /// Beta version.
        /// </summary>
        [Parameter(Key = "rotation")]
        public float? Rotation { get; set; }

        /// <summary>
        /// Beta version.
        /// </summary>
        [Parameter(Key = "scaleFactor")]
        public float? ScaleFactor { get; set; }

        /// <summary>
        /// Beta version.
        /// </summary>
        [Parameter(Key = "width")]
        public int? Width { get; set; }

        /// <summary>
        /// Beta version.
        /// </summary>
        [Parameter(Key = "height")]
        public int? Height { get; set; }

    }

}
