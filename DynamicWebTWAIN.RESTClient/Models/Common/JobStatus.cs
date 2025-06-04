using DynamicWebTWAIN.RestClient.Internal;

namespace DynamicWebTWAIN.RestClient
{
    public enum JobStatus
    {
        [Parameter(Value = "pending")]
        Pending,

        [Parameter(Value = "running")]
        Running,

        [Parameter(Value = "completed")]
        Completed,

        [Parameter(Value = "faulted")]
        Faulted,

        [Parameter(Value = "canceled")]
        Canceled,
    }
}
