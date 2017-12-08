using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces
{
    public interface ICustomSolutionFactory
    {
        void Create(string name, string assemblyName, out CustomSolution solution);
        void AddDocument(string name, SourceText text, ref CustomSolution solution);
    }
}
