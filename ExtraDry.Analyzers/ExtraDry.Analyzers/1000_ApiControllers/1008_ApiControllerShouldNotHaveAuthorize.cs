namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ApiControllerShouldNotHaveAuthorize : DryDiagnosticNodeAnalyzer {

    public ApiControllerShouldNotHaveAuthorize() : base(
        SyntaxKind.ClassDeclaration,
        1008,
        DryAnalyzerCategory.Security,
        DiagnosticSeverity.Warning,
        "API Controller Classes should not default all methods with Authorize",
        "Class '{0}' should not have an Authorize attribute",
        "Security should be considered at each individual endpoint.  Providing Authorize on the class can unintentionally allow new endpoints to have the wrong security policy.  Apply either AllowAnonymous or Authorize on each endpoint."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var _class = (ClassDeclarationSyntax)context.Node;
        var hasAuthorize = HasAttribute(context, _class, "Authorize", out var attribute);
        if(!hasAuthorize) {
            return;
        }
        var hasApiController = HasAttribute(context, _class, "ApiController", out var _);
        if(!hasApiController) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, attribute.GetLocation(), _class.Identifier.ValueText));
    }

}
