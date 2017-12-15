using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayer.Intefaces
{
    public interface ISolutionFactory
    {
        CustomSolution Create(string name, string assemblyName);
        void AddDocument(string name, SourceText text, ref CustomSolution solution);
        SourceText GetSourceText(string text, Encoding encoding = null, SourceHashAlgorithm checksumAlgorithm = SourceHashAlgorithm.Sha1);
    }
}
