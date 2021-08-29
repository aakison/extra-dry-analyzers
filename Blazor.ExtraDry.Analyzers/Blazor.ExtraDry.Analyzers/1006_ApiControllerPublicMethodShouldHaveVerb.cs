using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.IO;
using System.Linq;

namespace Blazor.ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ApiControllerPublicMethodShouldHaveVerb : DryDiagnosticNodeAnalyzer {

        public ApiControllerPublicMethodShouldHaveVerb() : base(
            SyntaxKind.MethodDeclaration,
            1006,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "Public methods of ApiController Classes should have a REST verb (e.g. HttpGet)",
            "Method '{0}' should have an HttpVerb attribute",
            "API Controllers should be exclusively used as a public API interface, public methods that are not exposed might indicate functionality that should be refactored into a Service class."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            try {
                var method = (MethodDeclarationSyntax)context.Node;
                var _class = method.FirstAncestorOrSelf<ClassDeclarationSyntax>(e => e is ClassDeclarationSyntax);
                var isPublic = HasVisibility(method, Visibility.Public);
                var hasApiAttribute = HasAttribute(context, _class, "ApiController", out var _);
                var hasVerbAttribute = HasAnyAttribute(context, method, out var _, "HttpGet", "HttpPut", "HttpPost", "HttpDelete", "HttpPatch");
                if(hasApiAttribute && isPublic && !hasVerbAttribute) {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.ValueText));
                }
            }
            catch(Exception ex) {
                File.WriteAllText(@"C:\Users\Adrian\Desktop\Heisenbug.txt", $"{ex.Message}\r\n{ex.StackTrace}");
            }
        }

    }
}
