using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces
{
    public interface IDiagnosticProvider
    {
        IEnumerable<string> GetCompilationDiagnostic(CSharpCompilation compilation);
        IEnumerable<string> GetCompilationDiagnostic(Project proj, ImmutableArray<DiagnosticAnalyzer> analyzers);
    }
}
