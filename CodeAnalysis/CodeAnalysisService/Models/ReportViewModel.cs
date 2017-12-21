using System.ComponentModel.DataAnnotations;

namespace CodeAnalysisService.Models
{
    public class ReportViewModel
    {
        [Display(Name = "File Name")]
        public string FileName { get; set; }
        public string Location { get; set; }
        public string AnalyzerID { get; set; }
        public string Message { get; set; }
        public string Severety { get; set; }
    }
}