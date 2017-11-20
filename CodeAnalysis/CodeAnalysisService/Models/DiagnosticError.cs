using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeAnalysisService.Models
{
    public class DiagnosticError
    {
        public string FileName { get; set; }
        public string WarningText { get; set; }

    }
}