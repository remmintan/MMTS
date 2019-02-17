using System.IO;
using System.Net;
using System.Net.Http;

namespace DataProvider
{
    internal class ApiClient
    {
        private readonly WebProxy proxy;

        public ApiClient()
        {
            if (File.Exists("./proxy.creds"))
            {
                var creds = File.ReadAllLines("./proxy.creds");
                proxy = new WebProxy(creds[0]);
                proxy.Credentials = new NetworkCredential(creds[1], creds[2]);
            }
            else
            {
                proxy = null;
            }
            
        }

        public string GetTickerData(string Url)
        {
            using (var client = new HttpClient(new HttpClientHandler { Proxy = proxy, UseProxy = proxy!=null }))
            {
                return client.GetStringAsync(Url).Result;
            }
        }
    }
}
