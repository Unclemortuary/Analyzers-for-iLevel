using CodeAnalysis.BusinessLogicLayer;
using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.Tests
{
    [TestClass]
    public class DiagnosticBLLTests
    {
        DiagnosticBLL objectUnderTest = new DiagnosticBLL();

        [TestMethod]
        public void FormatDiagnostic_EmptyDiagnosticsArray_ReturnsEmptyCollection()
        {
            var result = objectUnderTest.FormatDiagnostics(new Diagnostic[] { });

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void FormatDiagnostic_AnyDiagnosticsArray_ReturnsAnyCollection()
        {
            var result = objectUnderTest.FormatDiagnostics(new Diagnostic[] {Mock.Of<Diagnostic>(), Mock.Of<Diagnostic>() });

            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void FormatDiagnostic_TwoDiagnostics_ReturnsCollectionWithTwoStrings()
        {
            var diagnosticMockFirst = new Mock<Diagnostic>();
            var diagnosticMockSecond = new Mock<Diagnostic>();
            diagnosticMockFirst.Setup(dg => dg.ToString()).Returns("Test error message");
            diagnosticMockSecond.Setup(dg => dg.ToString()).Returns("Another test message");
            string textFirst = "// Test error message" + Environment.NewLine;
            string textSecond = "// Another test message" + Environment.NewLine;

            var result = objectUnderTest.FormatDiagnostics(new Diagnostic[] { diagnosticMockFirst.Object, diagnosticMockSecond.Object });

            Assert.IsTrue(result.Contains(textFirst));
            Assert.IsTrue(result.Contains(textSecond));
        }

        [TestMethod]
        public void SortDiagnostic_InputEmptyCollection_ReturnsEmptyArray()
        {
            var result = objectUnderTest.SortDiagnostics(new List<Diagnostic>(), It.IsAny<DiagnosticSeverity>());

            Assert.IsFalse(result.Any());
        }
    }
}
