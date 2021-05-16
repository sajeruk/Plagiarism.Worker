using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Plagiarism.Worker.Algorithms
{
    class HttpAlgorithmSource
    {
        [JsonProperty(PropertyName = "text")]
        public byte[] Text { get; set; }
        [JsonProperty(PropertyName = "encoding")]
        public string Encoding { get; set; } = "base64";
        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }

        public HttpAlgorithmSource(Source source)
        {
            Text = source.GetRawBytesNullTerminated();
            Language = source.GetLanguage();
        }

        public Source ToSource()
        {
            return new Source(Text, Language);
        }
    }

    class HttpAlgorithmRequest
    {
        [JsonProperty(PropertyName = "sourcePair")]
        public HttpAlgorithmSource[] SourcePair { get; set; }
    }

    class HttpAlgorithmResponse
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        public class ResponseResult
        {
            [JsonProperty(PropertyName = "message")]
            public string Message { get; set; }

            [JsonProperty(PropertyName = "score")]
            public double Score { get; set; }
        }

        [JsonProperty(PropertyName = "result", Required = Required.DisallowNull)]
        public ResponseResult Result { get; set; }

        [JsonProperty(PropertyName = "message", Required = Required.DisallowNull)]
        public string Message { get; set; }
    }

    [Serializable]
    public class HttpAlgorithm : BaseAlgorithm
    {
        public string Endpoint;
        public string Token;

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(Endpoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Auth-Token", Token);
            return client;
        }

        public HttpAlgorithm() : base(-1, "", false)
        {
            Endpoint = "";
        }

        public HttpAlgorithm(int id, string name, bool enabled, string endpoint) : base(id, name, enabled)
        {
            Endpoint = endpoint;
        }

        public override KeyValuePair<double, string> CompareSrc(Source source1, Source source2)
        {
            var client = CreateHttpClient();
            var request = new HttpAlgorithmRequest();
            request.SourcePair = new[] {
                new HttpAlgorithmSource(source1),
                new HttpAlgorithmSource(source2)
            };
            var reqJson = JsonConvert.SerializeObject(request);
            HttpContent requestContent = new StringContent(reqJson, Encoding.UTF8, "application/json");
            var result = client.PostAsync("check", requestContent).Result;
            if (!result.IsSuccessStatusCode)
            {
                Logger.Info("HTTP request finished with code " + result.StatusCode.ToString());
            }
            var responseStr = result.Content.ReadAsStringAsync().Result;
            var response = JsonConvert.DeserializeObject<HttpAlgorithmResponse>(responseStr);
            if (response == null)
            {
                Logger.Error("Got empty response");
                return new KeyValuePair<double, string>(0.5, "Empty response");
            }
            if (response.Result == null)
            {
                Logger.Error("Got error: " + response.Message);
                return new KeyValuePair<double, string>(0.5, "Error: " + response.Message);
            }
            return new KeyValuePair<double, string>(response.Result.Score, response.Result.Message);
        }
    }
}
