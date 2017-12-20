using System;
using iLevel.CodeAnalysis.BusinessLogicLayer.DTO;
using Microsoft.CodeAnalysis;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayer.Infrastructure
{
    class Mapper
    {
        public static ReportDTO Map(Diagnostic diagnostic)
        {
            ReportDTO report = new ReportDTO();
            report.FileName = diagnostic.Location.SourceTree.FilePath ?? "";
            report.Location = diagnostic.Location.SourceSpan.ToString() ?? "";
            report.Severety = diagnostic.Severity.ToString();
            report.AnalyzerID = diagnostic.Id;
            report.Message = diagnostic.GetMessage();
            return report;
        }
    }
}
