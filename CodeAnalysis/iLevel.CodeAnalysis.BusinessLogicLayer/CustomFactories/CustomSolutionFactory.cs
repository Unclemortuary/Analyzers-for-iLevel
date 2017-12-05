using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces;


namespace iLevel.CodeAnalysis.BusinessLogicLayer.CustomFactories
{
    public class CustomSolutionFactory : ICustomSolutionFactory
    {
        public CustomSolution AddDocument(DocumentId id, string name, SourceText text, CustomSolution solution)
        {
            solution.Solution =  solution.Solution.AddDocument(id, name, text);
            return solution;
        }

        public CustomSolution AddProject(ProjectId id, string name, string assemblyName)
        {
            var custom = new CustomSolution(new AdhocWorkspace().CurrentSolution);
            custom.Solution = custom.Solution.AddProject(id, name, assemblyName, LanguageNames.CSharp);
            return custom;
        }

        public CustomSolution Create()
        {
            return new CustomSolution(new AdhocWorkspace().CurrentSolution);
        }

        public CustomSolution CreateWithProject(ProjectId id, string name, string assemblyName, IEnumerable<MetadataReference> metadataReference)
        {
            var solutionWithProject = new CustomSolution(new AdhocWorkspace().CurrentSolution).AddProject(id, name, assemblyName);
            solutionWithProject.Solution = solutionWithProject.Solution.AddMetadataReferences(id, metadataReference);
            return solutionWithProject;
        }
    }


}
