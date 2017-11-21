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
        IEnumerable<CSharpSyntaxTree> GetSyntaxTrees(string[] sources);
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

        private string[] _sourceStrings;
        private string _language = LanguageNames.CSharp;

        public SolutionCreator(string[] inputStrings)
        {
            _sourceStrings = inputStrings;
        }

        public Document[] GetDocument()
        {
            return CreateDocuments(_sourceStrings, FilePrefix, ProjectName);
        }

        private Document[] CreateDocuments(string[] sources, string fileNamePrefix, string projectName)
        {
            var project = CreateProject(sources, fileNamePrefix, _defaultProjectName);
            var documents = project.Documents.ToArray();

            if (_sourceStrings.Length != documents.Length)
            {
                throw new SystemException("Amount of sources did not match amount of Documents created");
            }

            return documents;
        }

        private Project CreateProject(string[] sources, string fileNamePrefix, string projectName)
        {
            string fileExt = _cSharpDefaultFileExt;
            string language = _language;

            var projectId = ProjectId.CreateNewId(debugName: projectName);

            var solution = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, projectName, projectName, language)
                .AddMetadataReference(projectId, CorlibReference)
                .AddMetadataReference(projectId, SystemCoreReference)
                .AddMetadataReference(projectId, CSharpSymbolsReference)
                .AddMetadataReference(projectId, CodeAnalysisReference);

            int count = 0;
            foreach (var source in sources)
            {
                var newFileName = fileNamePrefix + count + "." + fileExt;
                var documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);
                solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
                count++;
            }
            return solution.GetProject(projectId);
        }

        public IEnumerable<CSharpSyntaxTree> GetSyntaxTrees(string[] sources)
        {
            return null;
        }
    }
}