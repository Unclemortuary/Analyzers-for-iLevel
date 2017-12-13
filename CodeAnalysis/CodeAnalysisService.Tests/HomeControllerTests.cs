using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using CodeAnalysisService.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalysisService.Tests
{
    [TestClass]
    public class HomeControllerTests
    {
        Mock<IDiagnosticProvider> _diagnosticServiceMock = new Mock<IDiagnosticProvider>();
        Mock<ISolutionProvider> _solutionCreatorMock = new Mock<ISolutionProvider>();
        Mock<HttpFileCollectionBase> _filesMock = new Mock<HttpFileCollectionBase>();
        Mock<HttpRequestBase> _requestMock = new Mock<HttpRequestBase>();
        Mock<HttpContextBase> _contextMock = new Mock<HttpContextBase>();
        Mock<HttpPostedFileBase> _normalFileMock = new Mock<HttpPostedFileBase>();
        Mock<HttpPostedFileBase> _wrongExtensionFileMock = new Mock<HttpPostedFileBase>();
        MemoryStream _testStream = new MemoryStream();
        Dictionary<string, string> _input;
        string _diagnosticReport;

        HomeController _controllerUnderTest;

        [TestInitialize]
        public void Setup()
        {
            //Setup input and output data
            string text = @"
new class A {
    int a;
}";
            _input = new Dictionary<string, string>() { ["first"] = text };
            _diagnosticReport = "Some message";

            //Setup dependencies mocks default behaviour
            _diagnosticServiceMock.Setup(
                ds => ds.GetCompilationDiagnostic(It.IsAny<CSharpCompilation>())).Returns(new List<string>());

            _solutionCreatorMock.Setup(
                sc => sc.GetCompilation(It.IsAny<IEnumerable<SyntaxTree>>(), It.IsAny<string>())).Returns(It.IsAny<CSharpCompilation>());

            _solutionCreatorMock.Setup(
                sc => sc.GetSyntaxTrees(It.IsAny<Dictionary<string, string>>())).Returns(new List<SyntaxTree>());

            //Setup HttpRequest mocks
            string fileName = "TestFile";

            _normalFileMock.Setup(f => f.FileName).Returns(fileName + ".cs");
            _normalFileMock.Setup(f => f.ToString()).Returns("file1");
            _normalFileMock.Setup(f => f.InputStream).Returns(_testStream);

            _wrongExtensionFileMock.Setup(f => f.FileName).Returns(fileName + ".jar");
            _wrongExtensionFileMock.Setup(f => f.ToString()).Returns("file2");
            _wrongExtensionFileMock.Setup(f => f.InputStream).Returns(_testStream);

            _filesMock.Setup(f => f.GetEnumerator()).Returns(new HttpPostedFileBase[] {_normalFileMock.Object, _wrongExtensionFileMock.Object }.GetEnumerator());
            _filesMock.Setup(f => f.Count).Returns(2);
            _filesMock.Setup(f => f["file1"]).Returns(_normalFileMock.Object);
            _filesMock.Setup(f => f["file2"]).Returns(_wrongExtensionFileMock.Object);

            _requestMock.Setup(r => r.Files).Returns(_filesMock.Object);
            _contextMock.Setup(c => c.Request).Returns(_requestMock.Object);

            //Setup tested controller
            _controllerUnderTest = new HomeController(_diagnosticServiceMock.Object, _solutionCreatorMock.Object);
            _controllerUnderTest.ControllerContext = new ControllerContext(_contextMock.Object, new RouteData(), _controllerUnderTest);
        }

        [TestCleanup]
        public void Clenup()
        {
            _diagnosticServiceMock.Reset();
            _solutionCreatorMock.Reset();
            _filesMock.Reset();
            _requestMock.Reset();
            _contextMock.Reset();
            _controllerUnderTest = null;
        }

        [TestMethod]
        public void GetCompilationDiagnostic_InputNullArg_ThrowsArgNullException()
        {
            Action result = () => _controllerUnderTest.GetCompilationDiagnostic(null);

            Assert.ThrowsException<ArgumentNullException>(result);
        }

        [TestMethod]
        public void GetCompilationDiagnostic_DiagnosticServiceCausesNullResult_ThrowsNullReferenceExc()
        {
            _diagnosticServiceMock.Reset();
            _diagnosticServiceMock.Setup(ds => ds.GetCompilationDiagnostic(It.IsAny<CSharpCompilation>())).Returns<IEnumerable<string>>(null);

            Action result = () => _controllerUnderTest.GetCompilationDiagnostic(new Dictionary<string, string>());

            Assert.ThrowsException<NullReferenceException>(result);
        }

        [TestMethod]
        public void GetCompilationDiagnostic_InputAnyDictionary_ReturnsAnyJsonResult()
        {
            var result = _controllerUnderTest.GetCompilationDiagnostic(new Dictionary<string, string>());

            Assert.IsInstanceOfType(result, typeof(JsonResult));
        }

        [TestMethod]
        public void GetCompilationDiagnostic_InputCertainDictionary_ReturnsDiagnostic()
        {
            SyntaxTree syntaxTree = Mock.Of<SyntaxTree>();
            CSharpCompilation compilation = default(CSharpCompilation);
            _solutionCreatorMock.Reset();
            _diagnosticServiceMock.Reset();
            _solutionCreatorMock.Setup(sc => sc.GetSyntaxTrees(_input)).Returns(new List<SyntaxTree>() { syntaxTree });
            _solutionCreatorMock.Setup(sc => sc.GetCompilation(new List<SyntaxTree>() { syntaxTree }, It.IsAny<string>())).Returns(compilation);
            _diagnosticServiceMock.Setup(ds => ds.GetCompilationDiagnostic(compilation)).Returns(new List<string>() { _diagnosticReport });

            var result = (List<string>) _controllerUnderTest.GetCompilationDiagnostic(_input).Data;

            Assert.IsTrue(result.Contains(_diagnosticReport));
        }

        [TestMethod]
        public void GetCompilationDiagnostic_InputNormalFiles_ReturnsJSONWithStringMessage()
        {
            Project project = new AdhocWorkspace().CurrentSolution.AddProject("TestName", "Name", LanguageNames.CSharp);

            _solutionCreatorMock.Setup(
                x => x.GetProject(_input, It.IsAny<string>())).Returns(project);
            _diagnosticServiceMock.Setup(
                x => x.GetCompilationDiagnostic(project, It.IsAny<ImmutableArray<DiagnosticAnalyzer>>()))
                .Returns(new List<string>());

            var result = (string) _controllerUnderTest.GetCompilationDiagnostic(_input).Data;

            Assert.AreEqual(result, _controllerUnderTest.OkMessage);
        }

        [TestMethod]
        public void GetCompilationDiagnostic_InputFilesWithWarnings_ReturnsListOfAnalyzersDiagnostic()
        {
    
            Project project = new AdhocWorkspace().CurrentSolution.AddProject("TestName", "Name", LanguageNames.CSharp);

            _solutionCreatorMock.Setup(
                x => x.GetProject(_input, It.IsAny<string>())).Returns(project);
            _diagnosticServiceMock.Setup(
                x => x.GetCompilationDiagnostic(project, It.IsAny<ImmutableArray<DiagnosticAnalyzer>>()))
                .Returns(new List<string>() { _diagnosticReport });

            var result = (List<string>) _controllerUnderTest.GetCompilationDiagnostic(_input).Data;

            Assert.IsTrue(result.Contains(_diagnosticReport));
        }

        [TestMethod]
        public void Upload_InputEmptyRequest_Returns204StatusCode()
        {
            _filesMock.Reset();
            _filesMock.Setup(f => f.Count).Returns(0);

            var result = (HttpStatusCodeResult) _controllerUnderTest.Upload();

            Assert.AreEqual(204, result.StatusCode);
        }

        [TestMethod]
        public void Upload_InputFileWithWrongExtension_Returns400StatusCode()
        {
            var result = (HttpStatusCodeResult) _controllerUnderTest.Upload();

            Assert.AreEqual(400, result.StatusCode);
        }

        [TestMethod]
        public void Upload_InputSources_ReturnsAnyJson()
        {
            _wrongExtensionFileMock.Reset();

            _wrongExtensionFileMock.Setup(f => f.FileName).Returns("AnotherTestFile" + ".cs");
            _wrongExtensionFileMock.Setup(f => f.InputStream).Returns(new MemoryStream());
            _wrongExtensionFileMock.Setup(f => f.ToString()).Returns("file2");

            var result = _controllerUnderTest.Upload();

            Assert.IsInstanceOfType(result, typeof(JsonResult));

            _wrongExtensionFileMock.Reset();
        }

        [TestMethod]
        public void Upload_InputSourcesCausesNullReferenceException_ThrowsNullReferenceExeption()
        {
            _diagnosticServiceMock.Reset();
            _diagnosticServiceMock.Setup(ds => ds.GetCompilationDiagnostic(It.IsAny<CSharpCompilation>())).Returns<IEnumerable<string>>(null);

            _wrongExtensionFileMock.Reset();

            _wrongExtensionFileMock.Setup(f => f.FileName).Returns("AnotherTestFile" + ".cs");
            _wrongExtensionFileMock.Setup(f => f.InputStream).Returns(new MemoryStream());
            _wrongExtensionFileMock.Setup(f => f.ToString()).Returns("file2");

            Action result = () => _controllerUnderTest.Upload();

            Assert.ThrowsException<NullReferenceException>(result);

            _wrongExtensionFileMock.Reset();
        }

        [TestMethod]
        public void Index_ReturnedIndexView()
        {
            var result = (ViewResult) _controllerUnderTest.Index();

            Assert.AreEqual("Index", result.ViewName);
        }
    }
}