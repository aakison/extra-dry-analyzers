using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AuthorizeAttributePolicyNotLiteral : DryDiagnosticNodeAnalyzer {

        public AuthorizeAttributePolicyNotLiteral() : base(
            SyntaxKind.MethodDeclaration,
            1207,
            DryAnalyzerCategory.Security,
            DiagnosticSeverity.Warning,
            "AuthorizeAttribute policies should use shared constant and not string literals.",
            "AuthorizeAttribute on method {0} should use a constant and not a string literal.",
            "The Authorize attribute policies are keyed by shared names that are used to lookup requirements.  The use of string literals as 'magic strings' to key these items is prone to error when adding and editing code.  Instead, use a compile-time checked substitute such as a `const` value or using `nameof` with an associated requirement."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var _method = (MethodDeclarationSyntax)context.Node;
            var hasAuthorizeAttribute = HasAnyAttribute(context, _method, out var authorizeAttribute, "Authorize");
            if(!hasAuthorizeAttribute) {
                return;
            }
            var policyPositional = FirstArgument(authorizeAttribute);
            var validPositional = policyPositional == null ? true : policyPositional?.Kind() != SyntaxKind.StringLiteralExpression;
            var policyNamed = NamedArgument(authorizeAttribute, "Policy");
            var validNamed = policyNamed == null ? true : policyNamed?.Kind() != SyntaxKind.StringLiteralExpression;
            if(validPositional && validNamed) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, authorizeAttribute.GetLocation(), _method.Identifier.ValueText));
        }

    }
}
