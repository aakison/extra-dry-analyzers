namespace ExtraDry.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ApiControllersShouldNotHaveRoute : DryDiagnosticNodeAnalyzer {

    public ApiControllersShouldNotHaveRoute() : base(
        SyntaxKind.ClassDeclaration,
        1003,
        DryAnalyzerCategory.Usage,
        DiagnosticSeverity.Info,
        "ApiController shouldn't have Route attribute",
        "ApiController '{0}' shouldn't have Route attribute",
        "Fully qualify the route on each controller action instead of composite from the class.  The route is a contract with users and is actually _less_ likely to change than the controller name.  This change makes both finding routes and reasoning about logic easier."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var _class = (ClassDeclarationSyntax)context.Node;
        var hasApiControllerAttribute = HasAttribute(context, _class, "ApiController", out var _);
        var hasRouteAttribute = HasAttribute(context, _class, "Route", out var routeNode);
        if(hasApiControllerAttribute && hasRouteAttribute) {
            context.ReportDiagnostic(Diagnostic.Create(Rule, routeNode.GetLocation(), _class.Identifier.ValueText));
        }
    }

}
