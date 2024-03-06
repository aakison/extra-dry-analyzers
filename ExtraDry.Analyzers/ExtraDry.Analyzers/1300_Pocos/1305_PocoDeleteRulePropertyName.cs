namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PocoDeleteRulePropertyName : DryDiagnosticNodeAnalyzer {

    public PocoDeleteRulePropertyName() : base(
        SyntaxKind.ClassDeclaration,
        1305,
        DryAnalyzerCategory.Usage,
        DiagnosticSeverity.Warning,
        "DeleteRule on classes should use nameof for property names.",
        "The property name positional argument to DeleteRule class '{0}' should be declared using the `nameof` operator.",
        "DeleteRule properties should be the name of a property on the enclosed class, use the nameof operator provides strong typing to the name and can prevent bugs when properties are renamed."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var _class = context.Node as ClassDeclarationSyntax;
        if(_class == null) {
            return;
        }
        var hasDelete = HasAttribute(context, _class, "DeleteRuleAttribute", out var softDeleteAttribute);
        if(!hasDelete) {
            return;
        }
        var propertyName = NthArgument(softDeleteAttribute, 1);
        if(propertyName == null) {
            return;
        }
        if(propertyName is InvocationExpressionSyntax invoker) {
            if(invoker.Expression is IdentifierNameSyntax) {
                return;
            }
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, propertyName.GetLocation(), _class.Identifier.ValueText));
    }

}
