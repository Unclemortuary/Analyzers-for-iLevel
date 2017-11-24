﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

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
            List<string> compilationDiagnostics = new List<string>();
            var diagnostics = compilation.GetDiagnostics();
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

            foreach (var diagnostic in diagnostics)
            {
                builder.AppendLine("// " + diagnostic.ToString());

                var location = diagnostic.Location;
                sortedDiagnostic.Add(builder.ToString());
                builder.Clear();
            }
            
            return sortedDiagnostic;
        }
    }


}