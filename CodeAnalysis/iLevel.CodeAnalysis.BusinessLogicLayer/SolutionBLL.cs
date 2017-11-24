using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace CodeAnalysis.BusinessLogicLayer
{
    public interface ISolutionCreator
    {
        IEnumerable<SyntaxTree> GetSyntaxTrees(Dictionary<string, string> sources);
        CSharpCompilation GetCompilation(IEnumerable<SyntaxTree> syntaxTrees, string assemblyName);
    }

    public class SolutionBLL : ISolutionCreator
    {

        private readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
        private readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        private readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);


        private string _defaultFilePrefix = "Service";
        private string _cSharpDefaultFileExt = "cs";
        private string _defaultProjectName = "TestProject";

        public string FilePrefix { get { return _defaultFilePrefix; } set { _defaultFilePrefix = value; } }
        public string FileExt { get { return _cSharpDefaultFileExt; } set { _cSharpDefaultFileExt = value; } }
        public string ProjectName { get { return _defaultProjectName; } set { _defaultProjectName = value; } }


        public IEnumerable<SyntaxTree> GetSyntaxTrees(Dictionary<string, string> sources)
        {
            List<SyntaxTree> list = new List<SyntaxTree>(sources.Count);

            foreach (var fileName in sources.Keys)
            {
                var stringText = SourceText.From(sources[fileName]);
                list.Add(SyntaxFactory.ParseSyntaxTree(stringText, null, fileName));
            }

            return list;
        }

        public CSharpCompilation GetCompilation(IEnumerable<SyntaxTree> syntaxTrees, string assemblyName)
        {
            string assembly = assemblyName ?? _defaultProjectName;
            return CSharpCompilation.Create(assembly)
                .AddReferences(CorlibReference, SystemCoreReference, CSharpSymbolsReference, CodeAnalysisReference)
                .AddSyntaxTrees(syntaxTrees);
        }
    }
}