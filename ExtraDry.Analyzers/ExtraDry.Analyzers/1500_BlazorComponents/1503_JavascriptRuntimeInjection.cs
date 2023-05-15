using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Linq;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class JavascriptRuntimeInjection : DryDiagnosticNodeAnalyzer {

        public JavascriptRuntimeInjection() : base(
            SyntaxKind.PropertyDeclaration,
            1503,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Warning,
            "JavaScript runtime injection should be replaced with module aware ExtraDryJavacriptModule.",
            "Property '{0}' should be replaced with ExtraDryJavacriptModule version.",
            "To avoid namespace pollution in projects that are designed to be consumed by others, don't take a global dependency on JavaScript.  The ExtraDryJavacriptModule wraps JavaScript global namespaces with a module namespace."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var property = (PropertyDeclarationSyntax)context.Node;
            var _class = ClassForMember(property);
            if(_class == null) {
                return;
            }
            var isInjected = HasAttribute(context, property, "InjectAttribute", out var _);
            if(!isInjected) {
                return;
            }
            var isJSRuntime = PropertyInheritsFrom(context, property, "IJSRuntime");
            if(!isJSRuntime) {
                return;
            }
            var isComponent = InheritsFrom(context, _class, "ComponentBase");
            if(!isComponent) {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(Rule, property.Identifier.GetLocation(), property.Identifier.ValueText));
        }

    }
}
