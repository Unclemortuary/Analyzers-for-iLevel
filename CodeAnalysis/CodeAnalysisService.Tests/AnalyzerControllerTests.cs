using System;
using System.Collections.Generic;
using CodeAnalysisService.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeAnalysisService.Tests
{
    [TestClass]
    public class AnalyzerControllerTests
    {
        private AnalyzerController instance = new AnalyzerController();
        private const string okMessage = "Files not have any warnings or errors";

        
        public void PrepareMocks()
        {

        }

        [TestMethod]
        public void GetDiagnostic_NullList_ArgumentNullExceptionReturned()
        {
            //arrange
            List<string> list = null;

            //axo cover - for coverage unit test

            //act
            Action action = () => instance.GetDiagnostic(list);
            
            //assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void GetDiagnostic_EmptyList_ArgumentNullExceptionReturned()
        {
            List<string> list = null;

            Action action = () => instance.GetDiagnostic(list);

            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void GetDiagnostic_NormalSourcesList_OkMessageReturned()
        {
            string testString = @"
class Class
{
   public static void Method(string arg) { }
}";
            List<string> list = new List<string>();
            list.Add(testString);

            var excpected = instance.GetDiagnostic(list);

            Assert.AreEqual(okMessage, excpected);
        }


        [TestMethod]
        public void GetDiagnostic_UnNormalSourcesList_DiagnosticResultsReturned()
        {
            string testString = @"
class Class
{
   public static void Method(string arg___) { }
}";
            List<string> list = new List<string>();
            list.Add(testString);

            var excpected = instance.GetDiagnostic(list);

            Assert.AreNotEqual(okMessage, excpected);
        }

    }
}