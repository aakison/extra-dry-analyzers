using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PocoSoftDeleteRulePropertyName : DryDiagnosticNodeAnalyzer {

        public PocoSoftDeleteRulePropertyName() : base(
            SyntaxKind.ClassDeclaration,
            1305,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "SoftDelete on classes should use nameof for property names.",
            "The property name postional argument to SoftDeleteRule class '{0}' should be declared using the `nameof` operator.",
            "SoftDelete properties should be the name of a property on the enclosed class, use the nameof operator provides strong typing ot the name and can prevent bugs when properties are renamed."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var _class = (ClassDeclarationSyntax)context.Node;
            var hasSoftDelete = HasAttribute(context, _class, "SoftDeleteRuleAttribute", out var softDeleteAttribute);
            if(!hasSoftDelete) {
                return;
            }
            var propertyName = FirstArgument(softDeleteAttribute);
            if(propertyName is InvocationExpressionSyntax invoker) {
                if(invoker.Expression is IdentifierNameSyntax) {
                    return;
                }
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, propertyName.GetLocation(), _class.Identifier.ValueText));
        }

    }
}
