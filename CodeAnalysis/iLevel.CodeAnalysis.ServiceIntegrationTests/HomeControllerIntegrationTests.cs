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
        public void FilesWithoutProgramMethod_GetCorrespondingMessage()
        {
            string argumentUnderscoreTest = @"
namespace Test_classes_for_analyzer
{
    class Program
    {
        static void Main(string[] args___) { }
    }
}";
            _input.Add("Program", argumentUnderscoreTest);

            UnityConfig.RegisterServices();
            AnalyzerConfig.RegisterAnalyzers(AnalyzerProvider.Analyzers);

            var controller = UnityConfig.Container.Resolve<HomeController>();
            var result = (List<string>) controller.GetCompilationDiagnostic(_input).Data;
            var expected = new ServiceDiagnosticResult()
            {
                Location = {FileName = "Program", Line = 6, Column = 26},
                SeveretyType = SeveretyType.Warning,
                AnalyzerMessage = "Rename argument name",
                AnalyzerID = "ILVL00001"
            };
            ServiceDiagnosticVerifier.Verify(expected, result[0]);
        }
    }
}
