using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Linq;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BlazorComponentsCommonPropertiesArePublic : DryDiagnosticNodeAnalyzer {

        public BlazorComponentsCommonPropertiesArePublic() : base(
            SyntaxKind.PropertyDeclaration,
            1502,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "Common properties on Blazor components should be public.",
            "Property '{0}' should be public.",
            "Common properties on Blazor components should be public."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var property = (PropertyDeclarationSyntax)context.Node;
            var _class = ClassForMember(property);
            if(_class == null) {
                return;
            }
            var isCommon = commonProperties.Any(e => property.Identifier.ValueText == e);
            if(!isCommon) {
                return;
            }
            var isPublic = HasVisibility(property, Visibility.Public);
            if(isPublic) {
                return;
            }
            var isComponent = InheritsFrom(context, _class, "ComponentBase");
            if(!isComponent) {
                return;
            }
            var isAbstract = IsAbstract(_class);
            if(isAbstract) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, property.Identifier.GetLocation(), property.Identifier.ValueText));
        }

        public string[] commonProperties = { "CssClass", "Placeholder", "UnmatchedAttributes" };
    }
}
