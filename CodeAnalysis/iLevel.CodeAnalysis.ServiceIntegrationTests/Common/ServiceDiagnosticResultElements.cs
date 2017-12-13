namespace iLevel.CodeAnalysis.ServiceIntegrationTests.Common
{
    public struct ServiceDiagnosticResult
    {
        public Location Location;
        public SeveretyType SeveretyType;
        public string AnalyzerMessage;
        public string AnalyzerID;
    }

    public struct Location
    {
        public Location(string name, int line, int column)
        {
            FileName = name;
            Line = line;
            Column = column;
        }
        public string FileName;
        public int Line;
        public int Column;
    }

    public enum SeveretyType
    {
        Error = 0,
        Warning = 1
    }
}
