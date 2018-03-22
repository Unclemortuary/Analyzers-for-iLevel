using Microsoft.VisualStudio.TestTools.UnitTesting;
using iLevel.CodeAnalysis.BestPractices.Tests.Common;
using Microsoft.CodeAnalysis.Diagnostics;
using iLevel.ViewPoint.CodeAnalysis.BestPractices;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis;

namespace iLevel.CodeAnalysis.BestPractices.Tests
{
    [TestClass]
    public class HelloWorldAnalyzerTests : CodeFixVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
            => new HelloWorldAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider()
            => new HelloWorldAnalyzerCodeFixProvider();

        [TestMethod]
        public void MethodWithEmptyParameters_ShowWarning()
        {
            const string test = @"
class Program
{
    public void Main()
    {
        Console.WriteLine();
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "HelloWorldAnalyzer",
                Message = "Maybe should write \"Hello world\" instead of an empty message",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 5, 9) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }
    }
}
