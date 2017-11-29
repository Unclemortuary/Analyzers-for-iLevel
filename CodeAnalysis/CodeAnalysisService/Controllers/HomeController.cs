using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeAnalysisService.Models;
using CodeAnalysis.BusinessLogicLayer;
using System.Net;
using System.Net.Http;
using System.IO;

namespace CodeAnalysisService.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDiagnosticService _diagnosticService;
        private readonly ISolutionCreator _solutionCreator;

        private readonly string DefaultCsHarpExtension = ".cs";
        private readonly string DefaultAssemblyName = "ilevel";


        public HomeController(IDiagnosticService diagnosticService, ISolutionCreator solutionCreator)
        {
            _diagnosticService = diagnosticService;
            _solutionCreator = solutionCreator;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload()
        {
            if (Request.Files.Count == 0)
                return new HttpStatusCodeResult(204, "No files was received");

            List<string> wrongFilesList = new List<string>();
            Dictionary<string, string> normalFilesList = new Dictionary<string, string>();

            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                if (upload != null)
                {
                    if(Path.GetExtension(upload.FileName) != DefaultCsHarpExtension)
                    {
                        wrongFilesList.Add(upload.FileName);
                        continue;
                    }
                    else
                    {
                        string fileName = Path.GetFileName(upload.FileName);
                        StreamReader streamReader = new StreamReader(upload.InputStream);
                        string text = streamReader.ReadToEnd();
                        normalFilesList.Add(fileName, text);
                    }
                }
            }

            if (wrongFilesList.Count != 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Some files has not appropriate format");
            }
            else
            {
                try
                {
                    var returnableMessage = GetCompilationDiagnostic(normalFilesList);
                    return returnableMessage;
                }
                catch(Exception e)
                {
                    if (e is NullReferenceException || e is ArgumentNullException)
                        return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                    else
                        throw;
                }
            }
        }

        internal JsonResult GetCompilationDiagnostic(Dictionary<string, string> files)
        {
            if (files == null)
                throw new ArgumentNullException();
            var compilation = _solutionCreator.GetCompilation(_solutionCreator.GetSyntaxTrees(files), DefaultAssemblyName);
            var diagnostics = _diagnosticService.GetCompilationDiagnostic(compilation);
            if (diagnostics == null)
                throw new NullReferenceException();
            if (diagnostics.Count() == 0)
                return Json("Your solution is OK");
            else
                return Json(diagnostics);
        }
    }
}