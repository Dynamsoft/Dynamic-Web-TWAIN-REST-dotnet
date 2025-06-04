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
    public class CheckBlankSettings
    {
        /// <summary>
        /// Minimum blemish pixel height detection threshold.
        /// </summary>
        [Parameter(Key = "minBlockHeight")]
        public int MinBlockHeight { get; set; }

        /// <summary>
        /// Maximum blemish pixel height detection threshold.
        /// </summary>
        [Parameter(Key = "maxBlockHeight")]
        public int MaxBlockHeight { get; set; }
    }

}
