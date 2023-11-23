using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class HttpUpdateVerbsShouldNotHaveProduces : DryDiagnosticNodeAnalyzer {

    public HttpUpdateVerbsShouldNotHaveProduces() : base(
        SyntaxKind.MethodDeclaration,
        1110,
        DryAnalyzerCategory.ApiUsage,
        DiagnosticSeverity.Info,
        "HttpPut, and HttpPatch actions should not have Produces attribute",
        "Method `{0}` should not declare that it produces a response body.",
        "The Produces attribute indicates to API consumers what the type of the response payload will be.  Updating actions should not have a response body with the possible exception of changing Ids which should be communicated to clients (typically only ever a web-friendly Id)."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        var hasVerbAttribute = HasAnyAttribute(context, method, out var _, "HttpPut", "HttpPatch", "HttpDelete");
        if(!hasVerbAttribute) {
            return;
        }
        var _class = ClassForMember(method);
        if(_class == null) {
            return;
        }
        var hasApiController = HasAttribute(context, _class, "ApiController", out var _);
        if(!hasApiController) {
            return;
        }
        var actionHasConsumesAttribute = HasAnyAttribute(context, method, out var _, "Produces");
        var controllerHasConsumesAttribute = HasAnyAttribute(context, _class, out var _, "Produces");
        if(!actionHasConsumesAttribute && !controllerHasConsumesAttribute) {
            return;
        }
        var idPayloadReturn = AnyReturnMatches(method, out var _, "ResourceReference");
        if(idPayloadReturn) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.ValueText));
    }

}
