using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeAnalysisService.Models;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;

namespace CodeAnalysisService.Analyzer_Classes
{
    public static class Analyzator
    {
        public static ServiceDiagnosticResult AnalyzeSolution(Document solution, AnalysisCustomizer settings)
        {
            return null;
        }
    }
}