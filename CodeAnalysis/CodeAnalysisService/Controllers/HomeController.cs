using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net;
using System.IO;
using CodeAnalysisService.Infrastructure;
using iLevel.CodeAnalysis.BusinessLogicLayer.Specification;
using iLevel.CodeAnalysis.AnalyzersAccesLayer.Interfaces;

namespace CodeAnalysisService.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDiagnosticProvider _diagnosticProvider;
        private readonly IMapper _mapper;

        private readonly string DefaultCsHarpExtension = ".cs";

        public  string OkMessage { get { return "As a result of diagnostics no warnings were found in your files"; } }
        public string NoFilesMessage { get { return "No files was received"; } }
        public string WrongExtensionMessage { get { return "Some of files has not appropriate format"; } }


        public HomeController(IDiagnosticProvider diagnosticProvider, IMapper mapper)
        {
            _diagnosticProvider = diagnosticProvider;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            return View(nameof(this.Index));
        }

        [HttpPost]
        public ActionResult UploadAndReturnDiagnostic()
        {
            if (Request.Files.Count == 0)
                return new HttpStatusCodeResult(HttpStatusCode.NoContent, NoFilesMessage);

            Dictionary<string, string> normalFiles = new Dictionary<string, string>();

            foreach (var file in Request.Files)
            {
                var upload = Request.Files[file.ToString()];
                if (upload != null)
                {
                    if (Path.GetExtension(upload.FileName) != DefaultCsHarpExtension)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, WrongExtensionMessage);

                    string fileName = Path.GetFileName(upload.FileName);
                    using (StreamReader streamReader = new StreamReader(upload.InputStream))
                    {
                        string text = streamReader.ReadToEnd();
                        normalFiles.Add(fileName, text);
                    }
                }
            }
            if (normalFiles.Count > 0)
            {
                var sourcesDTO = _mapper.ToSourceFileDTO(normalFiles);
                var returnedDiagnostic = _diagnosticProvider.GetDiagnostic(
                    sourcesDTO, AnalyzerProvider.Analyzers, new ExpressionSpecification(o => o.Severety != "Hidden"));
                if (returnedDiagnostic.Count() == 0)
                    return Json(OkMessage);
                else
                    return PartialView("ReportPartial", _mapper.ToReportViewModel(returnedDiagnostic));
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
    }
}