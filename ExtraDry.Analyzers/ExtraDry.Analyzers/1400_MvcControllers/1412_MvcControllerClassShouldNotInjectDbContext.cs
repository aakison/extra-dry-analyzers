namespace ExtraDry.Analyzers; 

// Basically same as DRY1012
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MvcControllerClassShouldNotInjectDdContext : DryDiagnosticNodeAnalyzer {

    public MvcControllerClassShouldNotInjectDdContext() : base(
        SyntaxKind.ConstructorDeclaration,
        1014,
        DryAnalyzerCategory.Usage,
        DiagnosticSeverity.Warning,
        "MVC Controller Classes should not directly use DbContext.",
        "Class '{0}' should not take a dependency on DbContext through it's constructor",
        "To properly separate layers of the application and enable re-use of logic, the DbContext class (and derived classes) should not be used by controllers.  Instead, create a re-usable service that wraps some core set of concepts form the DbContext, such as CRUD operations around an Entity.  Then, inject the service into the controller."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var ctor = (ConstructorDeclarationSyntax)context.Node;
        var _class = ClassForMember(ctor);
        if(_class == null) {
            return;
        }
        var isMvc = InheritsFrom(context, _class, "Controller");
        if(!isMvc) {
            return;
        }
        var parameter = FirstTypeParameter(context, ctor, "DbContext");
        if(parameter == null) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, ctor.Identifier.GetLocation(), _class.Identifier.ValueText));
    }

}
