using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace iLevel.CodeAnalysis.BestPractices
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HelloWorldAnalyzerCodeFixProvider)), Shared]
    public class HelloWorldAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Make Hello World";

        private const string argument = "Hello World!";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(HelloWorldAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.Single();

            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var node = root.FindNode(diagnosticSpan);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => MakeHelloWorldAsync(context.Document, node, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Document> MakeHelloWorldAsync(Document doc, SyntaxNode invocation, CancellationToken token)
        {
            if (invocation is InvocationExpressionSyntax inv)
            {
                var literalExpression = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(argument));

                var invocationWithArgument = inv.AddArgumentListArguments(SyntaxFactory.Argument(literalExpression));

                var oldRoot = await doc.GetSyntaxRootAsync(token).ConfigureAwait(false);

                var newRoot = oldRoot.ReplaceNode(invocation, invocationWithArgument);

                return doc.WithSyntaxRoot(newRoot);
            }
            else
            {
                throw new InvalidOperationException($"{invocation.GetType()}: {invocation.GetText()}");
            }
        }
    }
}
