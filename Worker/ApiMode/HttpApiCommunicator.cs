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

        public async Task<string> GetUnjudgedSolutionInfo()
        {
            string path = "api/plagiarism/take";
            return await Client.GetStringAsync(path);
        }

        public void SendJudgedSolutionInfo(JobTestResult res)
        {

        }

        public async Task<string> DownloadFile(string id)
        {
            string path = "api/fs/" + id;
            return await Client.GetStringAsync(path);
        }
    }
}
