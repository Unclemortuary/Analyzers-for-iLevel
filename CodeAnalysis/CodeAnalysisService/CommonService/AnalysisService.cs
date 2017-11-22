using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeAnalysisService.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace CodeAnalysisService.CommonService
{
    public interface IDiagnosticService
    {
        IEnumerable<string> GetCompilationDiagnostic(string[] sources);
    }

    public class AnalysisService : IDiagnosticService
    {
        private readonly ISolutionCreator _solutionCreator;

        public AnalysisService(ISolutionCreator solutionCreator)
        {
            _solutionCreator = solutionCreator;
        }

        public IEnumerable<string> GetCompilationDiagnostic(string[] sources)
        {
            List<string> compilationDiagnostics = new List<string>();
            var syntaxTrees = _solutionCreator.GetSyntaxTrees(sources);
            var diagnostics = _solutionCreator.GetCompilation(syntaxTrees, null).GetDiagnostics();
            return FormatDiagnostics(SortDiagnostics(diagnostics, DiagnosticSeverity.Error));
        }

        private Diagnostic[] SortDiagnostics(IEnumerable<Diagnostic> diagnostics, DiagnosticSeverity severetyType)
        {
            return diagnostics.Where(d => d.Severity.Equals(severetyType)).
                OrderBy(d => d.Location.SourceSpan.Start).ToArray();
        }

        public IEnumerable<string> FormatDiagnostics(params Diagnostic[] diagnostics)
        {
            var builder = new StringBuilder();
            List<string> sortedDiagnostic = new List<string>();
            for (int i = 0; i < diagnostics.Length; ++i)
            {
                builder.AppendLine("// " + diagnostics[i].ToString());

                var location = diagnostics[i].Location;
                sortedDiagnostic.Add(builder.ToString());
                builder.Clear();
            }
            return sortedDiagnostic;
        }
    }


}