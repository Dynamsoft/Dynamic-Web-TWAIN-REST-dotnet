using System;

namespace DynamicWebTWAIN.Service
{
    public interface IService : IDisposable
    {
        Uri BaseAddress { get; }

        Uri NormalBaseAddress { get; }

        /// <summary>
        /// Check if the service is running.
        /// </summary>
        bool IsAlive { get; }
    }
}
