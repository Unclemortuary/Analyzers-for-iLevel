using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeAnalysisService;
using CodeAnalysisService.Controllers;
using iLevel.CodeAnalysis.ServiceIntegrationTests.Common;
using Unity;

namespace iLevel.CodeAnalysis.ServiceIntegrationTests
{
    [TestClass]
    public class HomeControllerIntegrationTests
    {
        Dictionary<string, string> _input;
        HomeController _controllerUnderTest;

        [TestInitialize]
        public void Setup()
        {
            UnityConfig.RegisterServices();
            _input = new Dictionary<string, string>();
            _controllerUnderTest = UnityConfig.Container.Resolve<HomeController>();
        }

        [TestCleanup]
        public void Clenup()
        {
            _input = null;
            _controllerUnderTest = null;
        }

        [TestMethod]
        public void ServicesRegistrationPositiveTest()
        {
            var controller = UnityConfig.Container.Resolve<HomeController>();
            var viewResult = (ViewResult)controller.Index();
            var result = viewResult.ViewName;
            Assert.AreEqual("Index", result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AnalyzersReferencesRegistrationNegativeTest()
        {
            string argumentUnderscoreTest = @"
    class Program
    {
        static void Main(string[] args___)
        {

        }
    }
";
            _input.Add("Program", argumentUnderscoreTest);
            var controller = UnityConfig.Container.Resolve<HomeController>();
            controller.GetCompilationDiagnostic(_input);
        }

        [TestMethod]
        public void NormalFilesTest_ReturnsOkMessageDefinedInController()
        {
            string normalTest = @"
class Program
    {
        static void Main(string[] args) { }
    }
";
            _input.Add("Program", normalTest);

            AnalyzerConfig.RegisterAnalyzers(AnalyzerProvider.Analyzers);

            var controller = UnityConfig.Container.Resolve<HomeController>();
            var result = (string)controller.GetCompilationDiagnostic(_input).Data;
            Assert.AreEqual(controller.OkMessage, result);
        }

        [TestMethod]
        public void FilesWithoutProgramMethod_GetCorrespondingMessage()
        {
            string argumentUnderscoreTest = @"
    class Program
    {
        void Method() { }
    }
";
            _input.Add("Program", argumentUnderscoreTest);

            AnalyzerConfig.RegisterAnalyzers(AnalyzerProvider.Analyzers);

            var controller = UnityConfig.Container.Resolve<HomeController>();
            var resultList = (List<string>)controller.GetCompilationDiagnostic(_input).Data;
            var result = resultList[0];
            var expected = new ServiceDiagnosticResult()
            {
                Location = { FileName = "", Line = null, Column = null },
                SeveretyType = SeveretyType.error,
                DiagnosticMessage = "Program does not contain a static 'Main' method suitable for an entry point",
                AnalyzerID = ""
            };
            ServiceDiagnosticVerifier.Verify(expected, result);
        }

        [TestMethod]
        public void FilesWithArgumentNamesUnderscore_GetDiagnosticFromILVL0001Analyzer()
        {
            string argumentUnderscoreTest = @"
    class Program
    {
        static void Main(string[] args___) { }
    }
";
            _input.Add("Program", argumentUnderscoreTest);

            AnalyzerConfig.RegisterAnalyzers(AnalyzerProvider.Analyzers);

            var controller = UnityConfig.Container.Resolve<HomeController>();
            var resultList = (List<string>) controller.GetCompilationDiagnostic(_input).Data;
            var result = resultList[0];
            var expected = new ServiceDiagnosticResult()
            {
                Location = {FileName = "Program", Line = 4, Column = 26},
                SeveretyType = SeveretyType.warning,
                DiagnosticMessage = "Rename argument name",
                AnalyzerID = "ILVL0001"
            };
            ServiceDiagnosticVerifier.Verify(expected, result);
        }

        
    }
}
