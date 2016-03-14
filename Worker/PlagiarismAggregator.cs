using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plagiarism.Worker.Algorithms;
using Plagiarism.Worker.Misc;

namespace Plagiarism.Worker
{
    public enum AggregatorType
    {
        Min,
        Max,
        Median
    }

    public interface IPlagiarismAggregator
    {
        AlgorithmResult Aggregate(IEnumerable<AlgorithmResult> results);
    }

    public class MinAggregator : IPlagiarismAggregator
    {
        public AlgorithmResult Aggregate(IEnumerable<AlgorithmResult> results)
        {
            var agg = results.Aggregate((a, b) => a.Similarity < b.Similarity ? a : b);
            return AlgorithmResult.Res(Constants.AggregatorId, agg.Similarity, agg.AlgorithmVerdict);
        }
    }

    public class MaxAggregator : IPlagiarismAggregator
    {
        public AlgorithmResult Aggregate(IEnumerable<AlgorithmResult> results)
        {
            var agg = results.Aggregate((a, b) => a.Similarity > b.Similarity ? a : b);
            return AlgorithmResult.Res(Constants.AggregatorId, agg.Similarity, agg.AlgorithmVerdict);
        }
    }

    public class MedianAggregator : IPlagiarismAggregator
    {
        public AlgorithmResult Aggregate(IEnumerable<AlgorithmResult> results)
        {
            var res = results.OrderBy(a => a.Similarity).ToArray();
            var agg = res[res.Length / 2];
            return AlgorithmResult.Res(Constants.AggregatorId, agg.Similarity, agg.AlgorithmVerdict);
        }
    }
}
