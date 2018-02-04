using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace iLevel.CodeAnalysis.BestPractices
{
    /// <summary>
    /// Analyzer execution of methods that are described in iLevel.ViewPoint.BusinessLogic.Services
    /// Usually we use cache in these methods, but the new implementation of cache uses Change Tracking.
    /// So, each invocation of the cached method causes a new connection to DB.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ServiceMethodInLoopExecutionAnalyzer : DiagnosticAnalyzer
    {
        private const string ServicesNamespace = "iLevel.ViewPoint.BusinessLogic.Services";
        private static readonly IReadOnlyList<Type> LoopAncestors = new[]
        {
            typeof(ForStatementSyntax),
            typeof(ForEachStatementSyntax),
            typeof(WhileStatementSyntax),
            typeof(DoStatementSyntax)
        };

        internal const string DiagnosticId = "ILVL0002";
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, "Service method in loop",
            "Service method shouldn't be executed in a loop because there is no guarantee that cache has initialized properly",
            "iLevel.BestPractices", DiagnosticSeverity.Warning, true);

        ///
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        ///
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Do, SyntaxKind.InvocationExpression);
        }

        private void Do(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.Node is InvocationExpressionSyntax invocation)
            {
                var symbol = ctx.SemanticModel.GetSymbolInfo(invocation);
                if (symbol.Symbol?.ContainingNamespace.ToString() == ServicesNamespace)
                {
                    if (invocation.Ancestors().Any(a => LoopAncestors.Contains(a.GetType())))
                    {
                        var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation());
                        ctx.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}
