using System;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Collections.Generic;
using CodeAnalysisService.Controllers;
using Moq;
using CodeAnalysis.BusinessLogicLayer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace CodeAnalysisService.Tests
{
    [TestClass]
    public class HomeControllerTests
    {
        Mock<IDiagnosticService> diagnosticServiceMock = new Mock<IDiagnosticService>();
        Mock<ISolutionCreator> solutionCreatorMock = new Mock<ISolutionCreator>();
        HomeController controllerUnderTest;

        [TestInitialize]
        public void Setup()
        {
            diagnosticServiceMock.Setup(
                ds => ds.GetCompilationDiagnostic(It.IsAny<CSharpCompilation>())).Returns(new List<string>());
            solutionCreatorMock.Setup(
                sc => sc.GetCompilation(It.IsAny<IEnumerable<SyntaxTree>>(), It.IsAny<string>())).Returns(It.IsAny<CSharpCompilation>());
            solutionCreatorMock.Setup(
                sc => sc.GetSyntaxTrees(It.IsAny<Dictionary<string, string>>())).Returns(new List<SyntaxTree>());
            controllerUnderTest = new HomeController(diagnosticServiceMock.Object, solutionCreatorMock.Object);
        }

        [TestCleanup]
        public void Clenup()
        {
            diagnosticServiceMock.Reset();
            solutionCreatorMock.Reset();
        }

        [TestMethod]
        public void GetCompilationDiagnostic_WithNullArg_ThrowsArgNullException()
        {
            Action result = () => controllerUnderTest.GetCompilationDiagnostic(null);

            Assert.ThrowsException<ArgumentNullException>(result);
        }

        [TestMethod]
        public void GetCompilationDiagnostic_WithNullDiagnosticReturned_ThrowsNullReferenceExc()
        {
            diagnosticServiceMock.Reset();
            diagnosticServiceMock.Setup(ds => ds.GetCompilationDiagnostic(It.IsAny<CSharpCompilation>())).Returns<IEnumerable<string>>(null);

            Action result = () => controllerUnderTest.GetCompilationDiagnostic(new Dictionary<string, string>());

            Assert.ThrowsException<NullReferenceException>(result);
        }

        [TestMethod]
        public void GetCompilationDiagnostic_WithAnyDictionary_ReturnsAnyJsonResult()
        {
            var result = controllerUnderTest.GetCompilationDiagnostic(new Dictionary<string, string>());

            Assert.IsInstanceOfType(result, typeof(JsonResult));
        }
    }
}