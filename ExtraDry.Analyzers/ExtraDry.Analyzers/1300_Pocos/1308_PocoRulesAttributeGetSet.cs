using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PocoRulesAttributeGetSet : DryDiagnosticNodeAnalyzer {

        public PocoRulesAttributeGetSet() : base(
            SyntaxKind.PropertyDeclaration,
            1308,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "The RulesAttribute is only applicable to mutable properties.",
            "The property '{0}' has a RulesAttribute but does not provide both a getter and a setter.",
            "The RulesAttribute is designed to manage the consistent creation and updating of properties, in order to do so it must compare the existing value to a suggested value, and if different update the value.  This requires the getter and setter both exist, and that the affect is well defined."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var property = (PropertyDeclarationSyntax)context.Node;
            var hasRuleAttribute = HasAttribute(context, property, "RulesAttribute", out var _);
            if(!hasRuleAttribute) {
                return;
            }
            // Check for 2 accessors (get & set) that aren't private.
            if(property.AccessorList?.Accessors.Count(e => !HasVisibility(e, Visibility.Private)) == 2) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, property.Identifier.GetLocation(), property.Identifier.ValueText));
        }

    }

}
