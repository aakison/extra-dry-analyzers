using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EnumShouldHaveJsonConverter : DryDiagnosticNodeAnalyzer {

        public EnumShouldHaveJsonConverter() : base(
            SyntaxKind.EnumDeclaration,
            1301,
            DryAnalyzerCategory.OpenApiDocs,
            DiagnosticSeverity.Info,
            "Enum should be decoreated with JsonConverter.",
            "Enum '{0}' should add [JsonConverter(typeof(JsonStringEnumConverter))].",
            "Enums that are sent through OpenAPI should use strings to identify values instead of integers.  This is aligned with OpenAPI and cross-platform usage and provides better documentation for API developers.  As a good practice, use this attribute consistently throughout the project on all Enums."
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
