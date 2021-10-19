using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AuthorizeAttributeUsesPolicy : DryDiagnosticNodeAnalyzer {

        public AuthorizeAttributeUsesPolicy() : base(
            SyntaxKind.MethodDeclaration,
            1205,
            DryAnalyzerCategory.Security,
            DiagnosticSeverity.Warning,
            "AuthorizeAttribute should use policies.",
            "AuthorizeAttribute on method {0} should use a policy.",
            "The Authorize attribute controls access to the method/action and should have a descriptive and extensible security mechanism.  Don't rely solely on authentication for access, consider at least a basic 'stakeholder' type policy for authenticated users.  Also, do not rely on Roles which are less functional and not extensible."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var _method = (MethodDeclarationSyntax)context.Node;
            var hasAuthorizeAttribute = HasAnyAttribute(context, _method, out var authorizeAttribute, "Authorize");
            if(!hasAuthorizeAttribute) {
                return;
            }
            //var roleNamed = NamedArgument(authorizeAttribute, "Roles");
            var policyPositional = FirstArgument(authorizeAttribute);
            var policyNamed = NamedArgument(authorizeAttribute, "Policy");
            if(policyNamed != null || policyPositional != null) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, authorizeAttribute.GetLocation(), _method.Identifier.ValueText));
        }

    }
}
