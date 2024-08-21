namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BlazorComponentShouldHaveCommonProperties : DryDiagnosticNodeAnalyzer {

    public BlazorComponentShouldHaveCommonProperties() : base(
        SyntaxKind.ClassDeclaration,
        1501,
        DryAnalyzerCategory.Usage,
        DiagnosticSeverity.Warning,
        "Blazor components should have a common properties.",
        "Class '{0}' should have a '{1}' property",
        "Blazor components in the Extra Dry framework should have consistent functionality (i.e. developer-polymorphism).  This consistency reduces the cognitive load for consumers of components."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var _class = (ClassDeclarationSyntax)context.Node;
        var isComponentBase = _class.Identifier.ValueText == "ComponentBase";
        if(isComponentBase) {
            return;
        }
        var inheritsComponentBase = InheritsFrom(context, _class, "ComponentBase");
        if(!inheritsComponentBase) {
            return;
        }
        var implementsExtraDryComponent = Implements(context, _class, "IExtraDryComponent");
        if(!implementsExtraDryComponent) {
            return;
        }
        foreach(var name in names) {
            var hasCommonProperty = HasProperty(context, _class, name);
            //var hasCommonProperty = _class.Members.Any(e => (e as PropertyDeclarationSyntax)?.Identifier.ValueText == name);
            if(hasCommonProperty) {
                continue;
            }
            var isAbstract = IsAbstract(_class);
            if(isAbstract) {
                continue;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, _class.Identifier.GetLocation(), _class.Identifier.ValueText, name));
        }
    }

    public string[] names = { "CssClass", "UnmatchedAttributes" };
}
