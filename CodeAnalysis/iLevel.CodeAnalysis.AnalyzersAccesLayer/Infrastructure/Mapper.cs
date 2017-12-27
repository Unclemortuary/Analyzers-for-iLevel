using iLevel.CodeAnalysis.AnalyzersAccesLayer.Interfaces;
using DTO = iLevel.CodeAnalysis.BusinessLogicLayer.DTO;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayer.Infrastructure
{
    class Mapper : IMapper
    {
        public IEnumerable<DTO.ReportDTO> ToReportDTO(IEnumerable<Diagnostic> diagnostic)
        {
            List<DTO.ReportDTO> result = new List<DTO.ReportDTO>();
            foreach (var report in diagnostic)
            {
                result.Add(Map(report));
            }
            return result;
        }

        private DTO.ReportDTO Map(Diagnostic diagnostic)
        {
            DTO.ReportDTO report = new DTO.ReportDTO();
            report.FileName = diagnostic?.Location?.SourceTree?.FilePath ?? "";
            report.Location = new DTO.Location {
                Line = diagnostic.Location.GetLineSpan().StartLinePosition.Line,
                Column = diagnostic.Location.GetLineSpan().StartLinePosition.Character };
            report.Severety = diagnostic.Severity.ToString();
            report.AnalyzerID = diagnostic.Id;
            report.Message = diagnostic.GetMessage();
            return report;
        }
    }
}
