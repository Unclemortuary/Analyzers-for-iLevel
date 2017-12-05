using System.Collections.Generic;
using Microsoft.CodeAnalysis.Diagnostics;
using iLevel.ViewPoint.CodeAnalysis.BestPractices;
using iLevel.CodeAnalysis.BestPractices;

namespace CodeAnalysisService
{
    public class AnalyzerConfig
    {
        public static void RegisterAnalyzers(HashSet<DiagnosticAnalyzer> analyzers)
        {
            analyzers.Add(new ArgumentTrailingUnderscoreAnalyzer());
            analyzers.Add(new ServiceMethodInLoopExecutionAnalyzer());
        }
    }
}