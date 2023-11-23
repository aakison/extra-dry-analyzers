namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class HttpVerbsShouldHaveExplicitSecurity : DryDiagnosticNodeAnalyzer {

    public HttpVerbsShouldHaveExplicitSecurity() : base(
        SyntaxKind.MethodDeclaration,
        1105,
        DryAnalyzerCategory.Security,
        DiagnosticSeverity.Warning,
        "Http Verbs should have Authorize or AllowAnonymous attributes",
        "Method `{0}` should explicitly declare security intent.",
        "Http Verbs are exported and will be accessible to anyone without a security policy.  Either add the security policy with the `AuthorizeAttribute` or explicitly declare it as un-secured with `AllowAnonymousAttribute`."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        var hasVerbAttribute = HasAnyAttribute(context, method, out var _, "HttpGet", "HttpPost", "HttpPut", "HttpPatch", "HttpDelete");
        if(!hasVerbAttribute) {
            return;
        }
        var hasAuthorizeAttribute = HasAnyAttribute(context, method, out var _, "AllowAnonymous", "Authorize");
        if(hasAuthorizeAttribute) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.ValueText));
    }

}
