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
            report.Severety = diagnostic.Severity.ToString();
            report.Report = "//" + diagnostic.ToString() + Environment.NewLine;
            return report;
        }
    }
}
