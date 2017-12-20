using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net;
using System.IO;
using iLevel.CodeAnalysis.BusinessLogicLayer.DTO;
using iLevel.CodeAnalysis.BusinessLogicLayer.Specification;
using iLevel.CodeAnalysis.AnalyzersAccesLayer.Interfaces;

namespace CodeAnalysisService.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDiagnosticProvider _diagnosticProvider;

        private readonly string DefaultCsHarpExtension = ".cs";

        public string OkMessage { get { return "As a result of diagnostics no warnings were found in your files"; } }


        public HomeController(IDiagnosticProvider diagnosticProvider)
        {
            _diagnosticProvider = diagnosticProvider;
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
            return null;
        }
    }
}