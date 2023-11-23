namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MvcControllerShouldNotHaveAllowAnonymous : DryDiagnosticNodeAnalyzer {

    public MvcControllerShouldNotHaveAllowAnonymous() : base(
        SyntaxKind.ClassDeclaration,
        1407,
        DryAnalyzerCategory.Security,
        DiagnosticSeverity.Warning,
        "API Controller Classes should not default all methods to AllowAnonymous",
        "Class '{0}' should not have an AllowAnonymous attribute",
        "Security should be considered at each individual endpoint.  Providing AllowAnonymous on the class can unintentionally allow new endpoints to have the wrong security policy.  Apply either AllowAnonymous or Authorize on each endpoint."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var _class = (ClassDeclarationSyntax)context.Node;
        var hasAllowAnonymous = HasAttribute(context, _class, "AllowAnonymous", out var attribute);
        if(!hasAllowAnonymous) {
            return;
        }
        var isMvcController = InheritsFrom(context, _class, "Controller");
        if(!isMvcController) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, attribute.GetLocation(), _class.Identifier.ValueText));
    }

}
