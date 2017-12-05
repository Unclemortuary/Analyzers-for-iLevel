using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces;

namespace iLevel.CodeAnalysis.BusinessLogicLayer
{
    public class SolutionBLL : ISolutionCreator
    {
        private readonly ICustomSyntaxFactory _customSyntaxFactory;
        private readonly ICustomSolutionFactory _customSolutionFactory;

        private readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
        private readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        private readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);

        public SolutionBLL(ICustomSyntaxFactory factory, ICustomSolutionFactory customSolutionFactory)
        {
            if (factory != null)
                _customSyntaxFactory = factory;
            _customSolutionFactory = customSolutionFactory;
        }

        private string _defaultFilePrefix = "Service";
        private string _cSharpDefaultFileExt = "cs";
        private string _defaultProjectName = "DefaultProject";

        public string FilePrefix { get { return _defaultFilePrefix; } set { _defaultFilePrefix = value; } }
        public string FileExt { get { return _cSharpDefaultFileExt; } set { _cSharpDefaultFileExt = value; } }
        public string ProjectName { get { return _defaultProjectName; } set { _defaultProjectName = value; } }


        public IEnumerable<SyntaxTree> GetSyntaxTrees(Dictionary<string, string> sources)
        {
            List<SyntaxTree> list = new List<SyntaxTree>(sources.Count);

            foreach (var fileName in sources.Keys)
            {
                var stringText = _customSyntaxFactory.GetSourceText(sources[fileName]);
                list.Add(_customSyntaxFactory.ParseSyntaxTree(text: stringText, path: fileName));
            }

            return list;
        }

        public Project GetProject(Dictionary<string, string> sources, string projectName)
        {
            ProjectId id = ProjectId.CreateNewId(projectName);

            CustomSolution solution = _customSolutionFactory
                .CreateWithProject(id,
                projectName,
                _defaultProjectName,
                new MetadataReference[] { CorlibReference, SystemCoreReference, CSharpSymbolsReference, CodeAnalysisReference });
                
            foreach (var source in sources)
            {
                solution = _customSolutionFactory.AddDocument(DocumentId.CreateNewId(id, source.Key), source.Key, _customSyntaxFactory.GetSourceText(source.Value), solution);
            }

            return solution.Projects.First();
        }

        public CSharpCompilation GetCompilation(IEnumerable<SyntaxTree> syntaxTrees, string assemblyName)
        {
            string assembly = assemblyName ?? _defaultProjectName;
            if (syntaxTrees.Count() == 0)
                return null;

            return _customSyntaxFactory.Create(assembly, 
                syntaxTrees, 
                new MetadataReference[] { CorlibReference, SystemCoreReference, CSharpSymbolsReference, CodeAnalysisReference });
        }
    }
}