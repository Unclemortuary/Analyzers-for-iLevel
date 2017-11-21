using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.CodeAnalysis.Diagnostics;
using iLevel.ViewPoint.CodeAnalysis.BestPractices;
using iLevel.CodeAnalysis.BestPractices;


namespace CodeAnalysisService.CommonService
{
    public class AnalyzersSet
    {
        private static  AnalyzersSet instance = null;

        private Dictionary<string, DiagnosticAnalyzer> _analyzers = new Dictionary<string, DiagnosticAnalyzer>();

        private AnalyzersSet()
        {
            _analyzers.Add("ILVL0001", new ArgumentTrailingUnderscoreAnalyzer());
            _analyzers.Add("ILVL0002", new ServiceMethodInLoopExecutionAnalyzer());
        }

        public static AnalyzersSet Init()
        {
            if(instance == null)
            {
                instance = new AnalyzersSet();
            }

            return instance;
        }

        public DiagnosticAnalyzer GetAnalyzerById(string id)
        {
            DiagnosticAnalyzer analyzer;
            if (_analyzers.TryGetValue(id, out analyzer))
                return analyzer;
            else
                throw new ArgumentException("Analyzer with given Id is not exist");
        }
    }
}