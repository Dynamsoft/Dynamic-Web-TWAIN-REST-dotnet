using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace DynamicWebTWAIN.Service
{
    public class ServiceManager : IServiceManager
    {
        public static string DefaultServiceProcessName = "DynamicWebTWAINService.exe";
        public static readonly int DefaultMaxIdleTime = 5; // in seconds, if has websocket connection, the service will not be closed auto

        public IService Service {  get; private set; }

        public string ServiceDirectory { get; private set; }

        public string ServiceProcessName { get; private set; }

        public int MaxIdleTime { get; private set; }

        public string ServiceFullPath { get; private set; }

        public bool ServiceFileExists
        {
            get
            {
                return File.Exists(ServiceFullPath);
            }
        }

        public ServiceManager()
            : this(null)
        {
        }

        public ServiceManager(string serviceDirectory)
            : this(serviceDirectory, DefaultServiceProcessName, DefaultMaxIdleTime)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                DefaultServiceProcessName = "DynamicWebTWAINService.app";
            }
        }

        public ServiceManager(string serviceDirectory, int maxIdleTime)
            : this(serviceDirectory, DefaultServiceProcessName, maxIdleTime)
        {
        }

        public ServiceManager(string serviceDirectory, string serviceProcessName, int maxIdleTime)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new InvalidOperationException("Only Windows and macOS support creating services");
            }

            if (string.IsNullOrEmpty(serviceDirectory))
            {
                string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (assemblyLocation != null)
                {
                    string x64FolderName = "win-x64";
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        x64FolderName = "maccatalyst-x64";
                    }

                    serviceDirectory = Path.Combine(new string[] { assemblyLocation, "runtimes", x64FolderName, "native", "dynamsoft.dwt.service" });
                }

                else
                {
                    throw new FileNotFoundException("Service folder not found");
                }
            }

            if (string.IsNullOrEmpty(serviceProcessName))
            {
                serviceProcessName = DefaultServiceProcessName;
            }

            if (maxIdleTime < 0)
            {
                maxIdleTime = DefaultMaxIdleTime;
            }

            ServiceDirectory = serviceDirectory;
            ServiceProcessName = serviceProcessName;
            MaxIdleTime = maxIdleTime;
            ServiceFullPath = System.IO.Path.Combine(ServiceDirectory, ServiceProcessName);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                ServiceFullPath = System.IO.Path.Combine(ServiceFullPath, "Contents/MacOS/DynamicWebTWAINService");
            }

            if (!ServiceFileExists)
            {
                throw new FileNotFoundException("Service file not found", ServiceFullPath);
            }
        }

        public void CreateService()
        {
            if (Service != null)
            {
                Service.Dispose();
            }

            if (!ServiceFileExists)
            {
                return;
            }

            Process process = new Process();
            string sslServer = "", normalServer = "";

            Semaphore pool = new Semaphore(initialCount: 0, maximumCount: 1);

            // try to start service
            string lockFile = System.IO.Path.Combine(ServiceDirectory, "port.lock");
            string arguments = String.Format("-asconsole -asagent \"{0}\" {1} \"{2}\"", lockFile, MaxIdleTime,
                Environment.UserName);
            process.StartInfo.FileName = ServiceFullPath;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(e.Data);
                    if (e.Data.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                        sslServer = e.Data;
                    else if (e.Data.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                        normalServer = e.Data;
                    if (!String.IsNullOrEmpty(sslServer) && !String.IsNullOrEmpty(normalServer))
                        pool.Release();
                }
            });

            if (process.Start())
            {
                process.BeginOutputReadLine();

                pool.WaitOne();

                if (string.IsNullOrEmpty(sslServer) && string.IsNullOrEmpty(normalServer))
                {
                    process.Kill();
                }

                Service = new Service(new Uri(sslServer), new Uri(normalServer), process);
            }
        }


        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Service != null)
                {
                    Service.Dispose();
                }
            }
        }
    }
}
