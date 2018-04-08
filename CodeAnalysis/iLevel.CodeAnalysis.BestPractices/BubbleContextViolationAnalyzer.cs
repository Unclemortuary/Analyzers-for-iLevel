using System.Linq;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using iLevel.CodeAnalysis.BestPractices.Common;

namespace iLevel.CodeAnalysis.BestPractices
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BubbleContextViolationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ILVL005";
        private static readonly LocalizableString Title = "Bubble context violation for filters";
        private static readonly LocalizableString MessageFormat = "Filter's entities should not use grid service methods";
        private const string Category = "ilevel.BestPractises";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSemanticModelAction(Do);
        }

        private void Do(SemanticModelAnalysisContext ctx)
        {
            if (ctx.SemanticModel.SyntaxTree.FilePath.Contains(Constants.FiltersServiceDocumentName))
            {
                var root = ctx.SemanticModel.SyntaxTree.GetRoot();

                var allInvocations = root.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().ToList();

                if (allInvocations.Any())
                {
                    var semanticModel = ctx.SemanticModel;

                    foreach (var invocation in allInvocations)
                    {
                        var symbolInfo = semanticModel.GetSymbolInfo(invocation);

                        if (symbolInfo.Symbol?.ContainingType.ToString().Contains(Constants.GridDataServiceDocumentName) ?? false)
                        {
                            ctx.ReportDiagnostic(Diagnostic.Create(Rule, invocation.GetLocation()));
                        }
                    }
                }
            }
        }
    }
}
