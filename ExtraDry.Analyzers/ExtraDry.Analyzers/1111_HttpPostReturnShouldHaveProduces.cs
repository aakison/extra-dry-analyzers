using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpPostReturnShouldHaveProduces : DryDiagnosticNodeAnalyzer {

        public HttpPostReturnShouldHaveProduces() : base(
            SyntaxKind.MethodDeclaration,
            1111,
            DryAnalyzerCategory.ApiUsage,
            DiagnosticSeverity.Info,
            "HttpPost methods that return values should have Produces attribute",
            "Method `{0}` should explicitly declare Produces attribute as it returns a result.",
            "The Produces attribute indicates to API consumers that the endpoint returns a result.  This is exposed through the OpenAPI interface."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var hasVerbAttribute = HasAnyAttribute(context, method, out var _, "HttpPost");
            if(!hasVerbAttribute) {
                return;
            }
            if(method.ReturnType is PredefinedTypeSyntax predefined) {
                if(predefined.Keyword.Kind() == SyntaxKind.VoidKeyword) {
                    return;
                }
            }
            var returnsTask = AnyReturnMatches(method, out var _, "Task");
            if(returnsTask) { 
                return;
            }
            var hasProducesAttribute = HasAnyAttribute(context, method, out var _, "Produces");
            if(hasProducesAttribute) {
                return;
            }
            var _class = ClassForMember(method);
            var hasApiController = HasAttribute(context, _class, "ApiController", out var _);
            if(!hasApiController) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.ValueText));
        }

    }
}
