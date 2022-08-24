using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PocoSoftDeleteRuleUniqueUndeleteProperty : DryDiagnosticNodeAnalyzer {

        public PocoSoftDeleteRuleUniqueUndeleteProperty() : base(
            SyntaxKind.ClassDeclaration,
            1306,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "SoftDelete on classes should have a unique `Undelete` value.",
            "The Undelete property on positional argument to SoftDeleteRule class '{0}' should be distinct from the Delete property.",
            "SoftDelete values need to be distinct as the delete value will define that the entity has been soft deleted.  When enabling undelete, the undelete value must be different from the delete value to affect a change."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var _class = (ClassDeclarationSyntax)context.Node;
            var hasSoftDelete = HasAttribute(context, _class, "SoftDeleteRuleAttribute", out var softDeleteAttribute);
            if(!hasSoftDelete) {
                return;
            }
            var undeleteAttribute = NthArgument(softDeleteAttribute, 2);
            if(undeleteAttribute == null) {
                return;
            }
            var deleteAttribute = NthArgument(softDeleteAttribute, 1);
            if(!deleteAttribute.IsEquivalentTo(undeleteAttribute)) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, undeleteAttribute.GetLocation(), _class.Identifier.ValueText));
        }

    }
}
