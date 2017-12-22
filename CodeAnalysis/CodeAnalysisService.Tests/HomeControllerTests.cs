using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;
using System.Collections.Generic;
using CodeAnalysisService.Controllers;
using WEB = CodeAnalysisService.Infrastructure;
using CodeAnalysisService.Models;
using iLevel.CodeAnalysis.AnalyzersAccesLayer.Interfaces;
using iLevel.CodeAnalysis.BusinessLogicLayer.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CodeAnalysisService.Tests
{
    [TestClass]
    public class HomeControllerTests
    {
        Mock<IDiagnosticProvider> _diagnosticProviderMock = new Mock<IDiagnosticProvider>();
        Mock<WEB.IMapper> _mapperMock = new Mock<WEB.IMapper>();
        Mock<HttpFileCollectionBase> _filesMock = new Mock<HttpFileCollectionBase>();
        Mock<HttpRequestBase> _requestMock = new Mock<HttpRequestBase>();
        Mock<HttpContextBase> _contextMock = new Mock<HttpContextBase>();
        Mock<HttpPostedFileBase> _postedFileMock = new Mock<HttpPostedFileBase>();
        Mock<SourceFileDTO> sourcesDTOMock = new Mock<SourceFileDTO>();
        MemoryStream _testStream = new MemoryStream();
        const string _FILE_NAME= "TestFile";

        Dictionary<string, string> _expectedDictionary = new Dictionary<string, string> { ["TestFile.cs"] = "" };
        List<SourceFileDTO> _mapedSources;

        HomeController _controllerUnderTest;

        [TestInitialize]
        public void Setup()
        {
            
            _postedFileMock.Setup(f => f.FileName).Returns(_FILE_NAME + ".cs");
            _postedFileMock.Setup(f => f.ToString()).Returns(_FILE_NAME);
            _postedFileMock.Setup(f => f.InputStream).Returns(_testStream);

            _filesMock.SetupGet(
                x => x.Count).Returns(1);
            _filesMock.Setup(f => f.GetEnumerator())
                .Returns(new HttpPostedFileBase[] { _postedFileMock.Object }.GetEnumerator());
            _filesMock.Setup(f => f[_FILE_NAME]).Returns(_postedFileMock.Object);

            _requestMock.Setup(r => r.Files).Returns(_filesMock.Object);
            _contextMock.Setup(c => c.Request).Returns(_requestMock.Object);

            _mapedSources = new List<SourceFileDTO> { sourcesDTOMock.Object };
            _mapperMock.Setup(
                x => x.ToSourceFileDTO(_expectedDictionary)).Returns(_mapedSources);

            _controllerUnderTest = new HomeController(_diagnosticProviderMock.Object, _mapperMock.Object);
            _controllerUnderTest.ControllerContext = new ControllerContext(_contextMock.Object, new RouteData(), _controllerUnderTest);
        }

        [TestCleanup]
        public void Clenup()
        {
            _diagnosticProviderMock.Reset();
            _mapperMock.Reset();
            _filesMock.Reset();
            _postedFileMock.Reset();
            _requestMock.Reset();
            _contextMock.Reset();
            _controllerUnderTest = null;
            _mapedSources = null;
        }

        [TestMethod]
        public void UploadAndReturnDiagnostic_InputNoMIMEFiles_Returns204StatusCodeWithNoFilesMessage()
        {
            _filesMock.SetupGet(
                x => x.Count).Returns(0);
            var result = (HttpStatusCodeResult)_controllerUnderTest.UploadAndReturnDiagnostic();
            Assert.AreEqual(204, result.StatusCode);
            Assert.AreEqual(_controllerUnderTest.NoFilesMessage, result.StatusDescription);
        }

        [TestMethod]
        public void UploadAndReturnDiagnostic_InputFileWithWrongExtension_Returns400StatusCodeWithWrongExtensionMessage()
        {
            _postedFileMock.Reset();
            _postedFileMock.Setup(f => f.FileName).Returns(_FILE_NAME + ".txt");
            _postedFileMock.Setup(f => f.ToString()).Returns(_FILE_NAME);
            var result = (HttpStatusCodeResult)_controllerUnderTest.UploadAndReturnDiagnostic();
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual(_controllerUnderTest.WrongExtensionMessage, result.StatusDescription);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void UploadAndReturnDiagnostic_InputNullValueInFileCollection_ReturnsNullReferenceException()
        {
            _filesMock.Setup(
                x => x[It.IsAny<string>()]).Returns<HttpPostedFileBase>(null);
            var result = (HttpStatusCodeResult)_controllerUnderTest.UploadAndReturnDiagnostic();
        }

        [TestMethod]
        public void UploadAndReturnDiagnostic_DiagnosticAndMapperServicesCalls()
        {
            _controllerUnderTest.UploadAndReturnDiagnostic();

            _mapperMock.VerifyAll();
            _diagnosticProviderMock.Verify(
                x => x.GetDiagnostic(_mapedSources,
                WEB.AnalyzerProvider.Analyzers,
                _controllerUnderTest.DefaultSpecification));
        }

        [TestMethod]
        public void UploadAndReturnDiagnostic_ReturnsJsonMessageWhenDiagnosticCollectionIsEmpty()
        {
            _diagnosticProviderMock.Setup(
                x => x.GetDiagnostic(_mapedSources,
                WEB.AnalyzerProvider.Analyzers,
                _controllerUnderTest.DefaultSpecification)).Returns(new List<ReportDTO>());
            
            var result = (JsonResult) _controllerUnderTest.UploadAndReturnDiagnostic();
            
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            Assert.AreEqual(_controllerUnderTest.OkMessage, result.Data);
        }

        [TestMethod]
        public void UploadAndReturnDiagnostic_ReturnsPartialViewWhenDiagnosticCollectionNotEmpty()
        {
            var expected = new List<ReportViewModel> { new ReportViewModel() };

            _mapperMock.Reset();
            _mapperMock.Setup(
                x => x.ToSourceFileDTO(_expectedDictionary)).Returns(_mapedSources);
            _mapperMock.Setup(
                x => x.ToReportViewModel(It.IsAny<IEnumerable<ReportDTO>>()))
                .Returns(expected);

            _diagnosticProviderMock.Setup(
                x => x.GetDiagnostic(_mapedSources,
                WEB.AnalyzerProvider.Analyzers,
                _controllerUnderTest.DefaultSpecification)).Returns(new List<ReportDTO> { new ReportDTO()});

            var result = (PartialViewResult) _controllerUnderTest.UploadAndReturnDiagnostic();

            Assert.AreEqual(expected, result.Model);
        }

        [TestMethod]
        public void UploadAndReturnDiagnostic_InputCertainFile_ReturnsPartialViewWithCertainDiagnostic()
        {
            var expectedDiagnostic = new List<ReportDTO> { new ReportDTO() };
            var expected = new List<ReportViewModel> { new ReportViewModel() };

            _mapperMock.Reset();
            _mapperMock.Setup(
                x => x.ToSourceFileDTO(_expectedDictionary)).Returns(_mapedSources);
            _mapperMock.Setup(
                x => x.ToReportViewModel(expectedDiagnostic))
                .Returns(expected);

            _diagnosticProviderMock.Setup(
                x => x.GetDiagnostic(_mapedSources,
                WEB.AnalyzerProvider.Analyzers,
                _controllerUnderTest.DefaultSpecification)).Returns(expectedDiagnostic);

            var result = (PartialViewResult)_controllerUnderTest.UploadAndReturnDiagnostic();

            Assert.AreEqual(expected, result.Model);
        }

        [TestMethod]
        public void Index_ReturnedIndexView()
        {
            var result = (ViewResult) _controllerUnderTest.Index();

            Assert.AreEqual("Index", result.ViewName);
        }
    }
}