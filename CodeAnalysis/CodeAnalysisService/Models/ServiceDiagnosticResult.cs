using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeAnalysisService.Models
{
    public class ServiceDiagnosticResult
    {
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
    }
}