using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ApiControllersShouldNotInheritComponentBase : DryDiagnosticNodeAnalyzer {

        public ApiControllersShouldNotInheritComponentBase() : base(
            SyntaxKind.ClassDeclaration,
            1002,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Info,
            "ApiController shouldn't inherit from ControllerBase",
            "ApiController '{0}' shouldn't inherit from ControllerBase",
            "ApiController provides functionality for APIs without the heavy weight ControllerBase.  If features are needed from the ControllerBase, consider using Dependency Injection instead.  E.g. IHttpContextAccessor via DI to get access to HttpContext."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var _class = (ClassDeclarationSyntax)context.Node;
            var hasApiControllerAttribute = HasAttribute(context, _class, "ApiController", out var _);
            var inheritsControllerBase = InheritsFrom(context, _class, "ControllerBase");
            var inheritsController = InheritsFrom(context, _class, "Controller");
            if(hasApiControllerAttribute && inheritsControllerBase && !inheritsController) {
                context.ReportDiagnostic(Diagnostic.Create(Rule, _class.Identifier.GetLocation(), _class.Identifier.ValueText));
            }
        }

    }
}
