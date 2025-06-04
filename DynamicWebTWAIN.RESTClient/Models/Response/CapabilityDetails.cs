using DynamicWebTWAIN.RestClient.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicWebTWAIN.RestClient
{
    /**
     * Detailed information about a specific capability
     */
    public class CapabilityDetails
    {
        /**
         * The Capability.
         */
        [Parameter(Key = "capability")]
        public ValueAndLabel Capability { get; private set; }

        /**
         * The container type of the Capability
         */
        [Parameter(Key = "conType")]
        public ValueAndLabel ConType { get; private set; } = null;

        /**
         * The index for the current value of the Capability
         */
        [Parameter(Key = "curIndex")]
        public int? CurIndex { get; private set; }

        /**
         * The current value of the Capability
         * Except TWON_ARRAY type whose current values return via the attribute values
         */
        [Parameter(Key = "curValue")]
        public object CurValue { get; private set; } = null;

        /**
         * The default value of the Capability
         */
        [Parameter(Key = "defaultValue")]
        public object DefaultValue { get; private set; } = null;

        /**
         * The maximum value of the Capability
         */
        [Parameter(Key = "maxValue")]
        public int? MaxValue { get; private set; }

        /**
         * The minimum value of the Capability
         */
        [Parameter(Key = "minValue")]
        public int? MinValue { get; private set; }

        /**
         * The step size of the Capability
         */
        [Parameter(Key = "stepSize")]
        public int? StepSize { get; private set; }

        /**
         * The index for the default value of the Capability
         */
        [Parameter(Key = "defIndex")]
        public int? DefIndex { get; private set; }

        /**
         * The operation types that are supported by the Capability. Types include {"get", "set", "reset" "getdefault", "getcurrent"}
         */
        [Parameter(Key = "query")]
        public IReadOnlyList<string> Query { get; private set; } = null;

        /**
         * The value type of the Capability. 
         */
        [Parameter(Key = "valueType")]
        public ValueAndLabel ValueType { get; private set; } = null;

        /**
         * The current values of the Capability when conType's label is TWON_ARRAY.
         */
        [Parameter(Key = "values")]
        public IReadOnlyList<object> Values { get; private set; } = null;

        /**
         * The available values of the Capability when conType's label is TWON_ENUMERATION.
         */
        [Parameter(Key = "enums")]
        public IReadOnlyList<object> Enums { get; private set; } = null;
    }

    public class ValueAndLabel
    {
        /**
         * Numeric representation of the item
         */
        [Parameter(Key = "value")]
        public int Value { get; private set; } = 0;

        /**
         * Label or name of the item
         */
        [Parameter(Key = "label")]
        public string Label { get; private set; } = null;
    }
}
