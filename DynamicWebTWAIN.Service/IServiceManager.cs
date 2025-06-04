using System;

namespace DynamicWebTWAIN.Service
{
    public interface IServiceManager : IDisposable
    {
        string ServiceDirectory { get; }

        string ServiceProcessName { get; }

        string ServiceFullPath { get; }

        int MaxIdleTime { get; }

        bool ServiceFileExists { get; }

        void CreateService();

        IService Service { get; }
    }
}
