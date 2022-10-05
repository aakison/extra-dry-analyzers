using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Linq;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PocoSoftDeleteNameofReference : DryDiagnosticNodeAnalyzer {

        public PocoSoftDeleteNameofReference() : base(
            SyntaxKind.ClassDeclaration,
            1307,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "SoftDelete on classes nameof should reference property on class.",
            "The property name argument to SoftDeleteRule class '{0}' reference a property on the class or the class's base class.",
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
            if(!(FirstArgument(softDeleteAttribute) is InvocationExpressionSyntax propertyInvoker)) {
                // invalid, but handled by DRY1305
                return;
            }
            if(!(propertyInvoker.Expression is IdentifierNameSyntax identifier)) {
                // invalid, but handled by DRY1305
                return;
            }
            var propertyPath = propertyInvoker.ArgumentList.Arguments.FirstOrDefault()?.ToString() ?? "";
            var propertyName = propertyPath.Split('.').Last();
            var propertyExists = HasProperty(context, _class, propertyName);
            if(propertyExists) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, propertyInvoker.GetLocation(), _class.Identifier.ValueText));
        }

    }

}
