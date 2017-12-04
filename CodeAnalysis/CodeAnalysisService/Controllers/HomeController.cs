using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CodeAnalysis.BusinessLogicLayer;
using System.Net;
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
            return View(nameof(this.Index));
        }

        [HttpPost]
        public ActionResult Upload()
        {
            if (Request.Files.Count == 0)
                return new HttpStatusCodeResult(HttpStatusCode.NoContent, "No files was received");

            Dictionary<string, string> normalFiles = new Dictionary<string, string>();

            foreach (var file in Request.Files)
            {
                var upload = Request.Files[file.ToString()];
                if (upload != null)
                {
                    if (Path.GetExtension(upload.FileName) != DefaultCsHarpExtension)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Some files has not appropriate format");

                    string fileName = Path.GetFileName(upload.FileName);
                    using (StreamReader streamReader = new StreamReader(upload.InputStream))
                    {
                        string text = streamReader.ReadToEnd();
                        normalFiles.Add(fileName, text);
                    }
                }
            }
            var returnableMessage = GetCompilationDiagnostic(normalFiles);
            return returnableMessage;
        }

        internal JsonResult GetCompilationDiagnostic(Dictionary<string, string> files)
        {
            files = files ?? throw new ArgumentNullException(nameof(files));
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