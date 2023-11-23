namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AuthorizeAttributePolicyPositional : DryDiagnosticNodeAnalyzer {

    public AuthorizeAttributePolicyPositional() : base(
        SyntaxKind.MethodDeclaration,
        1206,
        DryAnalyzerCategory.Usage,
        DiagnosticSeverity.Info,
        "AuthorizeAttribute policy should be positional.",
        "AuthorizeAttribute on method {0} should use positional and not named policy.",
        "The Authorize attribute should only ever use policy therefore using named arguments to differentiate policy from roles is redundant."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var _method = (MethodDeclarationSyntax)context.Node;
        var hasAuthorizeAttribute = HasAnyAttribute(context, _method, out var authorizeAttribute, "Authorize");
        if(!hasAuthorizeAttribute) {
            return;
        }
        var policyNamed = NamedArgument(authorizeAttribute, "Policy");
        if(policyNamed == null) {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, authorizeAttribute.GetLocation(), _method.Identifier.ValueText));
    }

}
