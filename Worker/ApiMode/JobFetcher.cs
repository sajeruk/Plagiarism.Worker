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
            task.Wait();
            Logger.Info("Recieved line: {0}", task.Result);
            job = JsonConvert.DeserializeObject<Job>(task.Result);

            return task.Result.Length > 0;
        }
    }
}
