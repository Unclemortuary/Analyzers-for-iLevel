using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.Providers
{
    class DiagnosticProvider : IDiagnosticProvider
    {

        public IEnumerable<string> GetCompilationDiagnostic(CSharpCompilation compilation)
        {
            compilation = compilation ?? throw new ArgumentNullException(nameof(compilation));
            var diagnostics = compilation.GetDiagnostics();
            return FormatDiagnostics(SortDiagnostics(diagnostics, DiagnosticSeverity.Error));
        }

        public IEnumerable<string> GetCompilationDiagnostic(Project proj, ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            var compilation = proj.GetCompilationAsync().Result.WithAnalyzers(analyzers);
            var diagnostics = compilation.GetAllDiagnosticsAsync().Result;
            return FormatDiagnostics(SortDiagnostics(diagnostics, DiagnosticSeverity.Warning));
        }

        internal Diagnostic[] SortDiagnostics(IEnumerable<Diagnostic> diagnostics, DiagnosticSeverity severetyType)
        {
            return diagnostics.Where(d => d.Severity.Equals(severetyType)).
                OrderBy(d => d.Location.SourceSpan.Start).ToArray();
        }

        public IEnumerable<string> FormatDiagnostics(params Diagnostic[] diagnostics)
        {
            string diagnosticString = null;
            
            List<string> sortedDiagnostic = new List<string>();

            foreach (var diagnostic in diagnostics)
            {
                diagnosticString = "// " + diagnostic.ToString() + Environment.NewLine;
                sortedDiagnostic.Add(diagnosticString);
            }
            
            return sortedDiagnostic;
        }
    }


}