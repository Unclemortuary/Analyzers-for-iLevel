using CodeAnalysis.BusinessLogicLayer;
using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.Tests
{
    [TestClass]
    public class DiagnosticBLLTests
    {
        DiagnosticBLL objectUnderTest = new DiagnosticBLL();
        List<Mock<Diagnostic>> diagnosticMocks = new List<Mock<Diagnostic>>() { new Mock<Diagnostic>(), new Mock<Diagnostic>(), new Mock<Diagnostic>() };

        [TestCleanup]
        public void CleanUp()
        {
            foreach (var mock in diagnosticMocks)
                mock.Reset();
        }

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
            diagnosticMocks[0].Setup(dg => dg.ToString()).Returns("Test error message");
            diagnosticMocks[1].Setup(dg => dg.ToString()).Returns("Another test message");
            string textFirst = "// Test error message" + Environment.NewLine;
            string textSecond = "// Another test message" + Environment.NewLine;

            var result = objectUnderTest.FormatDiagnostics(new Diagnostic[] { diagnosticMocks[0].Object, diagnosticMocks[1].Object });

            Assert.IsTrue(result.Contains(textFirst));
            Assert.IsTrue(result.Contains(textSecond));
        }

        [TestMethod]
        public void SortDiagnostic_InputEmptyCollection_ReturnsEmptyArray()
        {
            var result = objectUnderTest.SortDiagnostics(new List<Diagnostic>(), It.IsAny<DiagnosticSeverity>());

            CollectionAssert.DoesNotContain(result, It.IsAny<Diagnostic>());
        }

        [TestMethod]
        public void SortDiagnostic_InputDiagnosticsWithFewSeverities_ReturnsOnlyErrors()
        {
            Location locationMock = Mock.Of<Location>(
                l => l.SourceSpan == It.IsAny<TextSpan>());

            diagnosticMocks[0].Setup(warn => warn.Severity).Returns(DiagnosticSeverity.Warning);
            diagnosticMocks[0].Setup(warn => warn.Location).Returns(locationMock);
            diagnosticMocks[1].Setup(err => err.Severity).Returns(DiagnosticSeverity.Error);
            diagnosticMocks[1].Setup(err => err.Location).Returns(locationMock);

            List<Diagnostic> input = new List<Diagnostic>() { diagnosticMocks[0].Object, diagnosticMocks[1].Object };

            var result = objectUnderTest.SortDiagnostics(input, DiagnosticSeverity.Error);

            CollectionAssert.DoesNotContain(result, diagnosticMocks[0].Object);
        }

        [TestMethod]
        public void SortDiagnostic_InputDiagnosticsWithDifferentLocations_ReturnsAscendingCollection()
        {
            TextSpan firstSpan = new TextSpan(5, 2);
            TextSpan secondSpan = new TextSpan(2, 2);
            TextSpan thirdSpan = new TextSpan(8, 2);

            Location firstLocation = Mock.Of<Location>(
                l => l.SourceSpan == firstSpan);
            Location secondLocation = Mock.Of<Location>(
                l => l.SourceSpan == secondSpan);
            Location thirdLocation = Mock.Of<Location>(
                l => l.SourceSpan == thirdSpan);

            diagnosticMocks[0].Setup(dg => dg.Location).Returns(firstLocation);
            diagnosticMocks[0].Setup(dg => dg.Severity).Returns(It.IsAny<DiagnosticSeverity>());
            diagnosticMocks[1].Setup(dg => dg.Location).Returns(secondLocation);
            diagnosticMocks[1].Setup(dg => dg.Severity).Returns(It.IsAny<DiagnosticSeverity>());
            diagnosticMocks[2].Setup(dg => dg.Location).Returns(thirdLocation);
            diagnosticMocks[2].Setup(dg => dg.Severity).Returns(It.IsAny<DiagnosticSeverity>());


            Diagnostic[] expected = new Diagnostic[] { diagnosticMocks[1].Object, diagnosticMocks[0].Object, diagnosticMocks[2].Object };
            Diagnostic[] input = new Diagnostic[] { diagnosticMocks[0].Object, diagnosticMocks[1].Object, diagnosticMocks[2].Object };

            var result = objectUnderTest.SortDiagnostics(input, It.IsAny<DiagnosticSeverity>());

            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetCompilationDiagnostic_InputNull_ThrowsNullReferenceException()
        {
            Action result = () => objectUnderTest.GetCompilationDiagnostic(null);

            Assert.ThrowsException<NullReferenceException>(result);
        }
    }
}
