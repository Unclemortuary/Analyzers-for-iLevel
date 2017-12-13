using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeAnalysisService;
using CodeAnalysisService.Controllers;
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
            string inputFile = @"
    class Program
    {
        static void Main(string[] args___)
        {

        }
    }
";
            _input.Add("Program", inputFile);
            var controller = UnityConfig.Container.Resolve<HomeController>();
            controller.GetCompilationDiagnostic(_input);
        }
    }
}
