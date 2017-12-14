using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using Moq;
using iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces;
using iLevel.CodeAnalysis.BusinessLogicLayer.Providers;


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
    public class SolutionProviderTests
    {
        Dictionary<string, string> _input = new Dictionary<string, string>
            {
                { "a", "1" },
                { "b", "2" }
            };
        List<SyntaxTree> _inputList = new List<SyntaxTree>() { Mock.Of<SyntaxTree>(), Mock.Of<SyntaxTree>() };
        SolutionProvider _objectUnderTest;
        Mock<ICustomSyntaxFactory> _syntaxFactoryMock = new Mock<ICustomSyntaxFactory>();
        Mock<ICustomSolutionFactory> _solutionFactoryMock = new Mock<ICustomSolutionFactory>();
        Mock<CustomSolution> _solutionMock = new Mock<CustomSolution>(new AdhocWorkspace().CurrentSolution);
        Project _testProject;

        [TestInitialize]
        public void Setup()
        {
            _syntaxFactoryMock.Setup(
                x => x.GetSourceText(It.IsAny<string>(), It.IsAny<Encoding>(), It.IsAny<SourceHashAlgorithm>()))
                .Returns(It.IsAny<SourceText>());
            _syntaxFactoryMock.Setup(
                x => x.ParseSyntaxTree(It.IsAny<SourceText>(), It.IsAny<ParseOptions>(), It.IsAny<string>(), It.IsAny<System.Threading.CancellationToken>()))
                .Returns(It.IsAny<SyntaxTree>());

            _objectUnderTest = new SolutionProvider(_syntaxFactoryMock.Object, _solutionFactoryMock.Object);

            _testProject = new AdhocWorkspace().CurrentSolution.AddProject(_objectUnderTest.ProjectName, _objectUnderTest.AssemblyName, LanguageNames.CSharp);

            _solutionMock.SetupGet(
                x => x.Projects).Returns(new List<Project>() { _testProject });
        }

        [TestCleanup]
        public void Cleanup()
        {
            _syntaxFactoryMock.Reset();
            _solutionMock.Reset();
            _objectUnderTest = null;
        }


        [TestMethod]
        public void GetSyntaxTrees_EmptyDictionary_ReturnsEmptyCollection()
        {
            Dictionary<string, string> input = new Dictionary<string, string>();

            var result = _objectUnderTest.GetSyntaxTrees(input);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetSyntaxTrees_NotEmptyDictionary_ReturnsNotEmptyCollection()
        {
            var result = _objectUnderTest.GetSyntaxTrees(_input);

            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void GetSyntaxTrees_CertainInput_ReturnsCertainSyntaxTree()
        {
            _syntaxFactoryMock.Reset();
            CustomSourceText textA = new CustomSourceText();
            CustomSourceText textB = new CustomSourceText();
            SyntaxTree syntaxTreeA = Mock.Of<SyntaxTree>();
            SyntaxTree syntaxTreeB = Mock.Of<SyntaxTree>();

            _syntaxFactoryMock.Setup(x => x.GetSourceText("1", It.IsAny<Encoding>(), It.IsAny<SourceHashAlgorithm>())).Returns(textA);
            _syntaxFactoryMock.Setup(x => x.GetSourceText("2", It.IsAny<Encoding>(), It.IsAny<SourceHashAlgorithm>())).Returns(textB);
            _syntaxFactoryMock.Setup(x => x.ParseSyntaxTree(textA, It.IsAny<ParseOptions>(), "a", It.IsAny<System.Threading.CancellationToken>())).Returns(syntaxTreeA);
            _syntaxFactoryMock.Setup(x => x.ParseSyntaxTree(textB, It.IsAny<ParseOptions>(), "b", It.IsAny<System.Threading.CancellationToken>())).Returns(syntaxTreeB);

            var result = _objectUnderTest.GetSyntaxTrees(_input);

            _syntaxFactoryMock.Verify();
            Assert.IsTrue(result.Contains(syntaxTreeA));
            Assert.IsTrue(result.Contains(syntaxTreeB));
        }


        [TestMethod]
        public void GetCompilation_InputEmptyCollection_ReturnsNull()
        {
            List<SyntaxTree> inputList = new List<SyntaxTree>();

            var result = _objectUnderTest.GetCompilation(inputList, "");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetCompilation_InputCertainCollectionTestAssemblyName_ReturnsTestAssemblyCompilation()
        {
            var result = _objectUnderTest.GetCompilation(_inputList, "TestAssembly");

            _syntaxFactoryMock.Verify(csf =>
            csf.Create("TestAssembly", _inputList, It.IsAny<IEnumerable<MetadataReference>>(), It.IsAny<CSharpCompilationOptions>()));
        }


        [TestMethod]
        public void GetCompilation_InputCertainCollectionWithNullAssemblyName_ReturnsDefaultProjectCompilation()
        {
            string defaultName = _objectUnderTest.AssemblyName;

            var result = _objectUnderTest.GetCompilation(_inputList, null);

            _syntaxFactoryMock.Verify(csf =>
            csf.Create(defaultName, It.IsAny<IEnumerable<SyntaxTree>>(), It.IsAny<IEnumerable<MetadataReference>>(), It.IsAny<CSharpCompilationOptions>()));
        }

        [TestMethod]
        public void GetProject_InputNullProjectNameAndEmptyCollection_ReturnsProjectWithoutDocuments()
        {
            CustomSolution solutionMockInstance = _solutionMock.Object;

            _solutionFactoryMock.Setup(
                x => x.Create(_objectUnderTest.ProjectName, _objectUnderTest.AssemblyName)).Returns(solutionMockInstance);

            var result = _objectUnderTest.GetProject(new Dictionary<string, string>(), null).Documents;

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetProject_Input2Sources_CallsMethodForDocsAdd2Times()
        {
            CustomSolution solutionMockInstance = _solutionMock.Object;
            
            _solutionFactoryMock.Setup(
                x => x.Create(_objectUnderTest.ProjectName, _objectUnderTest.AssemblyName)).Returns(solutionMockInstance);

            var result = _objectUnderTest.GetProject(_input, null).Documents;

            _solutionFactoryMock.Verify(x => x.AddDocument(It.IsAny<string>(), It.IsAny<SourceText>(), ref solutionMockInstance), Times.Exactly(2));
        }

        [TestMethod]
        public void GetProject_Input2CertainSources_CallsMethodForDocsAdd2TimesWithCertainSources()
        {
            CustomSolution solutionMockInstance = _solutionMock.Object;

            CustomSourceText textA = new CustomSourceText();
            CustomSourceText textB = new CustomSourceText();
            _syntaxFactoryMock.Setup(x => x.GetSourceText("1", It.IsAny<Encoding>(), It.IsAny<SourceHashAlgorithm>())).Returns(textA);
            _syntaxFactoryMock.Setup(x => x.GetSourceText("2", It.IsAny<Encoding>(), It.IsAny<SourceHashAlgorithm>())).Returns(textB);
            
            _solutionFactoryMock.Setup(
                x => x.Create("Some Name", _objectUnderTest.AssemblyName)).Returns(solutionMockInstance);

            var result = _objectUnderTest.GetProject(_input, "Some Name");
            
            _solutionFactoryMock.Verify(x => x.AddDocument("a", textA, ref solutionMockInstance));
            _solutionFactoryMock.Verify(x => x.AddDocument("b", textB, ref solutionMockInstance));
        }
    }
}
