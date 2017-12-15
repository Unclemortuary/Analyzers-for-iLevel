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
            UnityConfig.Container.Dispose();
            AnalyzerProvider.Analyzers.Clear();
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
    }";
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
        public void FilesWithoutProgramMethod_ReturnsCorrespondingMessage()
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
        public void FilesWithtMissedReference_ReturnsCorrespondingMessage()
        {
            string classWithServiceMethodDeclaration = @"
namespace Test_classes_for_analyzer
{
    class Class1
    {
        public void someMethod(int someArg, int anotherArg) { }
    }
}";
            string classUsesService = @"
namespace SomeNamespace
{
    class Program
    {
        static void Main(string[] args___)
        {
             Class1 class1 = new Class1();
             class1.someMethod(5);
        }
    }
}";
            _input.Add("Class", classWithServiceMethodDeclaration);
            _input.Add("Program", classUsesService);

            AnalyzerConfig.RegisterAnalyzers(AnalyzerProvider.Analyzers);

            var controller = UnityConfig.Container.Resolve<HomeController>();
            var result = (List<string>)controller.GetCompilationDiagnostic(_input).Data;
            var expected = new ServiceDiagnosticResult()
            {
                Location = { FileName = "Program", Line = 8, Column = 14 },
                SeveretyType = SeveretyType.error,
                DiagnosticMessage = "The type or namespace name 'Class1' could not be found (are you missing a using directive or an assembly reference?)",
                AnalyzerID = ""
            };
            ServiceDiagnosticVerifier.Verify(expected, result[0]);
        }

        [TestMethod]
        public void FilesWithArgumentNamesUnderscore_ReturnsWarningFromILVL0001Analyzer()
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

        [TestMethod]
        public void FileUsesServiceInLoop_ReturnsWarningFromILVL0002Analyzer()
        {
            string service = @"
namespace Service
{
    public interface ISomeService
    {
        void Method();
    }
}";

            string serviceInLoopTest = @"
using Service;

    class Program
    {
		static ISomeService service = null;
		
        static void Main(string[] args)
        {
            do
			{
				service.Method();
			} while(true);
        }
    }";
            _input.Add("Service", service);
            _input.Add("Program", serviceInLoopTest);

            AnalyzerConfig.RegisterAnalyzers(AnalyzerProvider.Analyzers);

            var controller = UnityConfig.Container.Resolve<HomeController>();
            var resultList = (List<string>)controller.GetCompilationDiagnostic(_input).Data;
            var result = resultList[0];
            var expected = new ServiceDiagnosticResult()
            {
                Location = { FileName = "Program", Line = 12, Column = 17 },
                SeveretyType = SeveretyType.warning,
                DiagnosticMessage = "Service method shouldn't be executed in a loop because there is no guarantee that cache has initialized properly",
                AnalyzerID = "ILVL0002"
            };
            ServiceDiagnosticVerifier.Verify(expected, result);
        }
    }
}
