using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CodeAnalysisService.CommonService
{
    public interface ISolutionCreator
    {
        IEnumerable<SyntaxTree> GetSyntaxTrees(string[] sources);
    }

    public class SolutionCreator : ISolutionCreator
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

        private string _language = LanguageNames.CSharp;

        public IEnumerable<SyntaxTree> GetSyntaxTrees(string[] sources)
        {
            List<SyntaxTree> list = new List<SyntaxTree>(sources.Length);

            for (int i = 0; i < sources.Length; i++)
            {
                list.Add(CSharpSyntaxTree.ParseText(sources[i]));
            }

            return list;
        }
    }
}