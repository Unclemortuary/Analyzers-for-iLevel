using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalysisService.Infrastructure;
using iLevel.CodeAnalysis.BusinessLogicLayer.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeAnalysisService.Tests
{
    [TestClass]
    public class MapperTests
    {
        Mapper _mapperUnderTest;

        [TestInitialize]
        public void Setup()
        {
            _mapperUnderTest = new Mapper();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _mapperUnderTest = null;
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ToSourceFileDTO_InputNull_ThrowsNullReferenceException()
        {
            _mapperUnderTest.ToSourceFileDTO(null);
        }

        [TestMethod]
        public void ToSourceFileDTO_InputDictionaryWith2Items_ReturnsCollectionWith2Items()
        {
            Dictionary<string, string> input = new Dictionary<string, string> { ["a"] = "A", ["b"] = "B" };
            var result = _mapperUnderTest.ToSourceFileDTO(input);
            Assert.AreEqual(2, result.Count());
        }
        
        [TestMethod]
        public void ToReportViewModel_InputEmptyCollection_ReturnsEmptyCollection()
        {
            var result = _mapperUnderTest.ToReportViewModel(new List<ReportDTO>());
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void ToReportViewModel_Input2ItemsCollection_Returns2ItemsCollection()
        {
            var result = _mapperUnderTest.ToReportViewModel(new List<ReportDTO> { new ReportDTO(), new ReportDTO()});
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void ToReportViewModel_InputItemWithNullField_ItemInReturnedCollectionHasSameNullField()
        {
            var itemWithNullFields = new ReportDTO { FileName = null };
            var result = _mapperUnderTest.ToReportViewModel(new List<ReportDTO> { itemWithNullFields });
            Assert.IsTrue(result.First().FileName == null);
        }
    }
}
