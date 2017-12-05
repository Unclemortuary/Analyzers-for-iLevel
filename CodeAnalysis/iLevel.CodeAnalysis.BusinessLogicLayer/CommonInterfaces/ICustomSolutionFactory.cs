using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces
{
    public interface ICustomSolutionFactory
    {
        CustomSolution Create();
        CustomSolution CreateWithProject(ProjectId id, string name, string assemblyName, IEnumerable<MetadataReference> metadataReference);
        CustomSolution AddProject(ProjectId id, string name, string assemblyName);
        CustomSolution AddDocument(DocumentId id, string name, SourceText text, CustomSolution solution);
    }
}
