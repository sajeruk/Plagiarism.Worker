using Plagiarism.Worker.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Plagiarism.Worker.SelfTestingMode
{
    public class Instance : WorkerInstance
    {
        PlagiarismTester PlagiarismChecker;

        public Instance(Configuration config) : base(config)
        {
            PlagiarismChecker = new PlagiarismTester(config);
            Sleeper = new ConstantSleeper(1);
        }

        protected override void DoRun(CancellationToken token)
        {
            var ans = PlagiarismChecker.Check(TestData.Source1, TestData.Source2);
            foreach (var res in ans.AlgoResults)
            {
                Logger.Info("Check result - Id: {0}, Similarity: {1}, Verdict: {2}", res.AlgorithmId, res.Similarity, res.AlgorithmVerdict);
                Sleeper.Sleep();
            }
            Logger.Info("Aggregated result - Id: {0}, Similarity: {1}, Verdict: {2}", ans.AggregationInfo.AlgorithmId, ans.AggregationInfo.Similarity, ans.AggregationInfo.AlgorithmVerdict);
        }
    }
}
