namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MvcControllerPostMethodsShouldValidateAntiForgery : DryDiagnosticNodeAnalyzer {

    public MvcControllerPostMethodsShouldValidateAntiForgery() : base(
        SyntaxKind.MethodDeclaration,
        1402,
        DryAnalyzerCategory.Usage,
        DiagnosticSeverity.Warning,
        "Post methods on MVC Controllers should validate anti forgery tokens",
        "Method '{0}' should have [ValidateAntiForgeryToken] attribute",
        "Use the [ValidateAntiForgeryToken] attribute (in conjunction with TODO:) to ensure."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        var hasRouteAttribute = HasAttribute(context, method, "ValidateAntiForgeryTokenAttribute", out var _);
        if(hasRouteAttribute) {
            return;
        }
        var hasPostAttribute = HasAttribute(context, method, "HttpPostAttribute", out var _);
        if(!hasPostAttribute) {
            return;
        }
        var _class = ClassForMember(method);
        if(_class == null) {
            return;
        }
        var isController = InheritsFrom(context, _class, "Controller");
        if(!isController) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.ValueText));
    }

}
