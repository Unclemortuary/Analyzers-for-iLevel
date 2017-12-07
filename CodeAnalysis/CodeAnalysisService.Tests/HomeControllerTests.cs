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
        Mock<IDiagnosticService> diagnosticServiceMock = new Mock<IDiagnosticService>();
        Mock<ISolutionCreator> solutionCreatorMock = new Mock<ISolutionCreator>();
        Mock<HttpFileCollectionBase> filesMock = new Mock<HttpFileCollectionBase>();
        Mock<HttpRequestBase> requestMock = new Mock<HttpRequestBase>();
        Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
        Mock<HttpPostedFileBase> normalFileMock = new Mock<HttpPostedFileBase>();
        Mock<HttpPostedFileBase> wrongExtensionFileMock = new Mock<HttpPostedFileBase>();
        MemoryStream testStream = new MemoryStream();
        Dictionary<string, string> input;
        string diagnosticReport;

        HomeController controllerUnderTest;

        [TestInitialize]
        public void Setup()
        {
            //Setup input and output data
  
            string text = @"
new class A {
    int a;
}";
            input = new Dictionary<string, string>() { ["first"] = text };
            diagnosticReport = "Some message";

            //Setup dependencies mocks default behaviour
            diagnosticServiceMock.Setup(
                ds => ds.GetCompilationDiagnostic(It.IsAny<CSharpCompilation>())).Returns(new List<string>());

            solutionCreatorMock.Setup(
                sc => sc.GetCompilation(It.IsAny<IEnumerable<SyntaxTree>>(), It.IsAny<string>())).Returns(It.IsAny<CSharpCompilation>());

            solutionCreatorMock.Setup(
                sc => sc.GetSyntaxTrees(It.IsAny<Dictionary<string, string>>())).Returns(new List<SyntaxTree>());


            //Setup HttpRequest mocks
            string fileName = "TestFile";

            normalFileMock.Setup(f => f.FileName).Returns(fileName + ".cs");
            normalFileMock.Setup(f => f.ToString()).Returns("file1");
            normalFileMock.Setup(f => f.InputStream).Returns(testStream);

            wrongExtensionFileMock.Setup(f => f.FileName).Returns(fileName + ".jar");
            wrongExtensionFileMock.Setup(f => f.ToString()).Returns("file2");
            wrongExtensionFileMock.Setup(f => f.InputStream).Returns(testStream);

            filesMock.Setup(f => f.GetEnumerator()).Returns(new HttpPostedFileBase[] {normalFileMock.Object, wrongExtensionFileMock.Object }.GetEnumerator());
            filesMock.Setup(f => f.Count).Returns(2);
            filesMock.Setup(f => f["file1"]).Returns(normalFileMock.Object);
            filesMock.Setup(f => f["file2"]).Returns(wrongExtensionFileMock.Object);

            requestMock.Setup(r => r.Files).Returns(filesMock.Object);
            contextMock.Setup(c => c.Request).Returns(requestMock.Object);

            //Setup tested controller
            controllerUnderTest = new HomeController(diagnosticServiceMock.Object, solutionCreatorMock.Object);
            controllerUnderTest.ControllerContext = new ControllerContext(contextMock.Object, new RouteData(), controllerUnderTest);
        }

        [TestCleanup]
        public void Clenup()
        {
            diagnosticServiceMock.Reset();
            solutionCreatorMock.Reset();
            filesMock.Reset();
            requestMock.Reset();
            contextMock.Reset();
            controllerUnderTest = null;
        }

        [TestMethod]
        public void GetCompilationDiagnostic_InputNullArg_ThrowsArgNullException()
        {
            Action result = () => controllerUnderTest.GetCompilationDiagnostic(null);

            Assert.ThrowsException<ArgumentNullException>(result);
        }

        [TestMethod]
        public void GetCompilationDiagnostic_InputNullDiagnosticReturned_ThrowsNullReferenceExc()
        {
            diagnosticServiceMock.Reset();
            diagnosticServiceMock.Setup(ds => ds.GetCompilationDiagnostic(It.IsAny<CSharpCompilation>())).Returns<IEnumerable<string>>(null);

            Action result = () => controllerUnderTest.GetCompilationDiagnostic(new Dictionary<string, string>());

            Assert.ThrowsException<NullReferenceException>(result);
        }

        [TestMethod]
        public void GetCompilationDiagnostic_InputAnyDictionary_ReturnsAnyJsonResult()
        {
            var result = controllerUnderTest.GetCompilationDiagnostic(new Dictionary<string, string>());

            Assert.IsInstanceOfType(result, typeof(JsonResult));
        }

        [TestMethod]
        public void GetCompilationDiagnostic_InputCertainDictionary_ReturnsDiagnostic()
        {
            //string name = "a";
            //string sourceCode = @"
//new class A {
//    int a;
//}";
            //Dictionary<string, string> input = new Dictionary<string, string>() { [name] = sourceCode };
            SyntaxTree syntaxTree = Mock.Of<SyntaxTree>();
            CSharpCompilation compilation = default(CSharpCompilation);
            solutionCreatorMock.Reset();
            diagnosticServiceMock.Reset();
            solutionCreatorMock.Setup(sc => sc.GetSyntaxTrees(input)).Returns(new List<SyntaxTree>() { syntaxTree });
            solutionCreatorMock.Setup(sc => sc.GetCompilation(new List<SyntaxTree>() { syntaxTree }, It.IsAny<string>())).Returns(compilation);
            diagnosticServiceMock.Setup(ds => ds.GetCompilationDiagnostic(compilation)).Returns(new List<string>() { diagnosticReport });

            var result = (List<string>) controllerUnderTest.GetCompilationDiagnostic(input).Data;

            Assert.IsTrue(result.Contains(diagnosticReport));
        }

        [TestMethod]
        public void GetCompilationDiagnostic_InputNormalFiles_ReturnsJSONWithStringMessage()
        {
    //        string text = @"
    //class Program
    //{
    //    static void Main(string[] args) { }
    //}";
            //Dictionary<string, string> input = new Dictionary<string, string>() { ["first"] = text };
            Project project = new AdhocWorkspace().CurrentSolution.AddProject("TestName", "Name", LanguageNames.CSharp);

            solutionCreatorMock.Setup(
                x => x.GetProject(input, It.IsAny<string>())).Returns(project);
            diagnosticServiceMock.Setup(
                x => x.GetCompilationDiagnostic(project, It.IsAny<ImmutableArray<DiagnosticAnalyzer>>()))
                .Returns(new List<string>());

            var result = (string) controllerUnderTest.GetCompilationDiagnostic(input).Data;

            Assert.AreEqual(result, controllerUnderTest.OkMessage);
        }

        [TestMethod]
        public void GetCompilationDiagnostic_InputFilesWithWarnings_ReturnsListOfAnalyzersDiagnostic()
        {
    //        string text = @"
    //class Program
    //{
    //    static void Main(string[] args__) { }
    //}";
    //        Dictionary<string, string> input = new Dictionary<string, string>() { ["first"] = text };
            Project project = new AdhocWorkspace().CurrentSolution.AddProject("TestName", "Name", LanguageNames.CSharp);

            //string diagnosticReport = "some message";

            solutionCreatorMock.Setup(
                x => x.GetProject(input, It.IsAny<string>())).Returns(project);
            diagnosticServiceMock.Setup(
                x => x.GetCompilationDiagnostic(project, It.IsAny<ImmutableArray<DiagnosticAnalyzer>>()))
                .Returns(new List<string>() { diagnosticReport });

            var result = (List<string>) controllerUnderTest.GetCompilationDiagnostic(input).Data;

            Assert.IsTrue(result.Contains(diagnosticReport));
        }

        [TestMethod]
        public void Upload_InputEmptyRequest_Returns204StatusCode()
        {
            filesMock.Reset();
            filesMock.Setup(f => f.Count).Returns(0);

            var result = (HttpStatusCodeResult) controllerUnderTest.Upload();

            Assert.AreEqual(204, result.StatusCode);
        }

        [TestMethod]
        public void Upload_InputFileWithWrongExtension_Returns400StatusCode()
        {
            var result = (HttpStatusCodeResult) controllerUnderTest.Upload();

            Assert.AreEqual(400, result.StatusCode);
        }

        [TestMethod]
        public void Upload_InputSources_ReturnsAnyJson()
        {
            wrongExtensionFileMock.Reset();

            wrongExtensionFileMock.Setup(f => f.FileName).Returns("AnotherTestFile" + ".cs");
            wrongExtensionFileMock.Setup(f => f.InputStream).Returns(new MemoryStream());
            wrongExtensionFileMock.Setup(f => f.ToString()).Returns("file2");

            var result = controllerUnderTest.Upload();

            Assert.IsInstanceOfType(result, typeof(JsonResult));

            wrongExtensionFileMock.Reset();
        }

        [TestMethod]
        public void Upload_InputSourcesCausesNullReferenceException_ThrowsNullReferenceExeption()
        {
            diagnosticServiceMock.Reset();
            diagnosticServiceMock.Setup(ds => ds.GetCompilationDiagnostic(It.IsAny<CSharpCompilation>())).Returns<IEnumerable<string>>(null);

            wrongExtensionFileMock.Reset();

            wrongExtensionFileMock.Setup(f => f.FileName).Returns("AnotherTestFile" + ".cs");
            wrongExtensionFileMock.Setup(f => f.InputStream).Returns(new MemoryStream());
            wrongExtensionFileMock.Setup(f => f.ToString()).Returns("file2");

            Action result = () => controllerUnderTest.Upload();

            Assert.ThrowsException<NullReferenceException>(result);

            wrongExtensionFileMock.Reset();
        }

        [TestMethod]
        public void Index_ReturnedIndexView()
        {
            var result = (ViewResult) controllerUnderTest.Index();

            Assert.AreEqual("Index", result.ViewName);
        }
    }
}