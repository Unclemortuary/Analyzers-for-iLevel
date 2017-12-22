using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using iLevel.CodeAnalysis.BusinessLogicLayer.Specification;
using iLevel.CodeAnalysis.AnalyzersAccesLayer.Interfaces;
using iLevel.CodeAnalysis.BusinessLogicLayer.DTO;
using iLevel.CodeAnalysis.AnalyzersAccesLayer.Infrastructure;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayer
{
    class DiagnosticProvider : IDiagnosticProvider
    {
        private readonly ISyntaxFactory _syntaxFactory;
        private readonly IMapper _mapper;

        public string ProjectName => "iLevelProject";
        public string AssemblyName => "iLevel";

        public DiagnosticProvider(ISyntaxFactory syntaxFactory, IMapper mapper)
        {
            _syntaxFactory = syntaxFactory;
            _mapper = mapper;
        }

        public IEnumerable<ReportDTO> GetDiagnostic(IEnumerable<SourceFileDTO> sources, HashSet<DiagnosticAnalyzer> analyzers, ISpecification specification)
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
            return _mapper.ToReportDTO(result.AsReadOnly()).Where(r => specification.IsStatisfiedBy(r)).ToList();
        }
    }
}
