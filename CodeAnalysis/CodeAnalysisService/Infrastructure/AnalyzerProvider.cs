using System.Collections.Generic;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalysisService.Infrastructure
{
    public static class AnalyzerProvider
    {
        public static HashSet<DiagnosticAnalyzer> Analyzers { get; }

        static AnalyzerProvider()
        {
            Analyzers = new HashSet<DiagnosticAnalyzer>();
        }
    }
}