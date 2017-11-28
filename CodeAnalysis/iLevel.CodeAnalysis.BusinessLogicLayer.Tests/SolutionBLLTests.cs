using System;
using CodeAnalysis.BusinessLogicLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections;
using System.Collections.Immutable;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Text;
using Test.Extensions;

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
        [TestMethod]
        public void GetSyntaxTrees_EmptyDictionary_ReturnsEmptyCollection()
        {
            ICustomSyntaxFactory customSyntaxFactory = Mock.Of<ICustomSyntaxFactory>();
            SolutionBLL testedObject = new SolutionBLL(customSyntaxFactory);
            Dictionary<string, string> input = new Dictionary<string, string>();

            var result = testedObject.GetSyntaxTrees(input);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetSyntaxTrees_NotEmptyDictionary_ReturnsNotEmptyCollection()
        {
            var mockedSourceText = new Mock<SourceText>(It.IsAny<ImmutableArray<byte>>(), It.IsAny<SourceHashAlgorithm>(), It.IsAny<SourceTextContainer>());

            ICustomSyntaxFactory customSyntaxFactory = Mock.Of<ICustomSyntaxFactory>(
              x => x.GetSourceText(It.IsAny<string>(), It.IsAny<Encoding>(), It.IsAny<SourceHashAlgorithm>()) == It.IsAny<SourceText>() &&
              x.ParseSyntaxTree(It.IsAny<SourceText>(), It.IsAny<ParseOptions>(), It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>()) == It.IsAny<SyntaxTree>());


            SolutionBLL testedObject = new SolutionBLL(customSyntaxFactory);
            Dictionary<string, string> input = new Dictionary<string, string>
            {
                { "a", "1" },
                { "b", "2" }
            };

            var result = testedObject.GetSyntaxTrees(input);

            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void GetSyntaxTrees_CertainInput_ReturnsCertainSyntaxTree()
        {
            var mock = new Mock<ICustomSyntaxFactory>();
            
            Dictionary<string, string> input = new Dictionary<string, string>
            {
                { "a", "1" },
                { "b", "2" }
            };

            CustomSourceText textA = new CustomSourceText();
            CustomSourceText textB = new CustomSourceText();
            SyntaxTree syntaxTreeA = Mock.Of<SyntaxTree>();
            SyntaxTree syntaxTreeB = Mock.Of<SyntaxTree>();


            mock.Setup<SourceText>(x => x.GetSourceText("1", It.IsAny<Encoding>(), It.IsAny<SourceHashAlgorithm>())).Returns(textA);
            mock.Setup<SourceText>(x => x.GetSourceText("2", It.IsAny<Encoding>(), It.IsAny<SourceHashAlgorithm>())).Returns(textB);
            mock.Setup(x => x.ParseSyntaxTree(textA, It.IsAny<ParseOptions>(), "a", It.IsAny<System.Threading.CancellationToken>())).Returns(syntaxTreeA);
            mock.Setup(x => x.ParseSyntaxTree(textB, It.IsAny<ParseOptions>(), "b", It.IsAny<System.Threading.CancellationToken>())).Returns(syntaxTreeB);


            SolutionBLL testedObject = new SolutionBLL(mock.Object);

            var result = testedObject.GetSyntaxTrees(input);

            Assert.IsTrue(result.Contains(syntaxTreeA));
            Assert.IsTrue(result.Contains(syntaxTreeB));
            mock.Verify();
        }


        [TestMethod]
        public void GetCompilation_InputEmptyCollection_ReturnsNull()
        {
            var objectOnTest = new SolutionBLL(Mock.Of<ICustomSyntaxFactory>());
            List<SyntaxTree> inputList = new List<SyntaxTree>();

            var result = objectOnTest.GetCompilation(inputList, "");

            Assert.IsNull(result);
        }

    }
}
