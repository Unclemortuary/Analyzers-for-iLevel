using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using System.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeActions;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Rename;

namespace iLevel.ViewPoint.CodeAnalysis.BestPractices
{
    /// <summary>
    /// Fix methods that have argument names that end on underscore
    /// void Method(Object arg_) => void Method(Object arg)
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ArgumentTrailingUnderscoreFixProvider)), Shared]
    public class ArgumentTrailingUnderscoreFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ArgumentTrailingUnderscoreAnalyzer.DiagnosticId);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            Diagnostic diagnostic = context.Diagnostics.Single();

            var node = root.FindNode(diagnostic.Location.SourceSpan);

            CodeAction codeAction = CodeAction.Create("Rename",
                createChangedSolution: t => RemoveUglyTail(context.Document, node, t));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private async Task<Solution> RemoveUglyTail(Document document, SyntaxNode node, CancellationToken token)
        {

            if (node is ParameterSyntax parameter)
            {
                string old = parameter.Identifier.Text;
                string fxd = old.TrimEnd('_');

                var model = await document.GetSemanticModelAsync(token).ConfigureAwait(false);
                IParameterSymbol symbol = model.GetDeclaredSymbol(parameter, token);
                
                return await Renamer.RenameSymbolAsync(document.Project.Solution, symbol, fxd, document.Project.Solution.Options, token);
            }
            else
            {
                throw new InvalidOperationException($"{node.GetType()}: {node.GetText()}");
            }
        }
    }
}
