namespace iLevel.CodeAnalysis.BusinessLogicLayer.DTO
{
    public class ReportDTO
    {
        public string FileName { get; set; }
        public Location Location { get; set; }
        public string AnalyzerID { get; set; }
        public string Severety { get; set; }
        public string Message { get; set; }
    }

    public struct Location
    {
        public int Line;
        public int Column;
    }
}
