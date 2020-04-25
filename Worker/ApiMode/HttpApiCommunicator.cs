using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Plagiarism.Worker.ApiMode
{
    public class HttpApiCommunicator
    {
        HttpClient Client;
        public HttpApiCommunicator(HttpClient client)
        {
            Client = client;
        }

        public Task<string> GetUnjudgedSolutionInfo()
        {
            string path = "api/plagiarism/take";
            return Client.GetStringAsync(path);
        }

        public Task<byte[]> DownloadFile(string id)
        {
            string path = "api/fs/" + id;
            return Client.GetByteArrayAsync(path);
        }

        public void PutReport(string json)
        {
            HttpContent requestContent = new StringContent(json, Encoding.UTF8, "application/json");
            string url = "api/plagiarism/put";
            var result = Client.PutAsync(url, requestContent).Result;
            if (!result.IsSuccessStatusCode)
            {
                try
                {
                    Logger.Info(result.Content.ReadAsStringAsync().Result);
                }
                catch (AggregateException e)
                {
                    Logger.Error(e.Message);
                }
            }
        }
    }
}
