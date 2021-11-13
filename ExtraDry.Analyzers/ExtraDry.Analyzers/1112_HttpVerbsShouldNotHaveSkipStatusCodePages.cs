using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpVerbsShouldNotHaveSkipStatusCodePages : DryDiagnosticNodeAnalyzer {

        public HttpVerbsShouldNotHaveSkipStatusCodePages() : base(
            SyntaxKind.MethodDeclaration,
            1112,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "Methods with Http Verbs should not have SkipStatusCodePages attribute",
            "Method `{0}` should not have SkipStatusCodePages attribute",
            "The SkipStatusCodePages attribute can be on either the class or the method.  However, for API controllers, it should always be applied and the attribute should be applied at the class level.  Methods within an API controller should not mix and match this attribute."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var hasVerbAttribute = HasAnyAttribute(context, method, out var _, "HttpGet", "HttpPost", "HttpPut", "HttpPatch", "HttpDelete");
            if(!hasVerbAttribute) {
                return;
            }
            var hasSkipAttribute = HasAttribute(context, method, "SkipStatusCodePages", out var skipAttribute);
            if(!hasSkipAttribute) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, skipAttribute.GetLocation(), method.Identifier.ValueText));
        }

    }
}
