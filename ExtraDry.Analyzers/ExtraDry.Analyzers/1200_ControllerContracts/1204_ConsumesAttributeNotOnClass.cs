namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ConsumesAttributeNotOnClass : DryDiagnosticNodeAnalyzer {

    public ConsumesAttributeNotOnClass() : base(
        SyntaxKind.ClassDeclaration,
        1204,
        DryAnalyzerCategory.OpenApiDocs,
        DiagnosticSeverity.Warning,
        "ConsumesAttribute should only apply to methods.",
        "ConsumesAttribute on class {0} should be removed and placed on the appropriate methods.",
        "The Consumes attribute indicates to API consumers what the type of the request payload should be.  Placing on a class will indicate a payload on all methods, including those that shouldn't produce anything (e.g. List, Retrieve)."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var _class = (ClassDeclarationSyntax)context.Node;
        var hasProducesAttribute = HasAnyAttribute(context, _class, out var producesAttribute, "Consumes");
        if(!hasProducesAttribute) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, producesAttribute.GetLocation(), _class.Identifier.ValueText));
    }

}
