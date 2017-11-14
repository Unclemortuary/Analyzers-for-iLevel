using CodeAnalysisService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CodeAnalysisService.Analyzer_Classes;
using Microsoft.CodeAnalysis;

namespace CodeAnalysisService.Controllers
{
    public class AnalyzerController : ApiController
    {
        private string testString = @"
class Class
{
    public void Method(string arg_) { }
}";

        private SolutionCreator creator;
        private Diagnostic[] results;

        public string Get()
        {
            Preparations();
            var sources = creator.GetDocuments();
            var usingAnalizer = AnalyzersSet.Init().GetAnalyzerById(creator.AnalyzerId);
            results = Analyzer.AnalyzeSolution(usingAnalizer, sources);
            var returnableResult = Analyzer.FormatDiagnostics(usingAnalizer, results);
            return returnableResult;
        }

        //This method must be substitude to POST later
        private void Preparations()
        {
            creator = new SolutionCreator(new string[] { testString });
        }
    }
}
