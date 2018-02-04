using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;

namespace iLevel.ViewPoint.CodeAnalysis.BestPractices
{

    /// <summary>
    /// Analyzer argument names that end on underscore
    /// void Method(Object arg_)
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ArgumentTrailingUnderscoreAnalyzer : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "ILVL0001";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, "Ugly argument name", "Rename argument name", "iLevel.BestPractices", DiagnosticSeverity.Warning, true);

        ///
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        ///
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Do, SyntaxKind.Parameter);
        }

        private void Do(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ParameterSyntax parameter &&
                parameter.Identifier.Text.EndsWith("_"))
            {
                var diagnostic = Diagnostic.Create(Rule, parameter.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
