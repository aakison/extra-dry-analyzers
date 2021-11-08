using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ProducesAttributeHasValidMimeType : DryDiagnosticNodeAnalyzer {

        public ProducesAttributeHasValidMimeType() : base(
            SyntaxKind.MethodDeclaration,
            1201,
            DryAnalyzerCategory.OpenApiDocs,
            DiagnosticSeverity.Warning,
            "ProducesAttribute should have a preferred mime type.",
            "ProducesAttribute should have either 'application/json' or 'application/octet-stream'.",
            "The Produces attribute indicates to API consumers what the type of the response payload will be.  Consumers have specific expectations on these types and limiting to the smallest set of necessary mime types simplifies API consumption."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var hasProducesAttribute = HasAnyAttribute(context, method, out var producesAttribute, "Produces");
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
                    // e.g. "application/json"
                    var literalValue = stringArgument.Token.ValueText;
                    if(literalValue == "application/json" || literalValue == "application/octet-stream") {
                        return;
                    }
                }
                else if(argument is MemberAccessExpressionSyntax member) {
                    // e.g. MediaTypeNames.Application.Json
                    var text = member.Name.Identifier.ValueText;
                    if(text == "Json" || text == "Octet") {
                        return;
                    }
                }
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, producesAttribute.GetLocation()));
        }

    }
}
