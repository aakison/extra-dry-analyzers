namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PocoIntIdPropertyJsonIgnore : DryDiagnosticNodeAnalyzer {

    public PocoIntIdPropertyJsonIgnore() : base(
        SyntaxKind.PropertyDeclaration,
        1301,
        DryAnalyzerCategory.Security,
        DiagnosticSeverity.Warning,
        "Int IDs should be JsonIgnore.",
        "Int property '{0}' should be decorated with [JsonIgnore].",
        "The integer Id is typically mapped to a primary key in the database and exposing this information could lead to an enumeration attack.  Create an alternate 'public' Id, such as a Uuid property or a string WebId to expose through user interface and APIs.  This applies to all Id conventions used by Entity Framework and other ORMs."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var property = (PropertyDeclarationSyntax)context.Node;
        var isIgnored = HasAttribute(context, property, "JsonIgnore", out var _);
        if(isIgnored) {
            return;
        }
        var isPublic = HasVisibility(property, Visibility.Public);
        if(!isPublic) {
            return;
        }
        var typeName = property.Type.ToString();
        var isInt = typeName == "int" || typeName == "System.Int32"; // Worry about retrieving context, or good enough?
        if(!isInt) {
            return;
        }
        var _class = ClassForMember(property);
        if(_class == null) {
            return;
        }
        var idByConvention = property.Identifier.ValueText.Equals("Id", StringComparison.OrdinalIgnoreCase) || property.Identifier.ValueText.Equals($"{_class.Identifier}Id", StringComparison.OrdinalIgnoreCase);
        var idByKeyAttribute = HasAttribute(context, property, "Key", out var _);
        if(!idByConvention && !idByKeyAttribute) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, property.Identifier.GetLocation(), property.Identifier.ValueText));
    }

}
