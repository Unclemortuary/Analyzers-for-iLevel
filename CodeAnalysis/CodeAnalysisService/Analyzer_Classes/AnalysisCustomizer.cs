using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalysisService.Analyzer_Classes
{
    public class AnalysisCustomizer
    {

        private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
        private static readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        private static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);

        internal static string DefaultFilePathPrefix = "Any";
        internal static string CSharpDefaultFileExt = "cs";
        internal static string VisualBasicDefaultExt = "vb";

        private string _projectName = "SomeProject";
        private string _analyzerId = "ILVL0001";
        

        public string ProjectName { get { return _projectName; } set { _projectName = value; } }
        public string AnalyzerId
        {
            get { return _analyzerId; }
            set
            {
                try
                {
                    AnalyzersSet.instance.Init().GetAnalyzerById(value);
                    _analyzerId = value;
                }
                catch (ArgumentException)
                {
                    Console.WriteLine(Environment.NewLine + "Attempt to use non existing Analyzer Id");
                }
            }
        }

        AnalysisCustomizer()
        {

        }
    }
}