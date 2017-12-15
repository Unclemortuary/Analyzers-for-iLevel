﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayer.Intefaces
{
    public interface ISolutionProvider
    {
        IEnumerable<SyntaxTree> GetSyntaxTrees(Dictionary<string, string> sources);
        CSharpCompilation GetCompilation(IEnumerable<SyntaxTree> syntaxTrees, string assemblyName);
        Project GetProject(Dictionary<string, string> sources, string projectName = null);
    }
}
