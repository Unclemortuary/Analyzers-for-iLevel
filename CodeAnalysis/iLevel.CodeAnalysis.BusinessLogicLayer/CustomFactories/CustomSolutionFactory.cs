using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.CustomFactories
{
    class CustomSolutionFactory : ICustomSolutionFactory
    {
        public void Create(string name, string assemblyName, out CustomSolution solution)
        {
            solution = new CustomSolution(new AdhocWorkspace().CurrentSolution);
            var projectId = ProjectId.CreateNewId(name);
            solution.Solution = solution.Solution.AddProject(projectId, name, assemblyName, LanguageNames.CSharp)
                .AddMetadataReferences(projectId, ReferenceResources.metadataReferences);
        }

        public void AddDocument(string name, SourceText text, ref CustomSolution solution)
        {
            solution.Solution = solution.Solution.AddDocument(DocumentId.CreateNewId(solution.ProjectId), name, text);
        }
    }
}
