using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class HttpCreateVerbsShouldReturnResourceReference : DryDiagnosticNodeAnalyzer {

    public HttpCreateVerbsShouldReturnResourceReference() : base(
        SyntaxKind.MethodDeclaration,
        1114,
        DryAnalyzerCategory.ApiUsage,
        DiagnosticSeverity.Info,
        "HttpPost actions should Produces ResourceReference output.",
        "Method `{0}` should declare that it produces a response body of type ResourceReference.",
        "The Produces attribute indicates to API consumers what the type of the response payload will be.  Create actions should have a response body that indicates or confirms the entity keys, UUID and Slug."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        var hasPostAttribute = HasAnyAttribute(context, method, out var _, "HttpPost");
        if(!hasPostAttribute) {
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
        var idPayloadReturn = AnyReturnMatches(method, out var _, "Task<ResourceReference>", "ResourceReference");
        if(idPayloadReturn) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.ValueText));
    }

}
