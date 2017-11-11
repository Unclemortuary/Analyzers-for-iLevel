using iLevel.CodeAnalysis.BestPractices.Tests.Common;
using iLevel.ViewPoint.CodeAnalysis.BestPractices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iLevel.CodeAnalysis.BestPractices.Tests
{
    [TestClass]
    public class ServiceMethodInLoopExecutionTests : CodeFixVerifier
    {
        const string ServiceClass = @"
namespace iLevel.ViewPoint.BusinessLogic.Services
{
    public interface ISomethingService
    {
        string Method();
        int Property { get; set; }
    }
}
";

        const string NotServiceClass = @"
namespace iLevel.ViewPoint.BusinessLogic.Proviers
{
    public interface ISomethingProvider
    {
        string Method();
        int Property { get; set; }
    }
}
";

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
            => new ServiceMethodInLoopExecutionAnalyzer();

        [TestMethod]
        public void ServiceMethodOutOfLoop_NoWarnings()
        {
            const string test = @"
using iLevel.ViewPoint.BusinessLogic.Services
class Program
{    
    public void Main(ISomethingService service)
    {
        service.Method();
        int i = service.Property;
        service.Property = i;
    }
}";
            VerifyCSharpDiagnostic(GetFilesBatch(test));
        }

        [TestMethod]
        public void ServiceMethodInForeachLoop_ShowWarnings()
        {
            const string test = @"
using iLevel.ViewPoint.BusinessLogic.Services
class Program
{    
    public void Main(ISomethingService service)
    {
        foreach (var item in new[] { 1 })
        {
            service.Method();
            int i = service.Property;
            service.Property = i;
        }        
    }
}";
            DiagnosticResult getDiagnosticResult(int line, int col) => new DiagnosticResult
            {
                Id = "ILVL0002",
                Message = "Service method shouldn't be executed in a loop",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test2.cs", line, col) }
            };

            VerifyCSharpDiagnostic(GetFilesBatch(test), new[]
            {
                getDiagnosticResult(9, 24),
                getDiagnosticResult(10, 24),
                getDiagnosticResult(11, 24)
            });
        }

        private static string[] GetFilesBatch(string test) => new[] { ServiceClass, NotServiceClass, test };
    }
}
