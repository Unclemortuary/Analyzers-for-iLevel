using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayer.Intefaces
{
    public interface IDiagnosticProvider
    {
        Report GetDiagnostic();
    }
}
