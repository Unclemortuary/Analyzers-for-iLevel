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
        public static List<string> _solutions = new List<string>();

        public HttpResponseMessage Get(int id)
        {
            if (id < _solutions.Count)
                return Request.CreateResponse(HttpStatusCode.OK, GetDiagnostic(_solutions));
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Your solution not found");
        }

        
        //Here implementation for non async upload
        //
        //public HttpResponseMessage Post()
        //{
        //    var file = HttpContext.Current.Request.Files[0];
        //    var fileName = Path.GetFileName(file.FileName);
        //    string root = HttpContext.Current.Server.MapPath("~/App_Data");
        //    file.SaveAs(root + "//" + fileName);
        //    StreamReader streamReader = new StreamReader(root + "//" + fileName);
        //    string text = streamReader.ReadToEnd();
        //    _solutions.Add(text);
        //    HttpResponseMessage msg = Request.CreateResponse(HttpStatusCode.Created, text);
        //    msg.Headers.Location = new Uri(Request.RequestUri + "/" + (_solutions.Count - 1));
        //    return msg;
        //}

        public async Task<HttpResponseMessage> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            var provider = new MultipartMemoryStreamProvider();

            //string root = HttpContext.Current.Server.MapPath("~/App_Data");
            await Request.Content.ReadAsMultipartAsync(provider);

            List<string> wrongFilesList = new List<string>();

            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');

                if (Path.GetExtension(filename) != ".cs") //subtitude ".cs" on some argument, that stored language type
                {
                    wrongFilesList.Add(filename);
                    continue;
                }
                else
                {
                    ///Here implementation for upload and save files in server directory "root"
                    ///
                    ///byte[] fileArray = await file.ReadAsByteArrayAsync();
                    ///filename = filename.Replace(".cs", ".txt"); //substitute ".cs" on some argument, that stored language type
                    ///using (FileStream fs = new FileStream(root + "\\" + filename, FileMode.Create))
                    ///{
                    ///    await fs.WriteAsync(fileArray, 0, fileArray.Length);
                    ///}
                    ///

                    string text = await file.ReadAsStringAsync();
                    _solutions.Add(text);
                }
            }
            if (wrongFilesList.Count > 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Some of files has not appropriate format");
            }
            else
            {
                HttpResponseMessage msg = Request.CreateResponse(HttpStatusCode.Created, "File(s) created");
                msg.Headers.Location = new Uri(Request.RequestUri + "/" + (_solutions.Count - 1));
                return msg;
            }
        }


        private string GetDiagnostic(List<string> list) //pass the analyzerId
        {
            var solutionCreator = new SolutionCreator(list.ToArray());
            //
            //set the analyzerID of SolutionCreater here
            //
            var document = solutionCreator.GetDocuments();
            var usedAnalyzer = AnalyzersSet.Init().GetAnalyzerById(solutionCreator.AnalyzerId);
            var diagnosticResult = Analyzer.AnalyzeSolution(usedAnalyzer, document);
            var returnableResult = Analyzer.FormatDiagnostics(usedAnalyzer, diagnosticResult);
            return returnableResult;
        }
    }
}
