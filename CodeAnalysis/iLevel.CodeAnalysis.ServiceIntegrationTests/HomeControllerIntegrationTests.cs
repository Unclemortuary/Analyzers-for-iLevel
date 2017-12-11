using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iLevel.CodeAnalysis.ServiceIntegrationTests.Common;

namespace iLevel.CodeAnalysis.ServiceIntegrationTests
{
    [TestClass]
    public class HomeControllerIntegrationTests
    {
        private const string _folderName = "CodeAnalysisService";
        private const int _port = 8080;

        [TestMethod]
        [TestCategory("Integration")]
        public void HomeControllerPositiveTest()
        {
            AppHostLauncher hostLauncher = new AppHostLauncher(_folderName, _port);
            hostLauncher.Launch();
        }
    }
}
