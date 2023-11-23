namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class HttpVerbsShouldNotTakeReservedWords : DryDiagnosticNodeAnalyzer {

    public HttpVerbsShouldNotTakeReservedWords() : base(
        SyntaxKind.MethodDeclaration,
        1113,
        DryAnalyzerCategory.Usage,
        DiagnosticSeverity.Warning,
        "Http GET methods should not take parameters with names that match internal properties",
        "Method `{0}` has an parameter with a name that matches the name that matches and internal property.",
        "Http GET methods are typically used to return entities and consume a query.  The queries are complex objects but are constructed from parameter in the query string.  Each item in the query string is passed to a single property.  If the parameter has the same name as one of these parameters there will be a runtime ambiguity causing parameter binding issues."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        var hasVerbAttribute = HasAnyAttribute(context, method, out var _, "HttpGet");
        if(!hasVerbAttribute) {
            return;
        }
        var parameters = Parameters(method);

        var allClear = parameters.All(e => !reservedWords.Any(r => r.Equals(e.Identifier.ValueText, StringComparison.OrdinalIgnoreCase)));
        if(allClear) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.ValueText));
    }

    private static readonly List<string> reservedWords = new() { "filter", "sort", "take", "skip", "token", "DefaultTake" };

}
