using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iLevel.CodeAnalysis.ServiceIntegrationTests.Common
{
    class AppHostLauncher
    {
        private readonly string _testedProjectFolderName;
        private readonly int _port;

        public AppHostLauncher(string folderName, int port)
        {
            _testedProjectFolderName = folderName;
            _port = port;
        }

        public void Launch()
        {
            var appParth = GetAppPath();
            string programFiles = Environment.GetEnvironmentVariable("ProgramFiles");
            var iisProcess = new Process();
            iisProcess.StartInfo.FileName = programFiles + @"\IIS Express\iisexpress.exe";
            iisProcess.StartInfo.Arguments = string.Format("/path:\"{0}\" /port:{1}", appParth, _port);
            iisProcess.Start();
        }

        private string GetAppPath()
        {
            string currentPath = Environment.CurrentDirectory;
            var result = Path.GetFullPath(
                Path.Combine(currentPath, string.Format("../../../{0}", _testedProjectFolderName))
                );
            return result;
        }
    }
}
