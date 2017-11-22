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

    }
}