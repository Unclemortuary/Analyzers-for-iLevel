using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using Moq;
using iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.Tests
{
    public class CustomSourceText : SourceText
    {
        public CustomSourceText() { }
        
        public override char this[int position] => default(char);

        public override Encoding Encoding => default(Encoding);

        public override int Length => default(int);

        public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) { }
    }

    [TestClass]
    public class SolutionBLLTests
    {
        Dictionary<string, string> input = new Dictionary<string, string>
            {
                { "a", "1" },
                { "b", "2" }
            };
        List<SyntaxTree> inputList = new List<SyntaxTree>() { Mock.Of<SyntaxTree>(), Mock.Of<SyntaxTree>() };
        SolutionBLL objectUnderTest;
        Mock<ICustomSyntaxFactory> mock = new Mock<ICustomSyntaxFactory>();
        Mock<ICustomSolutionFactory> solutionFactoryMock = new Mock<ICustomSolutionFactory>();

        [TestInitialize]
        public void Setup()
        {
            mock.Setup(x => x.GetSourceText(It.IsAny<string>(), It.IsAny<Encoding>(), It.IsAny<SourceHashAlgorithm>())).Returns(It.IsAny<SourceText>());
            mock.Setup(x => x.ParseSyntaxTree(It.IsAny<SourceText>(), It.IsAny<ParseOptions>(), It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>())).Returns(It.IsAny<SyntaxTree>());
            objectUnderTest = new SolutionBLL(mock.Object, solutionFactoryMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mock.Reset();
            objectUnderTest = null;
        }


        [TestMethod]
        public void GetSyntaxTrees_EmptyDictionary_ReturnsEmptyCollection()
        {
            Dictionary<string, string> input = new Dictionary<string, string>();

            var result = objectUnderTest.GetSyntaxTrees(input);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetSyntaxTrees_NotEmptyDictionary_ReturnsNotEmptyCollection()
        {
            var result = objectUnderTest.GetSyntaxTrees(input);

            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void GetSyntaxTrees_CertainInput_ReturnsCertainSyntaxTree()
        {
            mock.Reset();
            CustomSourceText textA = new CustomSourceText();
            CustomSourceText textB = new CustomSourceText();
            SyntaxTree syntaxTreeA = Mock.Of<SyntaxTree>();
            SyntaxTree syntaxTreeB = Mock.Of<SyntaxTree>();
            
            mock.Setup(x => x.GetSourceText("1", It.IsAny<Encoding>(), It.IsAny<SourceHashAlgorithm>())).Returns(textA);
            mock.Setup(x => x.GetSourceText("2", It.IsAny<Encoding>(), It.IsAny<SourceHashAlgorithm>())).Returns(textB);
            mock.Setup(x => x.ParseSyntaxTree(textA, It.IsAny<ParseOptions>(), "a", It.IsAny<System.Threading.CancellationToken>())).Returns(syntaxTreeA);
            mock.Setup(x => x.ParseSyntaxTree(textB, It.IsAny<ParseOptions>(), "b", It.IsAny<System.Threading.CancellationToken>())).Returns(syntaxTreeB);

            var result = objectUnderTest.GetSyntaxTrees(input);

            mock.Verify();
            Assert.IsTrue(result.Contains(syntaxTreeA));
            Assert.IsTrue(result.Contains(syntaxTreeB));
        }


        [TestMethod]
        public void GetCompilation_InputEmptyCollection_ReturnsNull()
        {
            List<SyntaxTree> inputList = new List<SyntaxTree>();

            var result = objectUnderTest.GetCompilation(inputList, "");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetCompilation_InputCertainCollectionTestAssemblyName_ReturnsTestAssemblyCompilation()
        {
            var result = objectUnderTest.GetCompilation(inputList, "TestAssembly");

            mock.Verify(csf =>
            csf.Create("TestAssembly", inputList, It.IsAny<IEnumerable<MetadataReference>>(), It.IsAny<CSharpCompilationOptions>()));
        }


        [TestMethod]
        public void GetCompilation_InputCertainCollectionWithNullAssemblyName_ReturnsDefaultProjectCompilation()
        {
            string defaultName = objectUnderTest.ProjectName;

            var result = objectUnderTest.GetCompilation(inputList, null);

            mock.Verify(csf =>
            csf.Create(defaultName, It.IsAny<IEnumerable<SyntaxTree>>(), It.IsAny<IEnumerable<MetadataReference>>(), It.IsAny<CSharpCompilationOptions>()));
        }
    }
}
