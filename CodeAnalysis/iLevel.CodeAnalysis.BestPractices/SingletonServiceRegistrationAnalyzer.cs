using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;

namespace iLevel.CodeAnalysis.BestPractices
{
    /// <summary>
    /// Analyze registration in .net core DI container services that ends with singleton (ex. - ConnectionServiceSingleton)
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SingletonServiceRegistrationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ILVL0003";

        private static readonly LocalizableString Title = "Singleton service registered in wrong way";
        private static readonly LocalizableString MessageFormat = "Singleton service must be registered throught the method \"AddSingleton\" in order to avoid possible perfomance reducing";
        private const string Category = "iLevel.BestPractises";

        private const string NeededNamespace = "Microsoft.Extensions.DependencyInjection.IServiceCollection";

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
                if (memberAccessExpression.Name.ToString().StartsWith("Add"))
                {
                    var memberSymbol = ctx.SemanticModel.GetSymbolInfo(invocation.Expression).Symbol as IMethodSymbol;
                    if (memberSymbol != null && memberSymbol.ToString().StartsWith(NeededNamespace))
                    {
                        if (invocation.ArgumentList.Arguments.Count > 0)
                        {
                            var typeArgument = invocation.ArgumentList.Arguments.First();
                            if (typeArgument.Expression is ObjectCreationExpressionSyntax objectCreation)
                            {
                                //TODO : check type of created object;
                            }
                            else
                            {
                                if (memberAccessExpression.Name.ToString() != "AddSingleton")
                                {
                                    ctx.ReportDiagnostic(Diagnostic.Create(Rule, ctx.Node.GetLocation())); // TODO : pass location of MethodExpression
                                }
                            }
                        }
                        else //TODO : check if we works with generic overload
                        {

                        }
                    }
                }
            }
        }
    }

    //TODO:
    
    
    //3) - check that method symbol name is a AddSingleton, if not - create a rule
    //4) - do the same for the generic parameter
}
