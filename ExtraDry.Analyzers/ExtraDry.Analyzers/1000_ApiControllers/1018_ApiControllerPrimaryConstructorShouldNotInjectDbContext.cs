namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ApiControllerPrimaryConstructorShouldNotInjectDdContext : DryDiagnosticNodeAnalyzer {

    public ApiControllerPrimaryConstructorShouldNotInjectDdContext() : base(
        SyntaxKind.ClassDeclaration,
        1018,
        DryAnalyzerCategory.Usage,
        DiagnosticSeverity.Warning,
        "API Controller Classes should not directly use DbContext.",
        "Class '{0}' should not take a dependency on DbContext through it's primary constructor",
        "To properly separate layers of the application and enable re-use of logic, the DbContext class (and derived classes) should not be used by controllers.  Instead, create a re-usable service that wraps some core set of concepts form the DbContext, such as CRUD operations around an Entity.  Then, inject the service into the controller."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var _class = (ClassDeclarationSyntax)context.Node;
        if(_class == null) {
            return;
        }
        //if(_class.Identifier.ValueText != "SampleController") {
        //    return;
        //}

        var hasApiControllerAttribute = HasAttribute(context, _class, "ApiController", out var _);
        if(!hasApiControllerAttribute) {
            return;
        }
        var parameter = FirstPrimaryCtorTypeParameter(context, _class, "DbContext");
        if(parameter == null) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, _class.Identifier.GetLocation(), _class.Identifier.ValueText));
    }

}
