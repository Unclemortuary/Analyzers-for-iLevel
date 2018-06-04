using System.Linq;
using System.Collections.Immutable;
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

                if (propDeclarationSymbol.ContainingNamespace.ToString().StartsWith(Constants.DomainEntitiesNamespace))
                {
                    if (propDeclarationSymbol.SetMethod != null)
                    {
                        var containingClass = propDeclarationSymbol.ContainingSymbol as ITypeSymbol;

                        if (containingClass?.TypeKind == TypeKind.Class)
                        {
                            if (containingClass.Interfaces.Any())
                            {
                                for (int i = 0; i < containingClass.Interfaces.Count(); i++)
                                {
                                    var _interface = containingClass.Interfaces[i];

                                    if (IsInterfacePropertyImmutable(_interface, propDeclarationSymbol))
                                        return;
                                }

                                ctx.ReportDiagnostic(Diagnostic.Create(Rule, propDeclarationSymbol.SetMethod.Locations.First()));
                            }
                        }
                    }
                }
            }
        }

        private bool IsInterfacePropertyImmutable(INamedTypeSymbol baseInterface, IPropertySymbol property)
        {
            var interfaceProperty = baseInterface.GetMembers(property.Name)
                .OfType<IPropertySymbol>()
                .FirstOrDefault(p => p.Type == property.Type);

            return interfaceProperty == null ? false : interfaceProperty.SetMethod == null;
        }
    }
}
