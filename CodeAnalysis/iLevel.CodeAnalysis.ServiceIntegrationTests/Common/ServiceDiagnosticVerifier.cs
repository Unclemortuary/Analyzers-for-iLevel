using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iLevel.CodeAnalysis.ServiceIntegrationTests.Common
{
    static class ServiceDiagnosticVerifier
    {
        public static void Verify(ServiceDiagnosticResult expected, string actual)
        {
            if (CheckLocationCorresponding(ref actual, expected.Location))
                if (actual.Contains(expected.SeveretyType.ToString()))
                {
                    actual = ExcludeSubstring(actual, expected.SeveretyType.ToString());
                    if (actual.Contains(expected.AnalyzerID))
                    {
                        actual = ExcludeSubstring(actual, expected.AnalyzerID);
                        if (actual.Contains(expected.DiagnosticMessage))
                            Assert.IsTrue(true);
                        else
                            throw new AssertFailedException(
                                string.Format("Diagnostic messages not match (expected : \"{0}\" ; actual : \"{1}\")",
                                    expected.DiagnosticMessage, actual));
                    }
                    else
                        throw new AssertFailedException(
                            string.Format("Analyzer IDs not match (\"{0}\" expected)",expected.AnalyzerID));
                }
                else
                    throw new AssertFailedException(string.Format(
                        "Severetys not matchs ({0} expected)", expected.SeveretyType.ToString()));
        }

        private static bool CheckLocationCorresponding(ref string diagnostic, Location expectedLocation)
        {
            
            string stringLocation = "(" + expectedLocation.Line + "," + expectedLocation.Column + ")";
            if (!diagnostic.Contains(expectedLocation.FileName))
                throw new AssertFailedException(
                    string.Format("File name (\"{0}\") not found in actual diagnostic : {1}",
                    expectedLocation.FileName, diagnostic));
            else if (expectedLocation.Line != null && expectedLocation.Column != null)
            {
                diagnostic = ExcludeSubstring(diagnostic, expectedLocation.FileName);
                if (!diagnostic.Contains(stringLocation))
                    throw new AssertFailedException(
                        string.Format("Diagnostic message with location {0} not found in actual diagnostic : {1}",
                            stringLocation, diagnostic));
                else
                {
                    diagnostic = ExcludeSubstring(diagnostic, stringLocation);
                    return true;
                }
            }
            else
                return true;
        }

        private static string ExcludeSubstring(string baseString, string subString)
        {
            var startIndex = baseString.IndexOf(subString);
            return baseString.Remove(startIndex, subString.Length);
        }
    }
}
