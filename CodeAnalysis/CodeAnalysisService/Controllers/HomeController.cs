using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeAnalysisService.Models;
using CodeAnalysis.BusinessLogicLayer;
using System.Net.Http;
using System.IO;

namespace CodeAnalysisService.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDiagnosticService _diagnosticService;
        private readonly ISolutionCreator _solutionCreator;

        private readonly string DefaultCsHarpExtension = ".cs";


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
        public JsonResult Upload()
        {
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

            if(wrongFilesList.Count != 0)
            {
                return Json(new string[] {"Some files hs not appropriate format"});
            }
            else
            {
                var compilation = _solutionCreator.GetCompilation(_solutionCreator.GetSyntaxTrees(normalFilesList), "ilvl");
                var diagnostics = _diagnosticService.GetCompilationDiagnostic(compilation);
                if (diagnostics.Count() == 0)
                    return Json(new string[] { "Your solution is OK" });
                else
                    return Json(diagnostics);
            }
        }
    }
}