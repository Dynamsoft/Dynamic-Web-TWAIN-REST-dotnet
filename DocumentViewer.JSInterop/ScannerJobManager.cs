using DynamicWebTWAIN.RestClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dynamsoft.DocumentViewer
{
    public class ScannerJobManager
    {
        private readonly Dictionary<string, JobData> _jobClients = new Dictionary<string, JobData>();
        private readonly object _lock = new object();

        public void AddJob(string jobUid, IScannerJobClient jobClient)
        {
            lock (_lock)
            {
                if (!_jobClients.ContainsKey(jobUid))
                {
                    _jobClients[jobUid] = new JobData
                    {
                        JobClient = jobClient,
                        RemainingPages = new HashSet<int>()
                    };
                }
            }
        }

        public async Task SetJobTransferEnded(string jobUid)
        {
            JobData jobData = null;
            bool shouldDeleteJob = false;

            lock (_lock)
            {
                if (_jobClients.TryGetValue(jobUid, out jobData))
                {
                    jobData.TransferEnded = true;
                    if (jobData.RemainingPages.Count == 0)
                    {
                        _jobClients.Remove(jobUid);
                        shouldDeleteJob = true;
                    }
                }
            }

            if (shouldDeleteJob)
            {
                await DeleteJob(jobData.JobClient);
            }
        }

        public async Task<bool> DeleteJob(string jobUid)
        {
            JobData jobData = null;

            lock (_lock)
            {
                if (_jobClients.TryGetValue(jobUid, out jobData))
                {
                    _jobClients.Remove(jobUid);
                }
            }

            if (jobData != null)
            {
               await DeleteJob(jobData.JobClient);
               return true;
            }

            return false;
        }

        private async Task DeleteJob(IScannerJobClient jobClient)
        {
            try
            {
                await jobClient.DeleteJob();
            }
            catch (Exception ex)
            {
                // Handle exception if needed
                Console.WriteLine($"Error deleting job: {ex.Message}");
            }
        }

        public void AddPage(string jobUid, int pageNumber)
        {
            lock (_lock)
            {
                if (_jobClients.TryGetValue(jobUid, out var jobData))
                {
                    jobData.RemainingPages.Add(pageNumber);
                }
            }
        }

        public async Task RemovePage(string jobUid, int pageNumber)
        {
            JobData jobData = null;
            bool shouldDeleteJob = false;

            lock (_lock)
            {
                if (_jobClients.TryGetValue(jobUid, out jobData))
                {
                    jobData.RemainingPages.Remove(pageNumber);

                    if (jobData.RemainingPages.Count == 0 && jobData.TransferEnded)
                    {
                        _jobClients.Remove(jobUid);
                        shouldDeleteJob = true;
                    }
                }
            }

            if (shouldDeleteJob)
            {
                await DeleteJob(jobData.JobClient);
            }
        }

        public bool JobExists(string jobUid)
        {
            lock (_lock)
            {
                return _jobClients.ContainsKey(jobUid);
            }
        }

        private class JobData
        {
            public IScannerJobClient JobClient { get; set; }
            public HashSet<int> RemainingPages { get; set; }

            public bool TransferEnded { get; set; } = false;
        }
    }
}
