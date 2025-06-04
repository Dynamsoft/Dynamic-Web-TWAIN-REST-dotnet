using DynamicWebTWAIN.RestClient.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicWebTWAIN.RestClient
{
    public class ScannerConfiguration : RequestParameters
    {
        [Parameter(Key = "IfShowUI")]
        public bool? IfShowUI { get; set; } = false;

        // Whether to scan in color, grey or black & white
        [Parameter(Key = "PixelType")]
        public EnumDWT_PixelType? PixelType { get; set; }

        // Measured by dots per pixel (DPI)
        [Parameter(Key = "Resolution")]
        public int? Resolution { get; set; }

        // Whether to use the document feeder or the flatbed of the device
        [Parameter(Key = "IfFeederEnabled")]
        public bool? IfFeederEnabled { get; set; }

        // Whether to scan one side or both sides
        [Parameter(Key = "IfDuplexEnabled")]
        public bool? IfDuplexEnabled { get; set; }

        // Number of pages to acquire. Default: 1.
        [Parameter(Key = "XferCount")]
        public int? XferCount { get; set; } 

        // Whether to close the built-in User Interface after acquisition. Only valid when {IfShowUI} is true.
        [Parameter(Key = "IfDisableSourceAfterAcquire")]
        public bool? IfDisableSourceAfterAcquire { get; set; }

        // Whether to retrieve information about the image after it's transferred.
        [Parameter(Key = "IfGetImageInfo")]
        public bool? IfGetImageInfo { get; set; }

        // Whether to retrieve extended information about the image after it's transferred.
        [Parameter(Key = "IfGetExtImageInfo")]
        public bool? IfGetExtImageInfo { get; set; }

        // How much extended information is retrieved. Only valid when {IfGetExtImageInfo} is true.
        [Parameter(Key = "ExtendedImageInfoQueryLevel")]
        public EnumDWT_ExtImageInfo? ExtendedImageInfoQueryLevel { get; set; }

        // Whether to close the data source after acquisition. Default: false.
        [Parameter(Key = "IfCloseSourceAfterAcquire")]
        public bool? IfCloseSourceAfterAcquire { get; set; }

        // Specify page size
        [Parameter(Key = "PageSize")]
        public EnumDWT_CapSupportedSizes? PageSize { get; set; }
    }

    public enum EnumDWT_PixelType : int
    {
        [Parameter(Value = "0")]
        TWPT_BW = 0,
        [Parameter(Value = "1")]
        TWPT_GRAY = 1,
        [Parameter(Value = "2")]
        TWPT_RGB = 2,
        [Parameter(Value = "3")]
        TWPT_PALLETE = 3,
        [Parameter(Value = "4")]
        TWPT_CMY = 4,
        [Parameter(Value = "5")]
        TWPT_CMYK = 5,
        [Parameter(Value = "6")]
        TWPT_YUV = 6,
        [Parameter(Value = "7")]
        TWPT_YUVK = 7,
        [Parameter(Value = "8")]
        TWPT_CIEXYZ = 8,
        [Parameter(Value = "9")]
        TWPT_LAB = 9,
        [Parameter(Value = "10")]
        TWPT_SRGB = 10,
        [Parameter(Value = "11")]
        TWPT_SCRGB = 11,
        [Parameter(Value = "16")]
        TWPT_INFRARED = 16
    }


    public enum EnumDWT_ExtImageInfo : int
    {
        [Parameter(Value = "0")]
        Default = 0,
        [Parameter(Value = "1")]
        Standard = 1,
        [Parameter(Value = "2")]
        Supported = 2
    }

    public enum EnumDWT_CapSupportedSizes : int
    {
        [Parameter(Value = "0")]
        TWSS_NONE = 0,
        [Parameter(Value = "1")]
        TWSS_A4LETTER = 1,
        [Parameter(Value = "2")]
        TWSS_B5LETTER = 2,
        [Parameter(Value = "3")]
        TWSS_USLETTER = 3,
        [Parameter(Value = "4")]
        TWSS_USLEGAL = 4,
        [Parameter(Value = "5")]
        TWSS_A5 = 5,
        [Parameter(Value = "6")]
        TWSS_B4 = 6,
        [Parameter(Value = "7")]
        TWSS_B6 = 7,
        [Parameter(Value = "9")]
        TWSS_USLEDGER = 9,
        [Parameter(Value = "10")]
        TWSS_USEXECUTIVE = 10,
        [Parameter(Value = "11")]
        TWSS_A3 = 11,
        [Parameter(Value = "12")]
        TWSS_B3 = 12,
        [Parameter(Value = "13")]
        TWSS_A6 = 13,
        [Parameter(Value = "14")]
        TWSS_C4 = 14,
        [Parameter(Value = "15")]
        TWSS_C5 = 15,
        [Parameter(Value = "16")]
        TWSS_C6 = 16,
        [Parameter(Value = "17")]
        TWSS_4A0 = 17,
        [Parameter(Value = "18")]
        TWSS_2A0 = 18,
        [Parameter(Value = "19")]
        TWSS_A0 = 19,
        [Parameter(Value = "20")]
        TWSS_A1 = 20,
        [Parameter(Value = "21")]
        TWSS_A2 = 21,
        [Parameter(Value = "1")]
        TWSS_A4 = 1, // Duplicate value
        [Parameter(Value = "22")]
        TWSS_A7 = 22,
        [Parameter(Value = "23")]
        TWSS_A8 = 23,
        [Parameter(Value = "24")]
        TWSS_A9 = 24,
        [Parameter(Value = "25")]
        TWSS_A10 = 25,
        [Parameter(Value = "26")]
        TWSS_ISOB0 = 26,
        [Parameter(Value = "27")]
        TWSS_ISOB1 = 27,
        [Parameter(Value = "28")]
        TWSS_ISOB2 = 28,
        [Parameter(Value = "12")]
        TWSS_ISOB3 = 12, // Duplicate value
        [Parameter(Value = "6")]
        TWSS_ISOB4 = 6,  // Duplicate value
        [Parameter(Value = "29")]
        TWSS_ISOB5 = 29,
        [Parameter(Value = "7")]
        TWSS_ISOB6 = 7,  // Duplicate value
        [Parameter(Value = "30")]
        TWSS_ISOB7 = 30,
        [Parameter(Value = "31")]
        TWSS_ISOB8 = 31,
        [Parameter(Value = "32")]
        TWSS_ISOB9 = 32,
        [Parameter(Value = "33")]
        TWSS_ISOB10 = 33,
        [Parameter(Value = "34")]
        TWSS_JISB0 = 34,
        [Parameter(Value = "35")]
        TWSS_JISB1 = 35,
        [Parameter(Value = "36")]
        TWSS_JISB2 = 36,
        [Parameter(Value = "37")]
        TWSS_JISB3 = 37,
        [Parameter(Value = "38")]
        TWSS_JISB4 = 38,
        [Parameter(Value = "2")]
        TWSS_JISB5 = 2,  // Duplicate value
        [Parameter(Value = "39")]
        TWSS_JISB6 = 39,
        [Parameter(Value = "40")]
        TWSS_JISB7 = 40,
        [Parameter(Value = "41")]
        TWSS_JISB8 = 41,
        [Parameter(Value = "42")]
        TWSS_JISB9 = 42,
        [Parameter(Value = "43")]
        TWSS_JISB10 = 43,
        [Parameter(Value = "44")]
        TWSS_C0 = 44,
        [Parameter(Value = "45")]
        TWSS_C1 = 45,
        [Parameter(Value = "46")]
        TWSS_C2 = 46,
        [Parameter(Value = "47")]
        TWSS_C3 = 47,
        [Parameter(Value = "48")]
        TWSS_C7 = 48,
        [Parameter(Value = "49")]
        TWSS_C8 = 49,
        [Parameter(Value = "50")]
        TWSS_C9 = 50,
        [Parameter(Value = "51")]
        TWSS_C10 = 51,
        [Parameter(Value = "52")]
        TWSS_USSTATEMENT = 52,
        [Parameter(Value = "53")]
        TWSS_BUSINESSCARD = 53,
        [Parameter(Value = "54")]
        TWSS_MAXSIZE = 54
    }

}
