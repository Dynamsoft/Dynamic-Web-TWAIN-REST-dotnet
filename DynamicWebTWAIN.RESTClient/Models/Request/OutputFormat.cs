using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DynamicWebTWAIN.RestClient.Internal;

namespace DynamicWebTWAIN.RestClient
{
    public enum OutputFormat
    {
        [Parameter(Value = "image/png")]
        ImagePng,
        [Parameter(Value = "image/jpeg")]
        ImageJpeg,
        [Parameter(Value = "image/tiff")]
        ImageTiff,
        [Parameter(Value = "application/pdf")]
        ApplicationPdf
    }
   
}
