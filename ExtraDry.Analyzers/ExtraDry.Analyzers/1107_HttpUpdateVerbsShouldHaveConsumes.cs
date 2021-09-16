using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpUpdateVerbsShouldHaveConsumes : DryDiagnosticNodeAnalyzer {

        public HttpUpdateVerbsShouldHaveConsumes() : base(
            SyntaxKind.MethodDeclaration,
            1107,
            DryAnalyzerCategory.ApiUsage,
            DiagnosticSeverity.Info,
            "HttpPost, HttpPut and HttpPatch methods should have Consumes attribute",
            "Method `{0}` should explicitly declare mime type for consuming in body.",
            "The Consumes attribute indicates to API consumers what the type of the payload should be.  This is enforced programmatically and is exposed through the OpenAPI interface."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var hasVerbAttribute = HasAnyAttribute(context, method, out var _, "HttpPost", "HttpPut", "HttpPatch");
            var hasConsumesAttribute = HasAnyAttribute(context, method, out var _, "Consumes");
            if(hasVerbAttribute && !hasConsumesAttribute) {
                context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.ValueText));
            }
        }

    }
}
