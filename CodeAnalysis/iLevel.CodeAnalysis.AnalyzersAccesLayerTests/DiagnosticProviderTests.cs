using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iLevel.CodeAnalysis.AnalyzersAccesLayer;
using iLevel.CodeAnalysis.AnalyzersAccesLayer.Interfaces;
using iLevel.CodeAnalysis.BusinessLogicLayer.Specification;
using iLevel.CodeAnalysis.BusinessLogicLayer.DTO;
using Moq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using System.Threading;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayerTests
{
    public abstract class CustomSourceText : SourceText
    {
        public override char this[int position] => default(char);

        public override Encoding Encoding => default(Encoding);

        public override int Length => default(int);

        public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) { }
    }

    [TestClass]
    public class DiagnosticProviderTests
    {
        Mock<ISyntaxFactory> _syntaxFactory = new Mock<ISyntaxFactory>();
        Mock<ISpecification> _specificationMock = new Mock<ISpecification>();
        Mock<IMapper> _mapperMock = new Mock<IMapper>();
        DiagnosticProvider _classUnderTest;

        [TestInitialize]
        public void Setup()
        {

        }

        [TestCleanup]
        public void Cleanup()
        {
            _syntaxFactory.Reset();
            _mapperMock.Reset();
            _classUnderTest = null;
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetDiagnosticTest_InputNullCollection_ThrowsNullReferenceException()
        {
            _classUnderTest = new DiagnosticProvider(_syntaxFactory.Object, _mapperMock.Object);
            _classUnderTest.GetDiagnostic(null, new HashSet<DiagnosticAnalyzer>(), _specificationMock.Object);
        }

        [TestMethod]
        public void GetDiagnosticTest_Input3ItemsCollection_SyntaxFactoryMethodsCalls3Times()
        {
            var anyCompilation = CSharpCompilation.Create(It.IsAny<string>());


            _mapperMock.Setup(
                x => x.ToReportDTO(It.IsAny<IEnumerable<Diagnostic>>())).Returns(new List<ReportDTO>());

            _specificationMock.Setup(
                x => x.IsStatisfiedBy(It.IsAny<ReportDTO>())).Returns(It.IsAny<bool>());
            _syntaxFactory.Setup(
                x => x.CreateCompilation(It.IsAny<string>(), 
                It.IsAny<IEnumerable<SyntaxTree>>(), 
                It.IsAny<IEnumerable<MetadataReference>>(), 
                It.IsAny<CSharpCompilationOptions>())).Returns(anyCompilation);

            _classUnderTest = new DiagnosticProvider(_syntaxFactory.Object, _mapperMock.Object);

            _classUnderTest.GetDiagnostic(new List<SourceFileDTO> { new SourceFileDTO(), new SourceFileDTO(), new SourceFileDTO() }, 
                It.IsAny<HashSet<DiagnosticAnalyzer>>(), 
                _specificationMock.Object);

            _syntaxFactory.Verify(
                x => x.GetSourceText(It.IsAny<string>(), It.IsAny<Encoding>(), It.IsAny<SourceHashAlgorithm>()), Times.Exactly(3));
            _syntaxFactory.Verify(
                x => x.ParseSyntaxTree(It.IsAny<SourceText>(), It.IsAny<ParseOptions>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [TestMethod]
        public void GetDiagnosticTest_Input2Concrete()
        {
            string testText = @"
class Test {
    public static void Main(string[] args) { }
}";
            var anyCompilation = CSharpCompilation.Create(It.IsAny<string>(), new List<SyntaxTree> { SyntaxFactory.ParseSyntaxTree(SourceText.From(testText)) });
            var anyCompilationWithAnalyzers = anyCompilation.WithAnalyzers(new List<DiagnosticAnalyzer> { Mock.Of<DiagnosticAnalyzer>() }.ToImmutableArray());
            var inputAnalyzers = new HashSet<DiagnosticAnalyzer>();
            var inputDTO = new SourceFileDTO();
            var expectedDTO = new ReportDTO();
            var sourceTextMock = new Mock<CustomSourceText>() { DefaultValue = DefaultValue.Empty };

            var test = sourceTextMock.Object;

            _syntaxFactory.Setup(
                x => x.GetSourceText(inputDTO.Text, It.IsAny<Encoding>(), It.IsAny<SourceHashAlgorithm>())).Returns(sourceTextMock.Object);
            _syntaxFactory.Setup(
                x => x.ParseSyntaxTree(sourceTextMock.Object, It.IsAny<ParseOptions>(), inputDTO.Name, It.IsAny<CancellationToken>()));
            _syntaxFactory.Setup(
                x => x.CreateCompilation(_classUnderTest.AssemblyName,
                It.IsAny<IEnumerable<SyntaxTree>>(),
                It.IsAny<IEnumerable<MetadataReference>>(),
                It.IsAny<CSharpCompilationOptions>())).Returns(anyCompilation);
            _syntaxFactory.Setup(
                x => x.CreateCompilationWithAnalyzers(anyCompilation, inputAnalyzers));

            _mapperMock.Setup(
                x => x.ToReportDTO(It.IsAny<IEnumerable<Diagnostic>>())).Returns(new List<ReportDTO> { expectedDTO });

            _specificationMock.Setup(
                x => x.IsStatisfiedBy(expectedDTO));

            _classUnderTest = new DiagnosticProvider(_syntaxFactory.Object, _mapperMock.Object);

            _classUnderTest.GetDiagnostic(new List<SourceFileDTO> { inputDTO },
                inputAnalyzers,
                _specificationMock.Object);

            _syntaxFactory.VerifyAll();
            _specificationMock.VerifyAll();
        }
    }
}
