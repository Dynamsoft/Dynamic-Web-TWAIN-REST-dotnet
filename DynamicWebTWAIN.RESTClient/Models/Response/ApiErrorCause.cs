using System;
using System.Diagnostics;
using System.Globalization;

namespace DynamicWebTWAIN.RestClient
{
    [Serializable]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ApiErrorCause
    {
        public ApiErrorCause() { }

        public ApiErrorCause(string message, string code)
        {
            Message = message;
            Code = code;
        }

        public string Message { get; private set; }

        public string Code { get; private set; }

        internal string DebuggerDisplay
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "Message: {0}, Code: {1}, Field: {2}", Message, Code);
            }
        }
    }
}
