﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces;

namespace iLevel.CodeAnalysis.BusinessLogicLayer
{
    public class SolutionBLL : ISolutionCreator
    {
        private string _defaultProjectName = "iLevelProject";
        private string _defaultAssemblyName = "iLevelAssembly";

        private readonly ICustomSyntaxFactory _customSyntaxFactory;
        private readonly ICustomSolutionFactory _customSolutionFactory;

        private readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
        private readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        private readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);

        public string ProjectName { get { return _defaultProjectName; } }
        public string AssemblyName { get { return _defaultAssemblyName; } }

        public SolutionBLL(ICustomSyntaxFactory factory, ICustomSolutionFactory customSolutionFactory)
        {
            if (factory != null)
                _customSyntaxFactory = factory;
            _customSolutionFactory = customSolutionFactory;
        }

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

        public Project GetProject(Dictionary<string, string> sources, string projectName = null)
        {
            projectName = projectName ?? _defaultProjectName;
            ProjectId id = ProjectId.CreateNewId(projectName);

            CustomSolution solution = _customSolutionFactory
                .CreateWithProject(id,
                projectName,
                _defaultAssemblyName,
                new MetadataReference[] { CorlibReference, SystemCoreReference, CSharpSymbolsReference, CodeAnalysisReference });
                
            foreach (var source in sources)
            {
                solution = _customSolutionFactory.AddDocument(DocumentId.CreateNewId(id, source.Key), source.Key, _customSyntaxFactory.GetSourceText(source.Value), solution);
            }

            return solution.Projects.First();
        }

        public CSharpCompilation GetCompilation(IEnumerable<SyntaxTree> syntaxTrees, string assemblyName)
        {
            string assembly = assemblyName ?? _defaultAssemblyName;
            if (syntaxTrees.Count() == 0)
                return null;

            return _customSyntaxFactory.Create(assembly, 
                syntaxTrees, 
                new MetadataReference[] { CorlibReference, SystemCoreReference, CSharpSymbolsReference, CodeAnalysisReference });
        }
    }
}