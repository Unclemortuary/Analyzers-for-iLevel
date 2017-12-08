using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net;
using System.IO;
using iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces;

namespace CodeAnalysisService.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDiagnosticProvider _diagnosticService;
        private readonly ISolutionProvider _solutionCreator;

        private readonly string DefaultCsHarpExtension = ".cs";
        private readonly string DefaultAssemblyName = "ilevel";

        public string OkMessage { get { return "As a result of diagnostics no warnings were found in your files"; } }


        public HomeController(IDiagnosticProvider diagnosticService, ISolutionProvider solutionCreator)
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
            {
                var proj = _solutionCreator.GetProject(files);
                var result = _diagnosticService.GetCompilationDiagnostic(proj, AnalyzerProvider.Analyzers.ToImmutableArray());
                if (result.Count() == 0)
                    return Json(OkMessage);
                return Json(result);
            }
            else
                return Json(diagnostics);
        }
    }
}