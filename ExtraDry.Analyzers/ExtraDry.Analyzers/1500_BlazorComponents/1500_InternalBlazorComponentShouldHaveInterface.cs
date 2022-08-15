using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InternalBlazorComponentShouldHaveInterface : DryDiagnosticNodeAnalyzer {

        public InternalBlazorComponentShouldHaveInterface() : base(
            SyntaxKind.ClassDeclaration,
            1500,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "Extra DRY Blazor components should have an interface.",
            "Class '{0}' should implement IExtraDryComponent",
            "Extra DRY Blazor components in the Extra Dry framework should have consistent functionality (i.e. developer-polymorphism).  This consistency reduces the cognitive load for consumers of components."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var _class = (ClassDeclarationSyntax)context.Node;
            
            var isComponentBase = _class.Identifier.ValueText == "ComponentBase";
            if(isComponentBase) {
                return;
            }
            var isAbstract = IsAbstract(_class);
            if(isAbstract) {
                return;
            }
            var inExtraDryNamespace = InNamespace(_class, "ExtraDry");
            if(!inExtraDryNamespace) {
                return;
            }
            var inheritsComponentBase = InheritsFrom(context, _class, "ComponentBase");
            if(!inheritsComponentBase) {
                return;
            }
            var implementsExtraDryComponent = Implements(context, _class, "IExtraDryComponent");
            if(implementsExtraDryComponent) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, _class.Identifier.GetLocation(), _class.Identifier.ValueText));
        }

    }
}
