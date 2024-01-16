namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PocoRequiredJsonIgnoreMismatch : DryDiagnosticNodeAnalyzer {

    public PocoRequiredJsonIgnoreMismatch() : base(
        SyntaxKind.PropertyDeclaration,
        1311,
        DryAnalyzerCategory.Usage,
        DiagnosticSeverity.Warning,
        "Required properties cannot be used as DTO when JsonIgnore set.",
        "The property '{0}' has both RequiredAttribute and JsonIgnoreAttribute.",
        "The Required and JsonIngore attributes are functionally in conflict.  When an object is sent from a client to the server as a DTO, the API will not expose this property.  However, the deserialized object will expect that this property is required.  As the DTO does not contain the value, data validation will fail."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var property = (PropertyDeclarationSyntax)context.Node;
        var hasRequiredAttribute = HasAttribute(context, property, "RequiredAttribute", out var _);
        var hasJsonIgnoreAttribute = HasAttribute(context, property, "JsonIgnoreAttribute", out var _);
        if(!hasRequiredAttribute || !hasJsonIgnoreAttribute) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, property.Identifier.GetLocation(), property.Identifier.ValueText));
    }

}
