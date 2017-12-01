using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalysis.BusinessLogicLayer
{
    public interface IDiagnosticService
    {
        IEnumerable<string> GetCompilationDiagnostic(CSharpCompilation compilation);
    }

    public class DiagnosticBLL : IDiagnosticService
    {

        public IEnumerable<string> GetCompilationDiagnostic(CSharpCompilation compilation)
        {
            compilation = compilation ?? throw new ArgumentNullException(nameof(compilation));
            var diagnostics = compilation.GetDiagnostics();
            return FormatDiagnostics(SortDiagnostics(diagnostics, DiagnosticSeverity.Error));
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