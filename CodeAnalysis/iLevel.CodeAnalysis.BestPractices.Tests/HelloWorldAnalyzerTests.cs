using Microsoft.VisualStudio.TestTools.UnitTesting;
using iLevel.CodeAnalysis.BestPractices.Tests.Common;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace iLevel.CodeAnalysis.BestPractices.Tests
{
    [TestClass]
    public class HelloWorldAnalyzerTests : CodeFixVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
            => new HelloWorldAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider()
            => new HelloWorldAnalyzerCodeFixProvider();

        private List<string> additionalAssemblies = new List<string> { "System.Console", "System.Runtime" };

        [TestMethod]
        public void MethodWithAnyParameter_NoWarnings()
        {
            const string test = @"
using System;
class Program
{
    public void Main(string[] args)
    {
        Console.WriteLine("");
    }
}";

            VerifyCSharpDiagnostic(test, additionalAssemblies);
        }

        [TestMethod]
        public void CallFromMethod_ShowWarning()
        {
            const string test = @"
using System;
class Program
{
    public void Main(string[] args)
    {
        Console.WriteLine();
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "HelloWorldAnalyzer",
                Message = "Maybe should write \"Hello world\" instead of an empty message",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 9) }
            };

            VerifyCSharpDiagnostic(test, additionalAssemblies, expected);

            const string fixtest = @"
using System;
class Program
{
    public void Main(string[] args)
    {
        Console.WriteLine(""Hello World!"");
    }
}";

            VerifyCSharpFix(test, fixtest, additionalAssemblies: additionalAssemblies);
        }

        [TestMethod]
        public void CallFromDelegate_ShowWarning()
        {
            const string test = @"
using System;
class Program
{
    Action test = () => { Console.WriteLine(); };
}";
            var expected = new DiagnosticResult
            {
                Id = "HelloWorldAnalyzer",
                Message = "Maybe should write \"Hello world\" instead of an empty message",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 5, 27) }
            };

            VerifyCSharpDiagnostic(test, additionalAssemblies, expected);

            const string fixtest = @"
using System;
class Program
{
    Action test = () => { Console.WriteLine(""Hello World!""); };
}";

            VerifyCSharpFix(test, fixtest, additionalAssemblies: additionalAssemblies);
        }
    }
}
