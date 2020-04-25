using Plagiarism.Worker.Algorithms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Plagiarism.Worker
{
    public class PlagiarismTester
    {
        private IAlgorithm[] Algos;
        private IPlagiarismAggregator Aggregator;

        public PlagiarismTester(Configuration conf)
        {
            var enabledAlgos = conf.Algorithms.Where(algo => algo.Enabled);
            if (enabledAlgos.Count() == 0)
            {
                Logger.Error("No algorithms for testing provided, exitting...");
                throw new Exception("No algorithms provided, check config file");
            }
            Algos = new IAlgorithm[enabledAlgos.Count()];
            Aggregator = CreateAggregator(conf.AggType);
            int i = 0;
            foreach (var enabledAlgo in conf.Algorithms.Where(algo => algo.Enabled))
            {
                Algos[i] = new DllAlgorithm(
                    enabledAlgo.Id,
                    enabledAlgo.Name,
                    enabledAlgo.Enabled,
                    Path.Combine(conf.DllDirectory, enabledAlgo.DllName));
                ++i;
            }
        }

        public AlgorithmAggregatedResult Check(Source source1, Source source2)
        {
            var answer = new List<AlgorithmResult>();
            foreach (var algo in Algos)
            {
                var baseAlgo = (algo as BaseAlgorithm);
                var cmpRes = algo.CompareSrc(source1, source2);
                var res = AlgorithmResult.Res(baseAlgo.Id, cmpRes.Key, cmpRes.Value);
                answer.Add(res);
            }

            var result = new AlgorithmAggregatedResult();
            result.AlgoResults = answer.ToArray();
            result.AggregationInfo = Aggregator.Aggregate(answer);

            return result;
        }

        private IPlagiarismAggregator CreateAggregator(AggregatorType type)
        {
            Logger.Info("Aggregator type: {0}", type.ToString());
            switch (type)
            {
                case AggregatorType.Max:
                    return new MaxAggregator();
                case AggregatorType.Min:
                    return new MinAggregator();
                case AggregatorType.Median:
                    return new MedianAggregator();
                default:
                    string error = string.Format("Unsupported aggregator type: {0}", type.ToString());
                    Logger.Error(error);
                    throw new Exception(error);
            }
        }
    }
}
