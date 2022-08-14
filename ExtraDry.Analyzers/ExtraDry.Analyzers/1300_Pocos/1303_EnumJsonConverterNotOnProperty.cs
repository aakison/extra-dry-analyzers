using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Linq;

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
            var property = (PropertyDeclarationSyntax)context.Node;
            var isPublic = HasVisibility(property, Visibility.Public);
            if(!isPublic) {
                return;
            }
            var isDecorated = HasAttribute(context, property, "JsonConverter", out var jsonConverterAttribute);
            if(!isDecorated) {
                return;
            }
            var _class = ClassForMember(property);
            if(!HasVisibility(_class, Visibility.Public)) {
                return;
            }
            var converterArgument = FirstArgument(jsonConverterAttribute);
            if(converterArgument == null) {
                return;
            }
            var converterType = converterArgument.DescendantNodes().FirstOrDefault(e => e.IsKind(SyntaxKind.IdentifierName)) as IdentifierNameSyntax;
            if(converterType == null) {
                return;
            }
            if(converterType.Identifier.ValueText != "JsonStringEnumConverter") {
                return; // custom converters not covered here.
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, property.Identifier.GetLocation(), property.Identifier.ValueText));
        }

    }

}
