using CodeAnalysis.BusinessLogicLayer;
using CodeAnalysisService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace CodeAnalysisService.Controllers
{
    public class AnalyzerController : ApiController
    {
        private readonly IDiagnosticService _diagnosticService;
        private readonly ISolutionCreator _solutionCreator;

        public AnalyzerController(IDiagnosticService diagnosticService, ISolutionCreator solutionCreator)
        {
            _diagnosticService = diagnosticService;
            _solutionCreator = solutionCreator;
        }

        public async Task<HttpResponseMessage> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var provider = new MultipartMemoryStreamProvider();

            await Request.Content.ReadAsMultipartAsync(provider);

            List<string> wrongFilesList = new List<string>();
            List<string> normalFilesList = new List<string>();

            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');

                if (Path.GetExtension(filename) != ".cs")
                {
                    wrongFilesList.Add(filename);
                    continue;
                }
                else
                {
                    string text = await file.ReadAsStringAsync();
                    normalFilesList.Add(text);
                }
            }


            if (wrongFilesList.Count > 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Some of files has not appropriate format");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
        }
    }
}
