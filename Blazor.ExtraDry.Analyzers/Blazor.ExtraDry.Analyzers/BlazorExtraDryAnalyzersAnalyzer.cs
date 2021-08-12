using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Blazor.ExtraDry.Analyzers {
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BlazorExtraDryAnalyzersAnalyzer : DiagnosticAnalyzer {
        public const string DiagnosticId = "BlazorExtraDryAnalyzers";
        private static readonly LocalizableString Title = "Variable can be made constant";
        private static readonly LocalizableString MessageFormat = "Variable '{0}' can be made constant";
        private static readonly LocalizableString Description = "Variables that are not modified should be made constants.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics {
            get { return ImmutableArray.Create(Rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.LocalDeclarationStatement);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;
            // make sure the declaration isn't already const:
            if(localDeclaration.Modifiers.Any(SyntaxKind.ConstKeyword)) {
                return;
            }

            // Ensure that all variables in the local declaration have initializers that
            // are assigned with constant values.
            foreach(var variable in localDeclaration.Declaration.Variables) {
                var initializer = variable.Initializer;
                if(initializer == null) {
                    return;
                }

                var constantValue = context.SemanticModel.GetConstantValue(initializer.Value, context.CancellationToken);
                if(!constantValue.HasValue) {
                    return;
                }
            }

            // Perform data flow analysis on the local declaration.
            var dataFlowAnalysis = context.SemanticModel.AnalyzeDataFlow(localDeclaration);

            foreach(var variable in localDeclaration.Declaration.Variables) {
                // Retrieve the local symbol for each variable in the local declaration
                // and ensure that it is not written outside of the data flow analysis region.
                var variableSymbol = context.SemanticModel.GetDeclaredSymbol(variable, context.CancellationToken);
                if(dataFlowAnalysis.WrittenOutside.Contains(variableSymbol)) {
                    return;
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation(), localDeclaration.Declaration.Variables.First().Identifier.ValueText));
        }

    }
}
