using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ApiControllerClassShouldNotHaveRoute : DryDiagnosticNodeAnalyzer {

        public ApiControllerClassShouldNotHaveRoute() : base(
            SyntaxKind.ClassDeclaration,
            1005,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "ApiController Classes shouldn't have Route attribute",
            "Class '{0}' shouldn't have Route attribute",
            "Use the route on the HttpVerbAttribute method and not the class.  This aids analysis of which route attaches to which endpoint."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var _class = (ClassDeclarationSyntax)context.Node;
            var hasRouteAttribute = HasAttribute(context, _class, "Route", out var routeNode);
            if(hasRouteAttribute) {
                context.ReportDiagnostic(Diagnostic.Create(Rule, routeNode.GetLocation(), _class.Identifier.ValueText));
            }
        }

    }
}
