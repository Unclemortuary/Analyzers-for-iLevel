using System.Collections.Generic;
using iLevel.CodeAnalysis.BusinessLogicLayer.DTO;
using Microsoft.CodeAnalysis.Diagnostics;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayer.Interfaces
{
    public interface IDiagnosticProvider
    {
        IEnumerable<ReportDTO> GetDiagnostic(IEnumerable<SourceFileDTO> sources, HashSet<DiagnosticAnalyzer> analyzers);
    }
}
