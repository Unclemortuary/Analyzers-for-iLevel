using System;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using CodeAnalysisService.Controllers;
using CodeAnalysis.BusinessLogicLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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


        HomeController controllerUnderTest;

        [TestInitialize]
        public void Setup()
        {
            //Setup dependencies mocks default behaviour
            diagnosticServiceMock.Setup(
                ds => ds.GetCompilationDiagnostic(It.IsAny<CSharpCompilation>())).Returns(new List<string>());

            solutionCreatorMock.Setup(
                sc => sc.GetCompilation(It.IsAny<IEnumerable<SyntaxTree>>(), It.IsAny<string>())).Returns(It.IsAny<CSharpCompilation>());

            solutionCreatorMock.Setup(
                sc => sc.GetSyntaxTrees(It.IsAny<Dictionary<string, string>>())).Returns(new List<SyntaxTree>());


            //Setup HttpRequest mocks
            filesMock.Setup(f => f.GetEnumerator()).Returns(new HttpFileCollection[] { }.GetEnumerator());
            requestMock.Setup(r => r.Files).Returns(filesMock.Object);
            contextMock.Setup(c => c.Request).Returns(requestMock.Object);

            //Setup tested controller
            controllerUnderTest = new HomeController(diagnosticServiceMock.Object, solutionCreatorMock.Object);
            controllerUnderTest.ControllerContext = new ControllerContext(contextMock.Object, new System.Web.Routing.RouteData(), controllerUnderTest);
        }

        [TestCleanup]
        public void Clenup()
        {
            diagnosticServiceMock.Reset();
            solutionCreatorMock.Reset();
            filesMock.Reset();
            requestMock.Reset();
            contextMock.Reset();
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
            string name = "a";
            string sourceCode = @"
new class A {
    int a;
}";
            Dictionary<string, string> input = new Dictionary<string, string>() { [name] = sourceCode };
            SyntaxTree syntaxTree = Mock.Of<SyntaxTree>();
            CSharpCompilation compilation = default(CSharpCompilation);
            string diagnostic = "";
            solutionCreatorMock.Reset();
            diagnosticServiceMock.Reset();
            solutionCreatorMock.Setup(sc => sc.GetSyntaxTrees(input)).Returns(new List<SyntaxTree>() { syntaxTree });
            solutionCreatorMock.Setup(sc => sc.GetCompilation(new List<SyntaxTree>() { syntaxTree }, It.IsAny<string>())).Returns(compilation);
            diagnosticServiceMock.Setup(ds => ds.GetCompilationDiagnostic(compilation)).Returns(new List<string>() { diagnostic });
            

            var action = controllerUnderTest.GetCompilationDiagnostic(input);

            var result = action.Data as List<string>;

            Assert.IsTrue(result.Contains(diagnostic));
        }

        [TestMethod]
        public void Upload_InputEmtyRequest_Returns204StatusCode()
        {
            filesMock.Setup(f => f.Count).Returns(0);

            var result = controllerUnderTest.Upload() as HttpStatusCodeResult;

            Assert.AreEqual(204, result.StatusCode);
        }
    }
}