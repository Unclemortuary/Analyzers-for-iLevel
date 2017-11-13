using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeAnalysisService.Models
{
    public class SingleSolution
    {
        List<CodeFile> _sourceCode;
    }

    struct CodeFile
    {
        string name;
        string path;
        string text;
    }
}