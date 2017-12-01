using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;


namespace CodeAnalysis.BusinessLogicLayer
{
    public interface ISolutionCreator
    {
        IEnumerable<SyntaxTree> GetSyntaxTrees(Dictionary<string, string> sources);
        CSharpCompilation GetCompilation(IEnumerable<SyntaxTree> syntaxTrees, string assemblyName);
    }

    public class SolutionBLL : ISolutionCreator
    {
        private readonly ICustomSyntaxFactory _customSyntaxFactory;

        private readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
        private readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        private readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);

        public SolutionBLL(ICustomSyntaxFactory factory)
        {
            if (factory != null)
                _customSyntaxFactory = factory;
        }

        private string _defaultFilePrefix = "Service";
        private string _cSharpDefaultFileExt = "cs";
        private string _defaultProjectName = "DefaultProject";

        public string FilePrefix { get { return _defaultFilePrefix; } set { _defaultFilePrefix = value; } }
        public string FileExt { get { return _cSharpDefaultFileExt; } set { _cSharpDefaultFileExt = value; } }
        public string ProjectName { get { return _defaultProjectName; } set { _defaultProjectName = value; } }


        public IEnumerable<SyntaxTree> GetSyntaxTrees(Dictionary<string, string> sources)
        {
            List<SyntaxTree> list = new List<SyntaxTree>(sources.Count);

            foreach (var fileName in sources.Keys)
            {
                var stringText = _customSyntaxFactory.GetSourceText(sources[fileName]);
                list.Add(_customSyntaxFactory.ParseSyntaxTree(text: stringText, path: fileName));
            }

            return list;
        }

        public CSharpCompilation GetCompilation(IEnumerable<SyntaxTree> syntaxTrees, string assemblyName)
        {
            string assembly = assemblyName ?? _defaultProjectName;
            if (syntaxTrees.Count() == 0)
                return null;

            return _customSyntaxFactory.Create(assembly, 
                syntaxTrees, 
                new MetadataReference[] { CorlibReference, SystemCoreReference, CSharpSymbolsReference, CodeAnalysisReference });
        }
    }
}