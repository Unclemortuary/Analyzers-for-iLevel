using iLevel.CodeAnalysis.AnalyzersAccesLayer.Interfaces;
using iLevel.CodeAnalysis.BusinessLogicLayer.DTO;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayer.Infrastructure
{
    class Mapper : IMapper
    {
        public IEnumerable<ReportDTO> ToReportDTO(IEnumerable<Diagnostic> diagnostic)
        {
            List<ReportDTO> result = new List<ReportDTO>();
            foreach (var report in diagnostic)
            {
                result.Add(Map(report));
            }
            return result;
        }

        private ReportDTO Map(Diagnostic diagnostic)
        {
            ReportDTO report = new ReportDTO();
            report.FileName = diagnostic?.Location?.SourceTree?.FilePath ?? "";
            report.Location = diagnostic?.Location?.SourceSpan.ToString() ?? "";
            report.Severety = diagnostic.Severity.ToString();
            report.AnalyzerID = diagnostic.Id;
            report.Message = diagnostic.GetMessage();
            return report;
        }
    }
}
