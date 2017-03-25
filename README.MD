Crystal Reports Cloud Service
============================= 

Crystal reports cannot be installed on Azure WebSites.
This project provides a template as a reference to create a cloud Service with Crystal Reports installed.
The service generates the reports as PDF stored them on your storage account and then provide an expiring URL that the user can use
to open the generated PDF.
This is a very basic implementation and is just provided as an starting point.


To run the code from this template.
1. Add all your reports to the project and set them as content with copy always settings.
2. Create an storage account
3. Add an entry on the Web.Config `<appSettings>` section. For example:

```
<add name="StorageConnectionString" connectionString=" <StorageConnectionString> "/>
```


Using the Cloud Service
------------------------

The cloud service is exposed thru Web API 

The following example shows how to invoke the cloudservice

```
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

```


Deployment Observations
=======================

The recommendation is to setup a virtual network and make sure that the cloud services is only accesible to your azure websites,
in that way the communication of report parameters is not exposed.



#NOTE:
The latest Crystal Reports Runtime installer can be downloaded from: 
https://wiki.scn.sap.com/wiki/display/BOBJ/Crystal+Reports%2C+Developer+for+Visual+Studio+Downloads

This installer is called from Startup.cmd

Remember to update the Startup.cmd script if you download a new version. 
