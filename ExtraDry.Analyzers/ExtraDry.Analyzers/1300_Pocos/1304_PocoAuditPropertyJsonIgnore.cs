namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PocoAuditPropertyJsonIgnore : DryDiagnosticNodeAnalyzer {

    public PocoAuditPropertyJsonIgnore() : base(
        SyntaxKind.PropertyDeclaration,
        1304,
        DryAnalyzerCategory.Security,
        DiagnosticSeverity.Warning,
        "Properties that might leak PID should be JsonIgnore.",
        "The property '{0}' should be decorated with [JsonIgnore].",
        "Properties that record information about versions and auditing might inadvertantly contain personally identifable data (PID).  This information is typically considered protected and care should be taken, especially in areas where it isn't normally considered, such as properties used for 'audit'ing and 'version'ing."
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
        var _class = ClassForMember(property);
        if(_class == null) { 
            // i.e. it's a propery on an interface
            return;
        }
        var invalidStems = new string[] { "audit", "version" };
        var propertyName = property.Identifier.ValueText.ToLowerInvariant();
        var hasInvalidStem = invalidStems.Any(e => propertyName.Contains(e));
        if(!hasInvalidStem) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, property.Identifier.GetLocation(), property.Identifier.ValueText));
    }

}
