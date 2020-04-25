using Newtonsoft.Json;
using Plagiarism.Worker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        LruCache<string, Source> Cache;

        public Instance(Configuration config) : base(config)
        {
            Sleeper = new ProgressiveSleeper(config.ApiModeConfiguration.RequestTimeout);
            Logger.Info("Creating HttpClient, endpoint {0}...", config.ApiModeConfiguration.Endpoint);
            Comm = new HttpApiCommunicator(CreateHttpClient(config.ApiModeConfiguration.Endpoint, config.ApiModeConfiguration.Token));
            Fetcher = new JobFetcher(Comm);
            PlagiarismChecker = new PlagiarismTester(config);
            Cache = new LruCache<string, Source>(10000);
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

                string json = null;
                if (RunTestMachine(job, ref json))
                {
                    Comm.PutReport(json);
                    Sleeper.Reset();
                }
                Sleeper.Sleep();
            }
        }

        private HttpClient CreateHttpClient(string endpoint, string token)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(endpoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
            client.DefaultRequestHeaders.Add("Worker-Token", token);
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest"); // To force django return error pages in plain text instead of HTML
            return client;
        }

        private Source LoadSource(string id)
        {
            Source result = Cache.Get(id);
            if (result == null)
            {
                byte[] content = Comm.DownloadFile(id);
                if (content != null)
                {
                    result = new Source(content);
                    Cache.Add(id, result);
                }
            }
            return result;
        }

        private bool RunTestMachine(Job job, ref string result)
        {
            Source src1 = LoadSource(job.SolutionToJudge.SolutionHash);
            if (src1 == null)
            {
                return false;
            }
            var comparasions = new List<ComparasionResult>();
            double plagiarismLevel = 0.0;

            Stopwatch sw = Stopwatch.StartNew();
            foreach (var soluition in job.SolutionsToCompare)
            {
                Logger.Debug("Testing {0} and {1}", job.SolutionToJudge.SolutionId, soluition.SolutionId);
                var src2 = LoadSource(soluition.SolutionHash);
                if (src2 == null)
                {
                    return false;
                }
                var checkerResult = PlagiarismChecker.Check(src1, src2);
                comparasions.Add(new ComparasionResult { SolutionId = soluition.SolutionId, TestResult = checkerResult });
                plagiarismLevel = Math.Max(plagiarismLevel, checkerResult.AggregationInfo.Similarity);
            }
            sw.Stop();
            Logger.Info("Tested {0} soluitons in {1} ms; result = {2:0.00}", job.SolutionsToCompare.Length, sw.ElapsedMilliseconds, plagiarismLevel);
            Logger.Debug("Cached solutions: {0}", Cache.Size());

            result = JsonConvert.SerializeObject(new JobTestResult{
                SolutionId = job.SolutionToJudge.SolutionId, OtherSolutions = comparasions.ToArray(), PlagiarismLevel = plagiarismLevel });
            return true;
        }
    }
}
