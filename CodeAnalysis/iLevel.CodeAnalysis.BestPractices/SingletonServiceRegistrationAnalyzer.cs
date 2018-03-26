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
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => throw new NotImplementedException();

        public override void Initialize(AnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }

    //TODO:
    //1) - check that invokationExpression is an IServiceCollection method
    //2) - check that the parameter is ends with "Singleton"
    //3) - do the same for the generic parameter
    //4) - check that method symbol name is a AddSingleton, if not - create a rule
    //5) - figure out how to handle overloads with Func
}
