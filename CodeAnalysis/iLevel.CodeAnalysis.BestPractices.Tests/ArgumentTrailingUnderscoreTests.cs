using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iLevel.CodeAnalysis.BestPractices.Tests.Common;
using Microsoft.CodeAnalysis.Diagnostics;
using iLevel.ViewPoint.CodeAnalysis.BestPractices;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis;

namespace iLevel.CodeAnalysis.BestPractices.Tests
{
    [TestClass]
    public class ArgumentTrailingUnderscoreTests : CodeFixVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
            => new ArgumentTrailingUnderscoreAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider()
            => new ArgumentTrailingUnderscoreFixProvider();

        [TestMethod]
        public void EmptyMethod_ShowWarning()
        {
            const string test = @"
class Class
{
    public void Method(string arg_) { }
}";
            var expected = new DiagnosticResult
            {
                Id = "ILVL0001",
                Message = "Rename argument name",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 24) }
            };

            VerifyCSharpDiagnostic(test, null, expected);

            const string fixtest = @"
class Class
{
    public void Method(string arg) { }
}";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void EmptyMethod_FixArgument()
        {
            const string test = @"
class Class
{
    public void Method(string arg_) { }
}";
            const string fixtest = @"
class Class
{
    public void Method(string arg) { }
}";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void NotEmptyMethod_FixArgumentAndUsages()
        {
            const string test = @"
class Class
{
    public void Method(string arg_) 
    {
        string nothing = arg_.Trim();
    }
}";
            const string fixtest = @"
class Class
{
    public void Method(string arg) 
    {
        string nothing = arg.Trim();
    }
}";
            VerifyCSharpFix(test, fixtest);
        }


        [TestMethod]
        public void MethodLambda_FixArgumentAndUsages()
        {
            const string test = @"
class Class
{
    public void Method(string arg_) => arg_.Trim();
}";
            const string fixtest = @"
class Class
{
    public void Method(string arg) => arg.Trim();
}";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void Interface_FixArgument()
        {
            const string test = @"
interface Interface
{
    void Method(string arg_);
}";
            const string fixtest = @"
interface Interface
{
    void Method(string arg);
}";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void NamedArgumentExecution_FixArgument()
        {
            const string test = @"
class Class
{
   public static void Method(string arg_) { }
}

class Executor
{
    public void Execute() => Class.Method(arg_: ""Hello World!!!"");
}";
            const string fixtest = @"
class Class
{
   public static void Method(string arg) { }
}

class Executor
{
    public void Execute() => Class.Method(arg: ""Hello World!!!"");
}";
            VerifyCSharpFix(test, fixtest);
        }
    }
}
