using DynamicWebTWAIN.RestClient.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicWebTWAIN.RestClient
{
    public class CapabilitiesUpdate : RequestParameters
    {
        /**
            * Whether to "ignore" or "fail" the request if an exception occurs. This is an overall setting that is inherited by all capabilities.
            */
        [Parameter(Key = "exception")]
        public StringEnum<EnumException>? Exception { get; set; }

        /**
         * Specifies how to set capabilities
         */
        [Parameter(Key = "capabilities")]
        public IReadOnlyList<CapabilitySetup> Capabilities { get; set; } = null;
    }

    public class CapabilitySetup : RequestParameters
    {
        /**
         * Specify a capability
         */
        [Parameter(Key = "capability")]
        public EnumDWT_Cap Capability { get; set; }

        /**
         * The value to set to the capability or the value of the capability after setting.
         * Except TWON_ARRAY type whose current values are set via the attribute values.
         */
        [Parameter(Key = "curValue")]
        public object CurValue { get; set; }

        /**
         * Whether to "ignore" or "fail" the request if an exception occurs when setting this specific capability.
         */
        [Parameter(Key = "exception")]
        public StringEnum<EnumException>? Exception { get; set; }
    }

    public class CapabilityResponse : CapabilitySetup
    {
        [Parameter(Key = "errorCode")]
        public int? ErrorCode { get; set; }

        [Parameter(Key = "errorString")]
        public string ErrorString { get; set; }
    }


    public enum EnumException
    {
        [Parameter(Value = "ignore")]
        Ignore,

        [Parameter(Value = "fail")]
        Fail,
    }
}
