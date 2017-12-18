using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using iLevel.CodeAnalysis.AnalyzersAccesLayer.Interfaces;
using iLevel.CodeAnalysis.BusinessLogicLayer.DTO;
using iLevel.CodeAnalysis.AnalyzersAccesLayer.Infrastructure;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayer
{
    class DiagnosticProvider : IDiagnosticProvider
    {
        private readonly ISyntaxFactory _syntaxFactory;
        private string _projectName = null;
        private string _assemblyName = null;

        public string ProjectName => _projectName ?? "iLevelProject";
        public string AssemblyName => _assemblyName ?? "iLevel";

        public DiagnosticProvider(ISyntaxFactory syntaxFactory)
        {
            _syntaxFactory = syntaxFactory;
        }

        public IEnumerable<ReportDTO> GetDiagnostic(IEnumerable<SourceFileDTO> sources, HashSet<DiagnosticAnalyzer> analyzers)
        {
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            List<Diagnostic> result = new List<Diagnostic>();

            foreach (var source in sources)
            {
                var sourceText = _syntaxFactory.GetSourceText(source.Text);
                syntaxTrees.Add(_syntaxFactory.ParseSyntaxTree(text: sourceText, path: source.Name));
            }

            var compilationWithoutAnalyzers = _syntaxFactory.CreateCompilation(AssemblyName, syntaxTrees);
            result = compilationWithoutAnalyzers.GetDiagnostics().ToList();
            if (result.Count == 0)
            {
                var compilationWithAnalyzers = _syntaxFactory.CreateCompilationWithAnalyzers(compilationWithoutAnalyzers, analyzers);
                result = compilationWithAnalyzers.GetAllDiagnosticsAsync().Result.ToList();
            }
            return ToDTO(result.AsReadOnly());
        }

        internal IEnumerable<ReportDTO> ToDTO(IEnumerable<Diagnostic> diagnostic)
        {
            List<ReportDTO> result = new List<ReportDTO>();
            foreach (var report in diagnostic)
            {
                result.Add(Mapper.Map(report));
            }
            return result;
        }
        
    }
}
