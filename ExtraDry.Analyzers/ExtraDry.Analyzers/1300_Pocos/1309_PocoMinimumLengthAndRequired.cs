namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PocoMinimumLengthAndRequired : DryDiagnosticNodeAnalyzer {

    public PocoMinimumLengthAndRequired() : base(
        SyntaxKind.PropertyDeclaration,
        1309,
        DryAnalyzerCategory.Usage,
        DiagnosticSeverity.Warning,
        "The MinimumLength property will miss an null string, ensure Required is also set.",
        "The property '{0}' has a StringLengthAttribute with MinimumLength but does not provide have a RequiredAttribute.",
        "The MinimumLength property ensures that the string is not too short.  However, the StringLengthAttribute does not check for a missing string.  This is by design and null/default values should be checked via a RequiredAttribute.  Make the MinimumLength guarantee stronger by including the RequiredAttribute."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var property = (PropertyDeclarationSyntax)context.Node;
        var hasStringLengthAttribute = HasAttribute(context, property, "StringLengthAttribute", out var stringLengthAttribute);
        if(!hasStringLengthAttribute) {
            return;
        }
        var hasRequiredAttribute = HasAttribute(context, property, "RequiredAttribute", out var _);
        if(hasRequiredAttribute) {
            return;
        }
        var minimumLength = NamedArgument(stringLengthAttribute, "MinimumLength");
        if(minimumLength == null) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, property.Identifier.GetLocation(), property.Identifier.ValueText));
    }

}
