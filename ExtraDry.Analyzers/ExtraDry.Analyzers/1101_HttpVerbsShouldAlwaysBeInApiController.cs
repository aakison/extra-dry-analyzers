using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpVerbsShouldAlwaysBeInApiController : DryDiagnosticNodeAnalyzer {

        public HttpVerbsShouldAlwaysBeInApiController() : base(
            SyntaxKind.MethodDeclaration,
            1101,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "Methods with Http{Patch/Put/Delete} should be in API Controller classes",
            "Class '{0}' should have an ApiController attribute",
            "Methods that are attributed with HttpPatch, HttpPut, or HttpDelete are used for API specific functions and should be in API Controller classes."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var hasVerbAttribute = HasAnyAttribute(context, method, out var _, "HttpPut", "HttpDelete", "HttpPatch");
            if(!hasVerbAttribute) {
                return;
            }
            var _class = ClassForMethod(method);
            if(_class == null) {
                return; // e.g. an interface
            }
            var hasApiAttribute = HasAttribute(context, _class, "ApiController", out var _);
            if(hasApiAttribute) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.ValueText));
        }

    }
}
