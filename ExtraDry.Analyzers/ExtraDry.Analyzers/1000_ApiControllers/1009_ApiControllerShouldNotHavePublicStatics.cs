namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ApiControllerShouldNotHavePublicStatic : DryDiagnosticNodeAnalyzer {

    public ApiControllerShouldNotHavePublicStatic() : base(
        SyntaxKind.MethodDeclaration,
        1009,
        DryAnalyzerCategory.Security,
        DiagnosticSeverity.Warning,
        "API Controller Classes should not have public static methods.",
        "Method '{0}' should be protected/private or moved to a Service",
        "API Controllers should not expose helper methods to other controllers and/or services.  If this is a local helper method, then make it private.  If this is common functionality, refactor it to a Service or Helper class.."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        var isPublic = HasVisibility(method, Visibility.Public);
        if(!isPublic) {
            return;
        }
        var isStatic = IsStatic(method);
        if(!isStatic) {
            return;
        }
        var _class = ClassForMember(method);
        if(_class == null) {
            return;
        }
        var isApiController = HasAttribute(context, _class, "ApiController", out var _);
        if(!isApiController) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.ValueText));
    }

}
