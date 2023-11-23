using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ApiControllerQueriesNamedQuery : DryDiagnosticNodeAnalyzer {

        public ApiControllerQueriesNamedQuery() : base(
            SyntaxKind.MethodDeclaration,
            1015,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "FilterQuery and PageQuery parameters should be named 'query'.",
            "Method '{0}' should explicity declare parameter with name 'query'.",
            "The query mechanism in ExtraDry can be used for either filter queries or page queries, and both of these objects have properties that can easily be confused with names other than 'query'.  For example, naming a filter query `filter` implies a property called `filter.filter`.  This can lead to routes not aligning.  Further, the ability to upgrade from a FilterQuery to a PageQuery is possible but only if the parameter name is not changed."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var filterParameter = FirstTypeParameter(context, method, "FilterQuery"); // SortQuery & PageQuery included as it inherits FilterQuery.
            if(filterParameter == null) {
                return;
            }
            if(filterParameter.Identifier.ValueText == "query") {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, filterParameter.GetLocation(), method.Identifier.ValueText));
        }

    }
}
