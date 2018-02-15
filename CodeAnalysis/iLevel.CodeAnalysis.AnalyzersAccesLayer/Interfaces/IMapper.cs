using iLevel.CodeAnalysis.BusinessLogicLayer.DTO;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayer.Interfaces
{
    public interface IMapper
    {
        IEnumerable<ReportDTO> ToReportDTO(IEnumerable<Diagnostic> diagnostic);
    }
}
