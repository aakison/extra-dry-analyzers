using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PocoStringLengthPreferred : DryDiagnosticNodeAnalyzer {

        public PocoStringLengthPreferred() : base(
            SyntaxKind.PropertyDeclaration,
            1310,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "Prefer the use of StringLength instead of MaxLength .",
            "The property '{0}' has a MaxLengthAttribute instead of StringLengthAttribute.",
            "The MaxLength and StringLength attributes are similar, with StringLength designed for Validation and MaxLength designed for Entity Framework column sizes.  While validation does not recognize MaxLength, EF does recognize StringLength.  So using StringLength will satisfy both systems while MaxLength will not."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var property = (PropertyDeclarationSyntax)context.Node;
            var hasMaxLengthAttribute = HasAttribute(context, property, "MaxLengthAttribute", out var _);
            if(!hasMaxLengthAttribute) {
                return;
            }
            var hasStringLengthAttribute = HasAttribute(context, property, "StringLengthAttribute", out var _);
            if(hasStringLengthAttribute) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, property.Identifier.GetLocation(), property.Identifier.ValueText));
        }

    }

}
