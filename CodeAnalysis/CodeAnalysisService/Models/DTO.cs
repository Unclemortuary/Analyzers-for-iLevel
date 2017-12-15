using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;

namespace CodeAnalysisService.Models
{
    public class DTO
    {
        Dictionary<string, string> Sources { get; set; }
        HashSet<DiagnosticAnalyzer> Analyzers { get; set; }
    }
}