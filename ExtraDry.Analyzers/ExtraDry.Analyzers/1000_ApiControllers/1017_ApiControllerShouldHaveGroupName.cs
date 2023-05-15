using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ApiControllerShouldHaveGroupName : DryDiagnosticNodeAnalyzer {

        public ApiControllerShouldHaveGroupName() : base(
            SyntaxKind.MethodDeclaration,
            1017,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "API Controller methods should have group name for OpenAPI.",
            "Class '{0}' is an API controller but does not indicate a GroupName parameter",
            "For ease of discovery and consumption, APIs should be partioned into logical groups."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var hasSwaggerAttribute = HasAttribute(context, method, "SwaggerOperationAttribute", out var _);
            if(!hasSwaggerAttribute) {
                return;
            }
            var _class = ClassForMember(method);
            if(_class == null) {
                return;
            }
            var hasApiController = HasAttribute(context, _class, "ApiController", out var _);
            if(!hasApiController) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.ValueText));
        }
    }
}
