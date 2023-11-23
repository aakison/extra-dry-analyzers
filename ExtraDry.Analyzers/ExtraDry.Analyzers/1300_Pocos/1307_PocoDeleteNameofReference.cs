namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PocoDeleteNameofReference : DryDiagnosticNodeAnalyzer {

    public PocoDeleteNameofReference() : base(
        SyntaxKind.ClassDeclaration,
        1307,
        DryAnalyzerCategory.Usage,
        DiagnosticSeverity.Warning,
        "DeleteRule on classes nameof should reference property on class.",
        "The property name argument to DeleteRule class '{0}' reference a property on the class or the class's base class.",
        "DeleteRule properties should be the name of a property on the enclosed class, use the nameof operator provides strong typing ot the name and can prevent bugs when properties are renamed."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var _class = (ClassDeclarationSyntax)context.Node;
        var hasDeleteRule = HasAttribute(context, _class, "DeleteRuleAttribute", out var deleteRuleAttribute);
        if(!hasDeleteRule) {
            return;
        }
        if(NthArgument(deleteRuleAttribute, 1) is not InvocationExpressionSyntax propertyInvoker) {
            // invalid, but handled by DRY1305
            return;
        }
        if(propertyInvoker.Expression is not IdentifierNameSyntax _) {
            // invalid, but handled by DRY1305
            return;
        }
        var propertyPath = propertyInvoker.ArgumentList.Arguments.FirstOrDefault()?.ToString() ?? "";
        var propertyName = propertyPath.Split('.').Last();
        var propertyExists = HasProperty(context, _class, propertyName);
        if(propertyExists) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, propertyInvoker.GetLocation(), _class.Identifier.ValueText));
    }

}
