namespace ExtraDry.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ApiControllerMethodsShouldNotHaveRoute : DryDiagnosticNodeAnalyzer {

    public ApiControllerMethodsShouldNotHaveRoute() : base(
        SyntaxKind.MethodDeclaration,
        1004,
        DryAnalyzerCategory.Usage,
        DiagnosticSeverity.Warning,
        "Methods of ApiControllers shouldn't have Route attribute",
        "Method '{0}' shouldn't have Route attribute",
        "Use the route on the the HttpVerbAttribute to prevent routes without verbs and verbs without routes."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        var hasRouteAttribute = HasAttribute(context, method, "Route", out var routeNode);
        if(hasRouteAttribute) {
            context.ReportDiagnostic(Diagnostic.Create(Rule, routeNode.GetLocation(), method.Identifier.ValueText));
        }
    }

}
