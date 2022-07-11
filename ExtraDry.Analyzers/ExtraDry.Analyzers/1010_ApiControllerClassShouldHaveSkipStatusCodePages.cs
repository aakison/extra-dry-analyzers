using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ApiControllerClassShouldHaveSkipStatusCodePages : DryDiagnosticNodeAnalyzer {

        public ApiControllerClassShouldHaveSkipStatusCodePages() : base(
            SyntaxKind.ClassDeclaration,
            1010,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "ApiController Classes should have SkipStatusCodePages attribute",
            "Class '{0}' should have SkipStatusCodePages attribute",
            "Use the SkipStatusCodePages attribute to prevent MVC from redirecting an API return result to an HTML description of the exception."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var _class = (ClassDeclarationSyntax)context.Node;
            var hasApiControllerAttribute = HasAttribute(context, _class, "ApiController", out var _);
            if(!hasApiControllerAttribute) {
                return;
            }
            var hasSkipOrApiException = HasAnyAttribute(context, _class, out var _, "SkipStatusCodePages", "ApiExceptionStatusCodes");
            if(hasSkipOrApiException) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, _class.Identifier.GetLocation(), _class.Identifier.ValueText));
        }

    }
}
