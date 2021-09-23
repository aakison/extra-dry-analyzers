using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpGetShouldHaveProduces : DryDiagnosticNodeAnalyzer {

        public HttpGetShouldHaveProduces() : base(
            SyntaxKind.MethodDeclaration,
            1109,
            DryAnalyzerCategory.ApiUsage,
            DiagnosticSeverity.Info,
            "HttpGet methods should have Produces attribute",
            "Method `{0}` should explicitly declare mime type for producing response body results.",
            "The Produces attribute indicates to API consumers what the type of the response payload will be.  This is exposed through the OpenAPI interface."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var hasVerbAttribute = HasAnyAttribute(context, method, out var _, "HttpGet");
            if(!hasVerbAttribute) {
                return;
            }
            var hasProducesAttribute = HasAnyAttribute(context, method, out var _, "Produces");
            if(hasProducesAttribute) {
                return;
            }
            var _class = ClassForMethod(method);
            var hasApiController = HasAttribute(context, _class, "ApiController", out var _);
            if(!hasApiController) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.ValueText));
        }

    }
}
