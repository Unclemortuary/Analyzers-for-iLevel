﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces;
using iLevel.CodeAnalysis.BusinessLogicLayer.Infrastructure;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.CustomFactories
{
    class CustomSolutionFactory : ICustomSolutionFactory
    {
        public CustomSolution Create(string name, string assemblyName)
        {
            CustomSolution solution = new CustomSolution(new AdhocWorkspace().CurrentSolution);
            var projectId = ProjectId.CreateNewId(name);
            solution.Solution = solution.Solution.AddProject(projectId, name, assemblyName, LanguageNames.CSharp)
                .AddMetadataReferences(projectId, ReferenceResources.metadataReferences);
            return solution;
        }

        public void AddDocument(string name, SourceText text, ref CustomSolution solution)
        {
            solution.Solution = solution.Solution.AddDocument(DocumentId.CreateNewId(solution.ProjectId), name, text);
        }
    }
}
