using System.Collections.Generic;
using System.Linq;
using Moq;
using System.IO;
using System.Web;

namespace iLevel.CodeAnalysis.ServiceIntegrationTests.Common
{
    class MIMEFIlesRequestMocker
    {
        public static HttpContextBase CreateHttpContextMockWithFilesFromStringPairs(Dictionary<string, string> stringPairs)
        {
            Mock<HttpFileCollectionBase> filesMock = new Mock<HttpFileCollectionBase>();
            Mock<HttpRequestBase> requestMock = new Mock<HttpRequestBase>();
            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            List<HttpPostedFileBase> postedFiles = new List<HttpPostedFileBase>();

            foreach (var pair in stringPairs)
            {
                Mock<HttpPostedFileBase> postedFileMock = new Mock<HttpPostedFileBase>();
                postedFileMock.SetupGet(f => f.FileName).Returns(pair.Key + ".cs");
                postedFileMock.SetupGet(f => f.ToString()).Returns(pair.Key);
                postedFileMock.SetupGet(f => f.InputStream).Returns(CreateStreamFromString(pair.Value));
                postedFiles.Add(postedFileMock.Object);
                filesMock.Setup(f => f[pair.Key]).Returns(postedFileMock.Object);
            }

            filesMock.SetupGet(
                x => x.Count).Returns(postedFiles.Count);
            filesMock.Setup(f => f.GetEnumerator())
                .Returns(postedFiles.GetEnumerator());
            

            requestMock.Setup(r => r.Files).Returns(filesMock.Object);
            contextMock.Setup(c => c.Request).Returns(requestMock.Object);

            return contextMock.Object;
        }

        public static Stream CreateStreamFromString(string text)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(memoryStream);
                writer.Write(text);
                writer.Flush();
                memoryStream.Position = 0;
                return memoryStream;
            }
        }
    }
}
