using Newtonsoft.Json;
using Plagiarism.Worker.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiarism.Worker.ApiMode
{
    public class Solution
    {
        [JsonProperty(PropertyName = "id")]
        public int SolutionId { get; set; }
        [JsonProperty(PropertyName = "resourceId")]
        public string SolutionHash { get; set; }
    }

    public class Job
    {
        [JsonProperty(PropertyName = "solution")]
        public Solution SolutionToJudge { get; set; }
        [JsonProperty(PropertyName = "solutions")]
        public Solution[] SolutionsToCompare { get; set; }
    }

    public class ComparasionResult
    {
        public int SolutionId { get; set; }
        public AlgorithmAggregatedResult TestResult { get; set; }
    }

    public class JobTestResult
    {
        public int SolutionId { get; set; }
        public ComparasionResult[] OtherSolutions { get; set; }

    }
}
