using System;
using System.Diagnostics.CodeAnalysis;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Class for retrieving GitHub API URLs
    /// </summary>
    public static partial class ApiUrls
    {
        static readonly Uri _version = new Uri("api/server/version", UriKind.Relative);
        static readonly Uri _settings = new Uri("api/server", UriKind.Relative);
        static readonly Uri _scanners = new Uri("api/device/scanners", UriKind.Relative);
        static readonly Uri _scannerJobs = new Uri("api/device/scanners/jobs", UriKind.Relative);
        static readonly Uri _docs = new Uri("api/storage/documents", UriKind.Relative);
        static readonly Uri _checkBlankImage = new Uri("api/process/check-blank", UriKind.Relative);
        static readonly Uri _readBarcode = new Uri("api/process/read-barcode", UriKind.Relative);
        
        public static Uri Version()
        {
            return _version;
        }

        public static Uri Settings()
        {
            return _settings;
        }

        public static Uri Scanners()
        {
            return _scanners;
        }

        public static Uri ScannerJobs()
        {
            return _scannerJobs;
        }

        public static Uri ScannerJob(string jobuid)
        {
            return "api/device/scanners/jobs/{0}".FormatUri(jobuid);
        }

        public static Uri ScannerCapability(string jobuid)
        {
            return "api/device/scanners/jobs/{0}/scanner/capabilities".FormatUri(jobuid);
        }

        public static Uri ScannerSettings(string jobuid)
        {
            return "api/device/scanners/jobs/{0}/scanner/settings".FormatUri(jobuid);
        }

        public static Uri ScannerJobPageInfo(string jobuid)
        {
            return "api/device/scanners/jobs/{0}/next-page-info".FormatUri(jobuid);
        }

        public static Uri ScannerJobPage(string jobuid)
        {
            return "api/device/scanners/jobs/{0}/next-page".FormatUri(jobuid);
        }

        public static Uri ScannerJobContent(string jobuid)
        {
            return "api/device/scanners/jobs/{0}/content".FormatUri(jobuid);
        }

        public static Uri Docs()
        {
            return _docs;
        }

        public static Uri Doc(string docuid)
        {
            return "api/storage/documents/{0}".FormatUri(docuid);
        }

        public static Uri DocContent(string docuid)
        {
            return "api/storage/documents/{0}/content".FormatUri(docuid);
        }

        public static Uri DocPage(string docuid)
        {
            return "api/storage/documents/{0}/pages".FormatUri(docuid);
        }

        public static Uri DocPage(string docuid, object page)
        {
            return "api/storage/documents/{0}/pages/{1}".FormatUri(docuid, page);
        }

        public static Uri CheckBlankImage()
        {
            return _checkBlankImage;
        }

        public static Uri ReadBarcode()
        {
            return _readBarcode;
        }
    }
}
