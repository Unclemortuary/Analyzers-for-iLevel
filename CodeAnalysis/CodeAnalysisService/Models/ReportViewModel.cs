namespace CodeAnalysisService.Models
{
    public class ReportViewModel
    {
        public string FileName { get; set; }
        public string Locatin { get; set; }
        public string AnalyzerID { get; set; }
        public string Message { get; set; }
        public string Severety { get; set; }
    }
}