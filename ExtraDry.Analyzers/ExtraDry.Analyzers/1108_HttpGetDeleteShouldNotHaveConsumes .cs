using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpGetDeleteShouldNotHaveConsumes : DryDiagnosticNodeAnalyzer {

        public HttpGetDeleteShouldNotHaveConsumes() : base(
            SyntaxKind.MethodDeclaration,
            1108,
            DryAnalyzerCategory.ApiUsage,
            DiagnosticSeverity.Info,
            "HttpGet and HttpDelete methods should not have Consumes attribute",
            "Method `{0}` should not declare that it consumes a body.",
            "The Consumes attribute indicates to API consumers what the type of the payload should be.  Listing, retreiving and deleting entities should not take a payload so no Consumes should be defined."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var hasVerbAttribute = HasAnyAttribute(context, method, out var _, "HttpGet", "HttpDelete");
            if(!hasVerbAttribute) {
                return;
            }
            var _class = ClassForMember(method);
            var hasApiController = HasAttribute(context, _class, "ApiController", out var _);
            if(!hasApiController) {
                return;
            }
            var actionHasConsumesAttribute = HasAnyAttribute(context, method, out var _, "Consumes");
            var controllerHasConsumesAttribute = HasAnyAttribute(context, _class, out var _, "Consumes");
            if(!actionHasConsumesAttribute && !controllerHasConsumesAttribute) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.ValueText));
        }

    }
}
