using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using iLevel.CodeAnalysis.AnalyzersAccesLayer.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Moq;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayerTests.Common
{
    static class CompilationCreater
    {
        public static CSharpCompilation CreateAnyCSharpCompilation(string sourceText)
        {
            return CSharpCompilation.Create(It.IsAny<string>(),
                new List<SyntaxTree> { SyntaxFactory.ParseSyntaxTree(SourceText.From(sourceText)) })
                .AddReferences(ReferenceResources.metadataReferences);
        }
    }
}
