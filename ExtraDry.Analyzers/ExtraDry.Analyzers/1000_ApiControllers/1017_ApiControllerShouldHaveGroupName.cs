namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ApiControllerShouldHaveGroupName : DryDiagnosticNodeAnalyzer {

    public ApiControllerShouldHaveGroupName() : base(
        SyntaxKind.ClassDeclaration,
        1017,
        DryAnalyzerCategory.Usage,
        DiagnosticSeverity.Warning,
        "API Controllers should have group name for OpenAPI.",
        "Class '{0}' is an API controller but does not indicate a GroupName parameter in the ApiExplorerSettings attribute.",
        "For ease of discovery and consumption of public facing APIs, they should be partioned into logical groups.  For internal APIs, such as for Microservices, this can be ignored project-wide."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var _class = (ClassDeclarationSyntax)context.Node;
        var hasApiController = HasAttribute(context, _class, "ApiController", out var _);
        if(!hasApiController) {
            return;
        }
        var hasApiExplorerSettings = HasAttribute(context, _class, "ApiExplorerSettings", out var apiExplorerSettings);
        var groupName = NamedArgument(apiExplorerSettings, "GroupName");
        if(hasApiExplorerSettings && groupName != null) {
            return;
        }
        var ignore = NamedArgument(apiExplorerSettings, "IgnoreApi");
        if(ignore.IsKind(SyntaxKind.TrueLiteralExpression)) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, _class.Identifier.GetLocation(), _class.Identifier.ValueText));
    }
}
