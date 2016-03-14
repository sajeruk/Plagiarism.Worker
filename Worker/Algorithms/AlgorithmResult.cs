using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiarism.Worker.Algorithms
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AlgorithmResult
    {
        [JsonProperty(PropertyName = "algoId")]
        public int AlgorithmId { get; set; }
        [JsonProperty(PropertyName = "similarity")]
        public double Similarity { get; set; }
        [JsonProperty(PropertyName = "verdict")]
        public string AlgorithmVerdict { get; set; }

        public static AlgorithmResult Res(int id, double sim, string verdict)
        {
            AlgorithmResult res = new AlgorithmResult();
            res.AlgorithmId = id;
            res.Similarity = sim;
            res.AlgorithmVerdict = verdict;
            return res;
        }
    }

    public class AlgorithmAggregatedResult
    {
        [JsonProperty(PropertyName = "results")]
        public AlgorithmResult[] AlgoResults { get; set; }
        [JsonProperty(PropertyName = "aggregated")]
        public AlgorithmResult AggregationInfo { get; set; }
    }
}
