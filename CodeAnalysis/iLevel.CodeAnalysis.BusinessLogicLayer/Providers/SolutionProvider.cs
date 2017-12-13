using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.Providers
{
    class SolutionProvider : ISolutionProvider
    {
        private string _defaultProjectName = "iLevelProject";
        private string _defaultAssemblyName = "iLevelAssembly";

        private readonly ICustomSyntaxFactory _customSyntaxFactory;
        private readonly ICustomSolutionFactory _customSolutionFactory;

        public string ProjectName => _defaultProjectName;
        public string AssemblyName => _defaultAssemblyName;

        public SolutionProvider(ICustomSyntaxFactory factory, ICustomSolutionFactory customSolutionFactory)
        {
            _customSyntaxFactory = factory;
            _customSolutionFactory = customSolutionFactory;
        }

        public IEnumerable<SyntaxTree> GetSyntaxTrees(Dictionary<string, string> sources)
        {
            List<SyntaxTree> list = new List<SyntaxTree>(sources.Count);

            foreach (var file in sources)
            {
                var stringText = _customSyntaxFactory.GetSourceText(file.Value);
                list.Add(_customSyntaxFactory.ParseSyntaxTree(text: stringText, path: file.Key));
            }

            return list;
        }

        public Project GetProject(Dictionary<string, string> sources, string projectName = null)
        {
            projectName = projectName ?? _defaultProjectName;
            CustomSolution solution;
            _customSolutionFactory.Create(projectName, _defaultAssemblyName, out solution);

            foreach (var source in sources)
                _customSolutionFactory
                    .AddDocument(source.Key, _customSyntaxFactory.GetSourceText(source.Value), ref solution);

            return solution.Projects.First();
        }

        public CSharpCompilation GetCompilation(IEnumerable<SyntaxTree> syntaxTrees, string assemblyName)
        {
            string assembly = assemblyName ?? _defaultAssemblyName;
            if (syntaxTrees.Count() == 0)
                return null;

            return _customSyntaxFactory.Create(assembly, syntaxTrees);
        }
    }
}