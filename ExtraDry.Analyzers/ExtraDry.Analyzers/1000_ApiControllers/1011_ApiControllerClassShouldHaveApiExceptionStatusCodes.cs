using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ApiControllerClassShouldHaveApiExceptionStatusCodes : DryDiagnosticNodeAnalyzer {

        public ApiControllerClassShouldHaveApiExceptionStatusCodes() : base(
            SyntaxKind.ClassDeclaration,
            1011,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "ApiController Classes should have ApiExceptionStatusCodes attribute",
            "Class '{0}' should have ApiExceptionStatusCodes attribute",
            "Use the ApiExceptionStatusCodes attribute to map common exceptions into status codes with associated ProblemDetails body."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var _class = (ClassDeclarationSyntax)context.Node;
            var hasApiControllerAttribute = HasAttribute(context, _class, "ApiController", out var _);
            if(!hasApiControllerAttribute) {
                return;
            }
            var hasSkipAttribute = HasAttribute(context, _class, "ApiExceptionStatusCodes", out var _);
            if(hasSkipAttribute) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, _class.Identifier.GetLocation(), _class.Identifier.ValueText));
        }

    }
}
