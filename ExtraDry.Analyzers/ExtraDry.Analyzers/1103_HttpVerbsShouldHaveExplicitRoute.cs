using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpVerbsShouldHaveExplicitRoute : DryDiagnosticNodeAnalyzer {

        public HttpVerbsShouldHaveExplicitRoute() : base(
            SyntaxKind.MethodDeclaration,
            1103,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "Http Verbs should explicity declare the route",
            "Http verb `{0}` on method `{1}` should explicity declare the full HTTP route",
            "Routes are contracts with users and should change less frequently than method signatures.  Leaving the route unspecified could allow refactorings to break the contract with external users."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var hasVerbAttribute = HasAnyAttribute(context, method, out var verbAttribute, "HttpGet", "HttpPost", "HttpPut", "HttpPatch", "HttpDelete");
            var argument = FirstArgument(verbAttribute) ?? NamedArgument(verbAttribute, "Template");
            if(hasVerbAttribute && argument == null) {
                context.ReportDiagnostic(Diagnostic.Create(Rule, verbAttribute.GetLocation(), verbAttribute.Name.ToFullString(), method.Identifier.ValueText));
            }
        }

    }
}
