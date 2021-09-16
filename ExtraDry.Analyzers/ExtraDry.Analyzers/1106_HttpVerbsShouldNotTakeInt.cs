using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpVerbsShouldNotTakeInt : DryDiagnosticNodeAnalyzer {

        public HttpVerbsShouldNotTakeInt() : base(
            SyntaxKind.MethodDeclaration,
            1106,
            DryAnalyzerCategory.Security,
            DiagnosticSeverity.Warning,
            "HttpVerbs methods should not take int values",
            "Method `{0}` has an int parameter, prefer Guid or String for keys.",
            "HttpVerb methods are typically used to return entities and consume a key index.  When these keys are ints, they are typically mapped to `integer identity` keys in database.  This exposes an enumeration attack that can expose more objects than attackers should be able to deduce are there."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var hasVerbAttribute = HasAnyAttribute(context, method, out var _, "HttpGet", "HttpPost", "HttpPut", "HttpPatch", "HttpDelete");
            if(!hasVerbAttribute) {
                return;
            }
            var firstParameter = FirstParameter(method);
            if(firstParameter == null) {
                return;
            }
            var typeName = firstParameter.Type.ToString();
            var isInt = intTypes.Any(e => e == typeName);
            if(isInt) {
                context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.ValueText));
            }
        }

        private static readonly List<string> intTypes = new List<string> { "int", "long", "Int32", "Int64", "short", "Int16", "unsigned", "Uint32" };

    }
}
