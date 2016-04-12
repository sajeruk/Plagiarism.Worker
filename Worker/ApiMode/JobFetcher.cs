using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiarism.Worker.ApiMode
{
    public class JobFetcher
    {
        HttpApiCommunicator Comm;

        public JobFetcher(HttpApiCommunicator comm)
        {
            Comm = comm;
        }

        public bool GetJob(ref Job job)
        {
            var task = Comm.GetUnjudgedSolutionInfo();
            try
            {
                task.Wait();
            }
            catch (Exception e)
            {
                Logger.Error("Error waiting task: {0}", e.Message);
                return false;
            }
            job = JsonConvert.DeserializeObject<Job>(task.Result);
            return task.Result.Length > 0;
        }
    }
}
