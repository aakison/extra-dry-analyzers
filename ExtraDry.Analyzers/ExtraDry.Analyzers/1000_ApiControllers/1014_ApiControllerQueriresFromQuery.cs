using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ApiControllerQueriesFromQuery : DryDiagnosticNodeAnalyzer {

        public ApiControllerQueriesFromQuery() : base(
            SyntaxKind.MethodDeclaration,
            1014,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "FilterQuery and PageQuery should be passed as query string.",
            "Method '{0}' should explicity declare Query parameter as [FromQuery]",
            "The Query mechanism in ExtraDry is intentionally concise and intended as a query parameter.  It is not concise enough to be part of a route.  It should also be part of a GET method which should be a URI only without a payload."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var filterParameter = FirstTypeParameter(context, method, "FilterQuery");
            if(filterParameter == null) {
                return;
            }
            var hasFromQueryAttribute = HasAttribute(context, filterParameter, "FromQuery", out var _);
            if(hasFromQueryAttribute) {
                return;
            }
            //var _class = ClassForMember(property);
            //if(_class == null) {
            //    return;
            //}
            //var hasApiControllerAttribute = HasAttribute(context, _class, "ApiController", out var _);
            //if(!hasApiControllerAttribute) {
            //    return;
            //}
            //var isInjected = HasAttribute(context, property, "InjectAttribute", out var _);
            //if(!isInjected) {
            //    return;
            //}
            //var isDbContext = PropertyInheritsFrom(context, property, "DbContext");
            //if(!isDbContext) {
            //    return;
            //}
            context.ReportDiagnostic(Diagnostic.Create(Rule, filterParameter.GetLocation(), method.Identifier.ValueText));
        }

    }
}
