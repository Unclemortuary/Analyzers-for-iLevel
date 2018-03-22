using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace iLevel.CodeAnalysis.BestPractices
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HelloWorldAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "HelloWorldAnalyzer";

        private static readonly LocalizableString Title = "Hello World in the console";
        private static readonly LocalizableString MessageFormat = "Maybe should write \"Hello world\" instead of an empty message";
        private const string Category = "iLevel.BestPractices";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Do, SyntaxKind.InvocationExpression);
        }

        private void Do(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax invocation &&
                invocation.ArgumentList.Arguments.Count == 0)
            {
                var symbol = context.SemanticModel.GetSymbolInfo(invocation);
                if (symbol.Symbol?.Name == "WriteLine" &&
                    symbol.Symbol?.ContainingSymbol.Name == "Console" &&
                    symbol.Symbol?.ContainingNamespace.Name == "System")
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
                }
            }
        }
    }
}
