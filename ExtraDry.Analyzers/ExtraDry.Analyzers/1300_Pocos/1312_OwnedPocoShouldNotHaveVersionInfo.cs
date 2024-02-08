using System.Reflection;

namespace ExtraDry.Analyzers; 

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class OwnedPocoShouldNotHaveVersionInfo : DryDiagnosticNodeAnalyzer {

    public OwnedPocoShouldNotHaveVersionInfo() : base(
        SyntaxKind.PropertyDeclaration,
        1312,
        DryAnalyzerCategory.Usage,
        DiagnosticSeverity.Warning,
        "VersionInfo property should not be in an Owned object.",
        "The property '{0}', of type VersionInfo, should not be in a class with an OwnedAttribute.",
        "The VersionInfo object is designed to provide auditing at a table level.  This saves the timestamp and the editing user or system agent.  Owned objects are not tables but are part of a parent table.  Move the VersionInfo to the parent POCO so that it is directly on the table."
        )
    { }

    public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var property = (PropertyDeclarationSyntax)context.Node;
        var name = property.Type.TryGetInferredMemberName();
        if(name != "VersionInfo") {
            return;
        }
        if(property.Parent is not ClassDeclarationSyntax) {
            return;
        }
        var _class = ClassForMember(property);
        var ownedAttribute = HasAttribute(context, _class, "Owned", out var _);
        if(!ownedAttribute) {
            return; 
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, property.Identifier.GetLocation(), property.Identifier.ValueText));
    }

}
