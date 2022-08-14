using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BlazorComponentShouldHaveCssClass : DryDiagnosticNodeAnalyzer {

        public BlazorComponentShouldHaveCssClass() : base(
            SyntaxKind.ClassDeclaration,
            1501,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "Blazor components should have a property named 'CssClass'.",
            "Class '{0}' should have a CssClass property",
            "Blazor components in the Extra Dry framework should have consistent functionality (i.e. developer-polymorphism).  This consistency reduces the cognitive load for consumers of components."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var _class = (ClassDeclarationSyntax)context.Node;
            var isComponentBase = _class.Identifier.ValueText == "ComponentBase";
            if(isComponentBase) {
                return;
            }
            var inheritsComponentBase = InheritsFrom(context, _class, "ComponentBase");
            if(!inheritsComponentBase) {
                return;
            }
            var hasCssClass = _class.Members.Any(e => (e as PropertyDeclarationSyntax)?.Identifier.ValueText == "CssClass");
            if(hasCssClass) {
                return;
            }
            var isAbstract = IsAbstract(_class);
            if(isAbstract) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, _class.Identifier.GetLocation(), _class.Identifier.ValueText));
        }

    }
}
