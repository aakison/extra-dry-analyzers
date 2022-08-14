using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConsumesAttributeHasValidMimeType : DryDiagnosticNodeAnalyzer {

        public ConsumesAttributeHasValidMimeType() : base(
            SyntaxKind.MethodDeclaration,
            1202,
            DryAnalyzerCategory.OpenApiDocs,
            DiagnosticSeverity.Warning,
            "ConsumesAttribute should have a preferred mime type.",
            "ConsumesAttribute should have either 'application/json' or 'multipart/form-data'.",
            "The Consumes attribute indicates to API consumers what the type of the response payload will be.  Consumers have specific expectations on these types and limiting to the smallest set of necessary mime types simplifies API consumption."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var hasProducesAttribute = HasAnyAttribute(context, method, out var producesAttribute, "Consumes");
            if(!hasProducesAttribute) {
                return;
            }
            var _class = ClassForMember(method);
            var hasApiController = HasAttribute(context, _class, "ApiController", out var _);
            if(!hasApiController) {
                return;
            }
            if(producesAttribute.ArgumentList?.Arguments.Count == 1) {
                var argument = FirstArgument(producesAttribute);
                if(argument is LiteralExpressionSyntax stringArgument) {
                    var literalValue = stringArgument.Token.ValueText;
                    if(literalValue == "application/json" || literalValue == "multipart/form-data") {
                        return;
                    }
                }
                else if(argument is MemberAccessExpressionSyntax member) {
                    // e.g. MediaTypeNames.Application.Json
                    var text = member.Name.Identifier.ValueText;
                    if(text == "Json") {
                        return;
                    }
                }
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, producesAttribute.GetLocation()));
        }

    }
}
