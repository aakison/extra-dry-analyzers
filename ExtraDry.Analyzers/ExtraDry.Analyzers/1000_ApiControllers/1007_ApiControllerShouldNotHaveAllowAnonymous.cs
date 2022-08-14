using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ApiControllerShouldNotHaveAllowAnonymous : DryDiagnosticNodeAnalyzer {

        public ApiControllerShouldNotHaveAllowAnonymous() : base(
            SyntaxKind.ClassDeclaration,
            1007,
            DryAnalyzerCategory.Security,
            DiagnosticSeverity.Warning,
            "API Controller Classes should not default all methods to AllowAnonymous",
            "Class '{0}' should not have an AllowAnonymous attribute",
            "Security should be considered at each individual endpoint.  Providing AllowAnonymous on the class can unintentionally allow new endpoints to have the wrong security policy.  Apply either AllowAnonymous or Authorize on each endpoint."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var _class = (ClassDeclarationSyntax)context.Node;
            var hasAllowAnonymous = HasAttribute(context, _class, "AllowAnonymous", out var attribute);
            if(!hasAllowAnonymous) {
                return;
            }
            var hasApiController = HasAttribute(context, _class, "ApiController", out var _);
            if(!hasApiController) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, attribute.GetLocation(), _class.Identifier.ValueText));
        }

    }
}
