using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Blazor.ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ApiControllersShouldNotInheritComponent : DryDiagnosticNodeAnalyzer {

        public ApiControllersShouldNotInheritComponent() : base(
            SyntaxKind.ClassDeclaration,
            1001,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "ApiController must not inherit from Controller",
            "ApiController '{0}' must not inherit from Controller",
            "Controller is designed for Razor views using MVC and is not designed for APIs. If features are needed from the ControllerBase, consider using Dependency Injection instead.  Worst case, inherit ControllerBase and ignore that warning."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var _class = (ClassDeclarationSyntax)context.Node;
            var hasApiControllerAttribute = HasAttribute(context, _class, "ApiController", out var _);
            var inheritsControllerBase = InheritsFrom(context, _class, "Controller");
            if(hasApiControllerAttribute && inheritsControllerBase) {
                context.ReportDiagnostic(Diagnostic.Create(Rule, _class.Identifier.GetLocation(), _class.Identifier.ValueText));
            }
        }

    }
}
