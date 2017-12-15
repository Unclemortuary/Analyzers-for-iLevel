using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using iLevel.CodeAnalysis.AnalyzersAccesLayer.Intefaces;
using Microsoft.CodeAnalysis.Text;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayer
{
    class DiagnosticProvider : IDiagnosticProvider
    {
        private readonly ISolutionFactory _solutionFactrory;
        private string _projectName = null;
        private string _assemblyName = null;

        public string ProjectName => _projectName ?? "iLevelProject";
        public string AssemblyName => _assemblyName ?? "iLevel";

        public DiagnosticProvider(ISolutionFactory solutionFactory)
        {
            _solutionFactrory = solutionFactory;
        }

        private Report GetSpecifiedDiagnostic()
        {

        }

        private Project GetProject(DTO sourcesDTO)
        {
            Project projectUnderDiagnostic = new AdhocWorkspace().CurrentSolution.AddProject();
            var solution = _solutionFactrory.Create()
        }

        private DiagnosticResult[] GetUnsortedDiagnostic(DTO sourcesDTO)
        {
            
        }
    }
}
