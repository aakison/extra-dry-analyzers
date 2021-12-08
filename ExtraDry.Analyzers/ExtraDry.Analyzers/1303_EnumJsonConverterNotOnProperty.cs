using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EnumJsonConverterNotOnProperty : DryDiagnosticNodeAnalyzer {

        public EnumJsonConverterNotOnProperty() : base(
            SyntaxKind.PropertyDeclaration,
            1303,
            DryAnalyzerCategory.OpenApiDocs,
            DiagnosticSeverity.Info,
            "JsonConverter should be on Enum instead of Property.",
            "Property '{0}' should not have JsonConverter, put [JsonConverter(typeof(JsonStringEnumConverter))] on enum instead.",
            "Enums that are sent through OpenAPI should use consistent strings to identify values.  To ensure this is done consistently, put the converter on the enum, this will prevent new APIs from accidentally reverting to integers."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var _enum = (EnumDeclarationSyntax)context.Node;
            var isPublic = HasVisibility(_enum, Visibility.Public);
            if(!isPublic) {
                return;
            }
            var isDecorated = HasAttribute(context, _enum, "JsonConverter", out var _);
            if(isDecorated) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, _enum.Identifier.GetLocation(), _enum.Identifier.ValueText));
        }

    }
}
