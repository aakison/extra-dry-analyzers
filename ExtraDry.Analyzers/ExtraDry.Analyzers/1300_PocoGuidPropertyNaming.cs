using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PocoGuidPropertyNaming : DryDiagnosticNodeAnalyzer {

        public PocoGuidPropertyNaming() : base(
            SyntaxKind.PropertyDeclaration,
            1300,
            DryAnalyzerCategory.OpenApiDocs,
            DiagnosticSeverity.Info,
            "Guid properties should be named Uuid.",
            "Guid property '{0}' should change name, preferably to 'Uuid'.",
            "The Guid property on a entity that is returned from OpenAPI should have naming conventions that are aligned with OpenAPI and cross-platform usage (the name Guid predates Uuid and is only common in the .NET ecosystem).  As a good practice, use this naming scheme consistently throughout project."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var property = (PropertyDeclarationSyntax)context.Node;
            var badName = property.Identifier.ValueText.Equals("Guid", StringComparison.OrdinalIgnoreCase);
            if(!badName) {
                return;
            }
            var isPublic = HasVisibility(property, Visibility.Public);
            if(!isPublic) {
                return;
            }
            var typeName = property.Type.ToString();
            var isGuid = typeName == "Guid" || typeName == "System.Guid"; // Worry about retrieving context, or good enough?
            if(!isGuid) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, property.Identifier.GetLocation(), property.Identifier.ValueText));
        }

    }
}
