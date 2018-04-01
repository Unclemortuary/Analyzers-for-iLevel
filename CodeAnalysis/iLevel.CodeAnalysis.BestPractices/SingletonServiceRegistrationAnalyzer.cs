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
    class SingletonServiceRegistrationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ILVL0004";
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
                        if (!memberSymbol.IsGenericMethod) //We already have analyzer for generics
                        {
                            if (invocation.ArgumentList.Arguments.Count > 0)
                            {
                                var firstArgument = invocation.ArgumentList.Arguments.First();

                                if (firstArgument.Expression is SimpleLambdaExpressionSyntax lambda)
                                {
                                    if (lambda.Body is ObjectCreationExpressionSyntax objectCreation) //if we simply create new object via lambda
                                    {
                                        if (objectCreation.Type?.ToString().Contains("Singleton") ?? false)
                                        {
                                            ctx.ReportDiagnostic(Diagnostic.Create(Rule, GetNodeLocation(ctx.Node)));
                                        }
                                    }
                                    else //Handle only case when we pass to lambda already created object 
                                    {
                                        var argumentType = ctx.SemanticModel.GetTypeInfo(lambda.Body);

                                        if (argumentType.Type?.ToString().Contains("Singleton") ?? false)
                                        {
                                            ctx.ReportDiagnostic(Diagnostic.Create(Rule, GetNodeLocation(ctx.Node))); // TODO : pass location of MethodExpression
                                        }
                                    }
                                }
                                else //First argument of non lambda Add.. methods is always Type
                                {
                                    if (firstArgument.Expression is TypeOfExpressionSyntax typeofExpression) //Handle only typeof case
                                    {
                                        if (typeofExpression.Type?.ToString().Contains("Singleton") ?? false)
                                        {
                                            var loca = GetNodeLocation(ctx.Node);

                                            ctx.ReportDiagnostic(Diagnostic.Create(Rule, GetNodeLocation(ctx.Node))); // TODO : pass location of MethodExpression
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private Location GetNodeLocation(SyntaxNode node)
        {
            var methodIdentifierName = node.DescendantNodes()
                .Where(n => n is IdentifierNameSyntax identifier && identifier.ToString().StartsWith("Add"))
                .First();
            return methodIdentifierName.GetLocation();
        }
    }
}
