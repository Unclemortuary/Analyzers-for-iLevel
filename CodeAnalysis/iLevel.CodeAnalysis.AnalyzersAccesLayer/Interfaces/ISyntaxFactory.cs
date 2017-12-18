using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Diagnostics;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayer.Interfaces
{
    public interface ISyntaxFactory
    {
        SyntaxTree ParseSyntaxTree(SourceText text, ParseOptions options = null, string path = "", CancellationToken cancellationToken = default(CancellationToken));
        SourceText GetSourceText(string text, Encoding encoding = null, SourceHashAlgorithm checksumAlgorithm = SourceHashAlgorithm.Sha1);
        CSharpCompilation CreateCompilation(string assemblyName, IEnumerable<SyntaxTree> syntaxTrees = null, IEnumerable<MetadataReference> references = null, CSharpCompilationOptions options = null);
        CompilationWithAnalyzers CreateCompilationWithAnalyzers(CSharpCompilation compilation, IEnumerable<DiagnosticAnalyzer> analyzers);
    }
}
