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
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
            => new ServiceMethodInLoopExecutionAnalyzer();

        const string ServiceClass = @"
namespace iLevel.ViewPoint.BusinessLogic.Services
{
    public interface ISomethingService
    {
        string Method();
    }
}
";

        const string NotServiceClass = @"
namespace iLevel.ViewPoint.BusinessLogic.Proviers
{
    public interface ISomethingProvider
    {
        string Method();
    }
}
";

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
        }        
    }
}";
            VerifyCSharpDiagnostic(GetFilesBatch(test), GetDiagnosticResult(9, 13));
        }

        [TestMethod]
        public void ServiceMethodInForLoop_ShowWarnings()
        {
            const string test = @"
using iLevel.ViewPoint.BusinessLogic.Services
class Program
{    
    public void Main(ISomethingService service)
    {
        for (int i = 0; i < 10; i++)
        {
            service.Method();
        }        
    }
}";
            VerifyCSharpDiagnostic(GetFilesBatch(test), GetDiagnosticResult(9, 13));
        }

        [TestMethod]
        public void ServiceMethodInWileLoop_ShowWarnings()
        {
            const string test = @"
using iLevel.ViewPoint.BusinessLogic.Services
class Program
{    
    public void Main(ISomethingService service)
    {
        while(true)
        {
            service.Method();
        }        
    }
}";
            VerifyCSharpDiagnostic(GetFilesBatch(test), GetDiagnosticResult(9, 13));
        }

        [TestMethod]
        public void ServiceMethodInDoLoop_ShowWarnings()
        {
            const string test = @"
using iLevel.ViewPoint.BusinessLogic.Services
class Program
{    
    public void Main(ISomethingService service)
    {
        do
        {
            service.Method();
        } while(true);       
    }
}";
            VerifyCSharpDiagnostic(GetFilesBatch(test), GetDiagnosticResult(9, 13));
        }

        private static string[] GetFilesBatch(string test) => new[] { ServiceClass, NotServiceClass, test };

        private static DiagnosticResult GetDiagnosticResult(int line, int col) => new DiagnosticResult
        {
            Id = "ILVL0002",
            Message = "Service method shouldn't be executed in a loop because there is no guarantee that cache has initialized properly",
            Severity = DiagnosticSeverity.Warning,
            Locations = new[] { new DiagnosticResultLocation("Test2.cs", line, col) }
        };
    }
}
