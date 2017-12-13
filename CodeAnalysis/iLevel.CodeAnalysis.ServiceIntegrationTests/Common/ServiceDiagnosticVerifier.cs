using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iLevel.CodeAnalysis.ServiceIntegrationTests.Common
{
    static class ServiceDiagnosticVerifier
    {
        public static void Verify(ServiceDiagnosticResult expected, string actual)
        {
            if(CheckLocationCorresponding(ref actual, expected.Location))
                if (true)
                {

                }
            Assert.IsTrue(true);
        }

        private static bool CheckLocationCorresponding(ref string diagnostic, Location expectedLocation)
        {
            string stringLocation = "(" + expectedLocation.Line + "," + expectedLocation.Column + ")";
            if (!diagnostic.Contains(expectedLocation.FileName))
                throw new AssertFailedException(
                    string.Format("File name (\"{0}\") not found in received diagnostic : {1}", expectedLocation.FileName, diagnostic));
            else
            {
                diagnostic = ExcludeSubstring(diagnostic, expectedLocation.FileName);
                if (!diagnostic.Contains(stringLocation))
                    throw new AssertFailedException(
                        string.Format("Diagnostic message with location {0} not found in received diagnostic : {1}", stringLocation, diagnostic));
                else
                {
                    diagnostic = ExcludeSubstring(diagnostic, stringLocation);
                    return true;
                }
            }
        }

        private static string ExcludeSubstring(string baseString, string subString)
        {
            var startIndex = baseString.IndexOf(subString);
            return baseString.Remove(startIndex, subString.Length);
        }
    }
}
