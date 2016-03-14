using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace Plagiarism.Worker.ApiMode
{
    public class Instance : WorkerInstance
    {
        private JobFetcher Fetcher;
        private HttpApiCommunicator Comm;
        PlagiarismTester PlagiarismChecker;

        public Instance(Configuration config) : base(config)
        {
            Sleeper = new ConstantSleeper(config.ApiModeConfiguration.RequestTimeout);
            Logger.Info("Creating HttpClient, endpoint {0}...", config.ApiModeConfiguration.Endpoint);
            Comm = new HttpApiCommunicator(CreateHttpClient(config.ApiModeConfiguration.Endpoint));
            Fetcher = new JobFetcher(Comm);
            PlagiarismChecker = new PlagiarismTester(config);
        }

        protected override void DoRun(CancellationToken token)
        {
            Job job = null;
            while (!token.IsCancellationRequested)
            {
                if (!Fetcher.GetJob(ref job))
                {
                    Sleeper.Sleep();
                    continue;
                }
                  
                if (job == null)
                {
                    Logger.Error("Fetcher returned null job reference");
                    throw new NullReferenceException("GetJob returned null job");
                }

                RunTestMachineDebug(job);
                JobTestResult result = new JobTestResult();
                result.SolutionId = job.SolutionToJudge.SolutionId;
                result.OtherSolutions = new ComparasionResult[job.SolutionsToCompare.Length];
                Sleeper.Sleep();
            }
        }

        private HttpClient CreateHttpClient(string endpoint)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(endpoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
            return client;
        }

        private string LoadSource(string id)
        {
            var task = Comm.DownloadFile(id);
            task.Wait();
            return task.Result;
        }

        private void RunTestMachineDebug(Job job)
        {
            var src1 = LoadSource(job.SolutionToJudge.SolutionHash);
            foreach (var soluition in job.SolutionsToCompare)
            {
                var src2 = LoadSource(soluition.SolutionHash);
                var checkerResult = PlagiarismChecker.Check(src1, src2);
                string serialized = JsonConvert.SerializeObject(checkerResult);
                Logger.Info("Comparing solutions {0} and {1}: {2}", job.SolutionToJudge.SolutionId, soluition.SolutionId, serialized);
                
            }
        }
    }
}
