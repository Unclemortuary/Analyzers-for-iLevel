using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeAnalysisService;
using CodeAnalysisService.Models;
using CodeAnalysisService.Controllers;
using CodeAnalysisService.Infrastructure;
using iLevel.CodeAnalysis.BusinessLogicLayer.Specification;
using iLevel.CodeAnalysis.ServiceIntegrationTests.Common;
using Unity;
using System.Linq;

namespace iLevel.CodeAnalysis.ServiceIntegrationTests
{
    [TestClass]
    public class HomeControllerIntegrationTests
    {
        Dictionary<string, string> _input;
        HomeController _controllerUnderTest;
        IEqualityComparer<ReportViewModel> testComparer = new ReportViewModelComparer();

        [TestInitialize]
        public void Setup()
        {
            UnityConfig.RegisterServices();
            AnalyzerConfig.RegisterAnalyzers(AnalyzerProvider.Analyzers);
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
            AnalyzerProvider.Analyzers.Clear();
            var context = MIMEFIlesRequestMocker.CreateHttpContextMockWithFilesFromStringPairs(_input);
            _controllerUnderTest.ControllerContext = new ControllerContext(context, new RouteData(), _controllerUnderTest);
            _controllerUnderTest.UploadAndReturnDiagnostic();
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
            var context = MIMEFIlesRequestMocker.CreateHttpContextMockWithFilesFromStringPairs(_input);
            _controllerUnderTest.ControllerContext = new ControllerContext(context, new RouteData(), _controllerUnderTest);
            var result = (JsonResult)_controllerUnderTest.UploadAndReturnDiagnostic();
            Assert.AreEqual(_controllerUnderTest.OkMessage, result.Data);
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

            
            var context = MIMEFIlesRequestMocker.CreateHttpContextMockWithFilesFromStringPairs(_input);
            _controllerUnderTest.ControllerContext = new ControllerContext(context, new RouteData(), _controllerUnderTest);
            _controllerUnderTest.Specification = new ExpressionSpecification(o => o.Severety == "Error");
            var result = (PartialViewResult)_controllerUnderTest.UploadAndReturnDiagnostic();
            var expected = new ReportViewModel
            {
                AnalyzerID = "CS5001",
                FileName = "",
                Location = "(0,0)",
                Severety = "Error",
                Message = "Program does not contain a static 'Main' method suitable for an entry point"
            };
            var resultCollection = (IEnumerable<ReportViewModel>)result.Model;
            var resultReport = resultCollection.First();
            Assert.IsTrue(resultCollection.Contains(expected, testComparer));
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

            var context = MIMEFIlesRequestMocker.CreateHttpContextMockWithFilesFromStringPairs(_input);
            _controllerUnderTest.Specification = new ExpressionSpecification(o => o.Severety == "Error");
            _controllerUnderTest.ControllerContext = new ControllerContext(context, new RouteData(), _controllerUnderTest);
            var result = (PartialViewResult)_controllerUnderTest.UploadAndReturnDiagnostic();
            var resultCollection = (IEnumerable<ReportViewModel>)result.Model;
            var expected = new ReportViewModel
            {
                AnalyzerID = "CS0246",
                FileName = "Program.cs",
                Location = "(7,13)",
                Severety = "Error",
                Message = "The type or namespace name 'Class1' could not be found (are you missing a using directive or an assembly reference?)"
            };
            Assert.IsTrue(resultCollection.Contains(expected, testComparer));
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

            var context = MIMEFIlesRequestMocker.CreateHttpContextMockWithFilesFromStringPairs(_input);
            _controllerUnderTest.Specification = new ExpressionSpecification(o => o.Severety == "Warning");
            _controllerUnderTest.ControllerContext = new ControllerContext(context, new RouteData(), _controllerUnderTest);

            var result = (PartialViewResult)_controllerUnderTest.UploadAndReturnDiagnostic();
            var resultCollection = (IEnumerable<ReportViewModel>)result.Model;

            var expected = new ReportViewModel
            {
                AnalyzerID = "ILVL0001",
                FileName = "Program.cs",
                Location = "(3,33)",
                Severety = "Warning",
                Message = "Rename argument name"
            };
            Assert.IsTrue(resultCollection.Contains(expected, testComparer));
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

            var context = MIMEFIlesRequestMocker.CreateHttpContextMockWithFilesFromStringPairs(_input);
            _controllerUnderTest.Specification = new ExpressionSpecification(o => o.Severety == "Warning");
            _controllerUnderTest.ControllerContext = new ControllerContext(context, new RouteData(), _controllerUnderTest);

            var result = (PartialViewResult)_controllerUnderTest.UploadAndReturnDiagnostic();
            var resultCollection = (IEnumerable<ReportViewModel>)result.Model;

            var expected = new ReportViewModel
            {
                AnalyzerID = "ILVL0002",
                FileName = "Program.cs",
                Location = "(11,17)",
                Severety = "Warning",
                Message = "Service method shouldn't be executed in a loop because there is no guarantee that cache has initialized properly"
            };
            Assert.IsTrue(resultCollection.Contains(expected, testComparer));
        }
    }
}
