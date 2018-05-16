using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using iLevel.CodeAnalysis.BestPractices.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace iLevel.CodeAnalysis.BestPractices
{
    /// <summary>
    /// Checks implementation of immutability of domain entities. Next implementations supported:
    /// 1) Only "get" properties
    /// 2) Existance of immutable interface
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ImmutableDomainEntitiesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ILVL006";
        private static readonly LocalizableString Title = "Domain immutability violation";
        private static readonly LocalizableString MessageFormat = "Domain entities should be immutable";
        private const string Category = "ilevel.BestPractises";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Do, SyntaxKind.PropertyDeclaration);
        }

        private void Do(SyntaxNodeAnalysisContext ctx)
        {
            if (ctx.Node is PropertyDeclarationSyntax propDeclaration)
            {
                var propDeclarationSymbol = ctx.SemanticModel.GetDeclaredSymbol(propDeclaration);

                var sus = propDeclarationSymbol.ContainingNamespace;

                if (propDeclarationSymbol.ContainingNamespace.ToString().StartsWith(Constants.DomainEntitiesNamespace))
                {
                    if (propDeclarationSymbol.SetMethod != null)
                    {
                        var containingClass = propDeclarationSymbol.ContainingSymbol as INamedTypeSymbol;

                        if (containingClass?.TypeKind == TypeKind.Class)
                        {
                            var baseType = containingClass.BaseType;

                            if (baseType?.TypeKind == TypeKind.Interface)
                            {
                                if (IsInterfaceAreImmutable(baseType))
                                {
                                    ctx.ReportDiagnostic(Diagnostic.Create(Rule, propDeclarationSymbol.SetMethod.Locations.First()));
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool IsInterfaceAreImmutable(INamedTypeSymbol baseInterface)
        {
            throw new NotImplementedException();
        }
    }
}
