using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.Infrastructure
{
    class ReferenceResources
    {
        private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
        private static readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        private static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);

        public static MetadataReference[] metadataReferences = new MetadataReference[]
        {
            CodeAnalysisReference,
            CorlibReference,
            CSharpSymbolsReference,
            SystemCoreReference
        };
    }
}
