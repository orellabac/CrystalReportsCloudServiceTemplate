using CrystalDecisions.CrystalReports.Engine;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace CrystalReportsService.Controllers
{

    public class ReportInfo
    {
        public string ReportName { get; set; }
        public string TenantName { get; set; }
        public string Token { get; set; }
    }


    public class ReportDefinitionInfo
    {
        public string localFileInfo { get; set; }
    }




    public class ReportOutputResponse
    {
        public string Url { get; set; }
    }

    public class CrystalReportsController : ApiController
    {

        // POST api/crystalreports
        public HttpResponseMessage Post([FromBody]ReportInfo report)
        {
            try
            {
                var reportName = report.ReportName;
                var tenantName = report.TenantName;
                var crystalReportFileInfo = ObtainReportDefinition(reportName, tenantName);

                ReportDocument doc = new ReportDocument();
                doc.Load(crystalReportFileInfo.localFileInfo);
                var tempFile = Path.GetTempFileName();
                //Export to PDF on temp file
                doc.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, tempFile);

                var resName = GetReportOutputName(report);
                var url = new StorageAbstraction().Save(resName, tempFile);
                var responseMessage = new ReportOutputResponse() { Url = url };
                var response = Request.CreateResponse(responseMessage);
                return response;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error! " + ex.Message + "--" + ex.StackTrace);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Generates an unique name that will be used on the storage medium
        /// The name will be $"{reportInfo.Token}{bareFileName}{reportInfo.TenantName ?? ""}.pdf";
        /// </summary>
        private string GetReportOutputName(ReportInfo reportInfo)
        {
            //A guid is used as part of the name to make it unique. We make sure to remove extensions.
            var bareFileName = Path.GetFileNameWithoutExtension(reportInfo.ReportName);
            var outputname = $"{reportInfo.Token}{bareFileName}{reportInfo.TenantName ?? ""}.pdf";
            return outputname;
        }

        private ReportDefinitionInfo ObtainReportDefinition(string reportName,string tenantname)
        {
            //Valid permissions and apply mappings here

            var targetfilename = System.Web.Hosting.HostingEnvironment.MapPath($"~/{reportName}");

            return new ReportDefinitionInfo() { localFileInfo = targetfilename };
        }
    }
}
