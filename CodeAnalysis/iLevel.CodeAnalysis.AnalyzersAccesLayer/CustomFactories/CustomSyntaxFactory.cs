﻿using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using iLevel.CodeAnalysis.AnalyzersAccesLayer.Interfaces;
using iLevel.CodeAnalysis.AnalyzersAccesLayer.Infrastructure;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayer.CustomFactories
{
    class CustomSyntaxFactory : ISyntaxFactory
    {
        public CSharpCompilation CreateCompilation(string assemblyName, IEnumerable<SyntaxTree> syntaxTrees = null, IEnumerable<MetadataReference> references = null, CSharpCompilationOptions options = null)
        {
            return CSharpCompilation.Create(assemblyName, syntaxTrees, references, options).AddReferences(ReferenceResources.metadataReferences);
        }

        public CompilationWithAnalyzers CreateCompilationWithAnalyzers(CSharpCompilation compilation, IEnumerable<DiagnosticAnalyzer> analyzers)
        {
            ImmutableArray<DiagnosticAnalyzer> immutableAnalyzers = new List<DiagnosticAnalyzer>(analyzers).ToImmutableArray();
            return compilation.WithAnalyzers(immutableAnalyzers);
        }

        public SourceText GetSourceText(string text, Encoding encoding = null, SourceHashAlgorithm checksumAlgorithm = SourceHashAlgorithm.Sha1)
        {
            return SourceText.From(text, encoding, checksumAlgorithm);
        }

        public SyntaxTree ParseSyntaxTree(SourceText text, ParseOptions options = null, string path = "", CancellationToken cancellationToken = default(CancellationToken))
        {
            return SyntaxFactory.ParseSyntaxTree(text, options, path);
        }
    }
}
