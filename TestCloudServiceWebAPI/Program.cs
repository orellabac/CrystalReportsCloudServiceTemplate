using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace TestCloudServiceWebAPI
{
    public class ReportInfo
    {
        public string ReportName { get; set; }
        public string TenantName { get; set; }
        public string Token { get; set; }
    }

    public class ReportDefinitionInfo
    {
        public string Url { get; set; }
    }


    class Program
    {
        static void Main(string[] args)
        {

            var client = new HttpClient();
            var baseAddress = "http://<your cloud service address>.cloudapp.net/";
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            var reportInfo = new ReportInfo() { ReportName = "SampleReport.rpt", TenantName = "Tenant1", Token = Guid.NewGuid().ToString() };

            var st = new Stopwatch();
            st.Start();
            HttpResponseMessage response = client.PostAsJsonAsync("api/crystalreports", reportInfo).Result;
            st.Stop();
            response.EnsureSuccessStatusCode();
            var res = response.Content.ReadAsAsync<ReportDefinitionInfo>().Result;
            Console.WriteLine($"Call processed in {st.ElapsedMilliseconds} ms");

        }
    }
}
