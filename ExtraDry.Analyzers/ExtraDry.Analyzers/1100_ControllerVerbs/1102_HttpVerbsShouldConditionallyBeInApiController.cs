using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpVerbsShouldConditionallyBeInApiController : DryDiagnosticNodeAnalyzer {

        public HttpVerbsShouldConditionallyBeInApiController() : base(
            SyntaxKind.MethodDeclaration,
            1102,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "Methods with Http{Get/Post} should be in API Controller classes (or MVC controllers)",
            "Class '{0}' should have an ApiController attribute (or be an MVC controller)",
            "Methods that are attributed with HttpGet, or HttpPost are used for API specific functions and should be in API Controller classes (or in an MVC controller class)."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var hasVerbAttribute = HasAnyAttribute(context, method, out var _, "HttpGet", "HttpPost");
            if(!hasVerbAttribute) {
                return;
            }
            var _class = ClassForMember(method);
            if(_class == null) {
                return; // e.g. an interface
            }
            var hasApiAttribute = HasAttribute(context, _class, "ApiController", out var _);
            var isMvcController = InheritsFrom(context, _class, "ControllerBase");
            if(hasApiAttribute || isMvcController) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.ValueText));
        }

    }
}
