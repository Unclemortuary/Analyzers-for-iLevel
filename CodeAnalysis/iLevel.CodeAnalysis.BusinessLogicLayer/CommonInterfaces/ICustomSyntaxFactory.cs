using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces
{
    public interface ICustomSyntaxFactory
    {
        SyntaxTree ParseSyntaxTree(SourceText text, ParseOptions options = null, string path = "", CancellationToken cancellationToken = default(CancellationToken));
        SourceText GetSourceText(string text, Encoding encoding = null, SourceHashAlgorithm checksumAlgorithm = SourceHashAlgorithm.Sha1);
        CSharpCompilation Create(string assemblyName, IEnumerable<SyntaxTree> syntaxTrees = null, IEnumerable<MetadataReference> references = null, CSharpCompilationOptions options = null);
    }
}
