using System.Linq;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using iLevel.CodeAnalysis.BestPractices.Common;

namespace iLevel.CodeAnalysis.BestPractices
{
    /// <summary>
    /// Analyze correct registration in .net core DI container services that have "Singleton" phrase in name(ex. - ConnectionServiceSingleton)
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SingletonServiceRegistrationForGenericAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ILVL0003";
        private static readonly LocalizableString Title = "Singleton service registered in wrong way";
        private static readonly LocalizableString MessageFormat = "Singleton service must be registered throught the method \"AddSingleton\" in order to avoid possible perfomance reducing";
        private const string Category = "iLevel.BestPractises";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Do, SyntaxKind.InvocationExpression);
        }

        private void Do(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.Node is InvocationExpressionSyntax invocation)
            {
                var memberAccessExpression = invocation.Expression as MemberAccessExpressionSyntax;

                if (Constants.NotSingletonAddServiceMethodNames.Select(x => x.Contains(memberAccessExpression.Name.ToString())).Any())
                {
                    var memberSymbol = ctx.SemanticModel.GetSymbolInfo(invocation.Expression).Symbol as IMethodSymbol;

                    if (memberSymbol != null && memberSymbol.ToString().StartsWith(Constants.IServiceCollectionNamespace))
                    {
                        if (memberSymbol.IsGenericMethod)
                        {
                            var firstGenericArgument = memberSymbol.TypeArguments.First();

                            var name = firstGenericArgument?.Name.ToString();

                            if (firstGenericArgument.ToString().Contains("Singleton"))
                            {
                                var genericSyntaxNode = ctx.Node.DescendantNodes().Where(n => n is GenericNameSyntax).First();

                                ctx.ReportDiagnostic(Diagnostic.Create(Rule, genericSyntaxNode.GetLocation()));
                            }
                        }
                    }
                }
            }
        }
    }
}
