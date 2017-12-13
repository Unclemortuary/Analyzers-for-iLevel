using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces
{
    public interface ICustomSolutionFactory
    {
        CustomSolution Create(string name, string assemblyName);
        void AddDocument(string name, SourceText text, ref CustomSolution solution);
    }
}
