using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ApiControllerShouldNotUseSwaggerOperation : DryDiagnosticNodeAnalyzer {

        public ApiControllerShouldNotUseSwaggerOperation() : base(
            SyntaxKind.MethodDeclaration,
            1016,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "API Controller methods should prefer comments and conventions over the `SwaggerOperation` attribute.",
            "Method '{0}' should not have [SwaggerOperation] attribute",
            "The use of `SwaggerOperation` for comments doesn't support markdown and creates a disconnect between public documentation and internal Intellisense."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var hasSwaggerAttribute = HasAttribute(context, method, "SwaggerOperationAttribute", out var swagger);
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
