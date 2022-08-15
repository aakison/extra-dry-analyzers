using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ApiControllerPropertyShouldNotInjectDdContext : DryDiagnosticNodeAnalyzer {

        public ApiControllerPropertyShouldNotInjectDdContext() : base(
            SyntaxKind.PropertyDeclaration,
            1013,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "API Controller Classes should not directly use DbContext.",
            "Class '{0}' should not take a dependency on DbContext through a property",
            "To properly separate layers of the application and enable re-use of logic, the DbContext class (and derived classes) should not be used by controllers.  Instead, create a re-usable service that wraps some core set of concepts form the DbContext, such as CRUD operations around an Entity.  Then, inject the service into the controller."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var property = (PropertyDeclarationSyntax)context.Node;
            var _class = ClassForMember(property);
            if(_class == null) {
                return;
            }
            var hasApiControllerAttribute = HasAttribute(context, _class, "ApiController", out var _);
            if(!hasApiControllerAttribute) {
                return;
            }
            var isInjected = HasAttribute(context, property, "InjectAttribute", out var _);
            if(!isInjected) {
                return;
            }
            var isDbContext = PropertyInheritsFrom(context, property, "DbContext");
            if(!isDbContext) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, property.Identifier.GetLocation(), _class.Identifier.ValueText));
        }

    }
}
