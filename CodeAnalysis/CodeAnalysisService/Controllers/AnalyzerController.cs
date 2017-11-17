using CodeAnalysisService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CodeAnalysisService.Analyzer_Classes;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace CodeAnalysisService.Controllers
{
    public class AnalyzerController : ApiController
    {

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
                return Request.CreateResponse(HttpStatusCode.Created, GetDiagnostic(normalFilesList));
            }
        }


        internal string GetDiagnostic(List<string> list)
        {
            if (list == null || list.Count == 0)
                throw new ArgumentNullException("list of sources is empty");
            else
            {
                var solutionCreator = new SolutionCreator(list.ToArray());
                var document = solutionCreator.GetDocuments();
                var usedAnalyzer = AnalyzersSet.Init().GetAnalyzerById(solutionCreator.AnalyzerId);
                var diagnosticResult = Analyzer.AnalyzeSolution(usedAnalyzer, document);
                var returnableResult = Analyzer.FormatDiagnostics(usedAnalyzer, diagnosticResult);
                if (returnableResult == "")
                    return "Files not have any warnings or errors";
                return returnableResult;
            }
        }
    }
}
