using DynamicWebTWAIN.RestClient.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicWebTWAIN.RestClient
{
    public class ScannerJobStatus
    {
        /// <summary>
        /// Current status of the job.
        /// </summary>
        [Parameter(Key = "status")]
        public StringEnum<JobStatus> Status { get; private set; }

        [Parameter(Key = "code")]
        public int Code { get; private set; } = 0;

        [Parameter(Key = "message")]
        public string Message { get; private set; } = null;
    }

}
