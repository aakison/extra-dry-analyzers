using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace ExtraDry.Analyzers {

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpVerbsShouldHaveCrudNames : DryDiagnosticNodeAnalyzer {

        public HttpVerbsShouldHaveCrudNames() : base(
            SyntaxKind.MethodDeclaration,
            1104,
            DryAnalyzerCategory.Usage,
            DiagnosticSeverity.Info,
            "Http Verbs should be named with their CRUD counterparts",
            "Rename `{0}` method to start with `{1}`",
            "Http verbs are aligned with CRUD operations loosely, make it explict by adding the name of the CRUD operation to the beginning of the method.  This also explicitly disambiguates methods like `List` from `Retrieve`."
            )
        { }

        public override void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var hasVerbAttribute = HasAnyAttribute(context, method, out var verbAttribute, "HttpGet", "HttpPost", "HttpPut", "HttpPatch", "HttpDelete");
            if(!hasVerbAttribute) {
                return;
            }
            var _class = ClassForMember(method);
            if(_class == null) {
                return; // e.g. for interface
            }
            var isApiController = HasAttribute(context, _class, "ApiController", out var _);
            if(!isApiController) {
                return;
            }
            var validPrefixes = prefixLookup[verbAttribute.Name.ToString()];
            var hasValidPrefix = validPrefixes.Any(e => method.Identifier.ValueText.StartsWith(e));
            if(hasValidPrefix) {
                return;
            }
            var valid = string.Join(" or ", validPrefixes);
            context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier.ValueText, valid));
        }

        private readonly Dictionary<string, List<string>> prefixLookup = new() {
            { "HttpGet", new List<string> { "List", "Retrieve", "Tree" } },
            { "HttpPost", new List<string> { "Create", "ListHierarchy", "Tree", "ListTree" } },
            { "HttpPut", new List<string> { "Update", "Upsert" } },
            { "HttpPatch", new List<string> { "Patch", "Insert" } },
            { "HttpDelete", new List<string> { "Delete" } },
        };

    }
}
