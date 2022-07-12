using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MvcControllerClassShouldHaveIgnoreApi : DryDiagnosticNodeAnalyzer {

        public MvcControllerClassShouldHaveIgnoreApi() : base(
            SyntaxKind.ClassDeclaration,
            1401,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "MvcController Classes should have ApiExplorerSettings(IgnoreApi = true) attribute",
            "Class '{0}' should have ApiExplorerSettings(IgnoreApi = true) attribute",
            "Use the ApiExplorerSettings(IgnoreApi = true) attribute to prevent MVC from mistaking MVC pages as API pages and including them in API documentation."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var _class = (ClassDeclarationSyntax)context.Node;
            var isControllerItself = _class.Identifier.ValueText == "Controller";
            if(isControllerItself) {
                return;
            }
            var isMvcComponent = InheritsFrom(context, _class, "Controller");
            if(!isMvcComponent) {
                return;
            }
            var hasApiController = HasAttribute(context, _class, "ApiControllerAttribute", out var _);
            if(hasApiController) {
                // Still wrong, but this error is likely wrong as they should remove Controller
                // rather than change explorer settings, other rules will provider better guidance.
                return;
            }
            var _ = HasAttribute(context, _class, "ApiExplorerSettingsAttribute", out var explorerSettings);
            var ignoreApiArgument = NamedArgument(explorerSettings, "IgnoreApi");
            if(ignoreApiArgument is LiteralExpressionSyntax literal) {
                if(literal.Token.ValueText == "true") {
                    return;
                }
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, _class.Identifier.GetLocation(), _class.Identifier.ValueText));
        }

    }
}
