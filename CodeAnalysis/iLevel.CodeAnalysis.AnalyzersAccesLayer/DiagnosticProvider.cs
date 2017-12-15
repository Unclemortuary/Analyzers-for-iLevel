using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayer
{
    public interface IDiagnosticProvider
    {
        Report GetAnalyzersDiagnostic();
        Report GetCompilationDiagnostic();
    }

    class DiagnosticProvider : IDiagnosticProvider
    {
        public Report GetAnalyzersDiagnosticReport()
        {

        }

        public Report GetCompilationDiagnostic()
        {

        }

        private DiagnosticResult[] GetUnsortedDiagnostic()
        {

        }
    }
}
