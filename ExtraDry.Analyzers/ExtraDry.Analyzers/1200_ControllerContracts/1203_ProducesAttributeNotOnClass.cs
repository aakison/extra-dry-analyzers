namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ProducesAttributeNotOnClass : DryDiagnosticNodeAnalyzer {

    public ProducesAttributeNotOnClass() : base(
        SyntaxKind.ClassDeclaration,
        1203,
        DryAnalyzerCategory.OpenApiDocs,
        DiagnosticSeverity.Warning,
        "ProducesAttribute should only apply to methods.",
        "ProducesAttribute on class {0} should be removed and placed on the appropriate methods.",
        "The Produces attribute indicates to API consumers what the type of the response payload will be.  Placing on a class will indicate a response on all methods, including those that shouldn't produce anything (e.g. PUT, DELETE)."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var _class = (ClassDeclarationSyntax)context.Node;
        var hasProducesAttribute = HasAnyAttribute(context, _class, out var producesAttribute, "Produces");
        if(!hasProducesAttribute) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, producesAttribute.GetLocation(), _class.Identifier.ValueText));
    }

}
