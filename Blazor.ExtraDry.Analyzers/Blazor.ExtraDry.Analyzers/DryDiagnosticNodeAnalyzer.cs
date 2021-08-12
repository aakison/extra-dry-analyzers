using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Blazor.ExtraDry.Analyzers
{
    public abstract class DryDiagnosticNodeAnalyzer : DiagnosticAnalyzer
    {
        protected DryDiagnosticNodeAnalyzer(
            SyntaxKind kind,
            int code,
            DryAnalyzerCategory category,
            DiagnosticSeverity severity,
            string title,
            string message,
            string description)
        {
            Kind = kind;
            if(!rules.ContainsKey(code))
            {
                rules.Add(code, new DiagnosticDescriptor($"DRY{code}", title, message, category.ToString(), severity,
                    isEnabledByDefault: true, description: description));
            }
            Rule = rules[code];
        }

        private static readonly Dictionary<int, DiagnosticDescriptor> rules = new Dictionary<int, DiagnosticDescriptor>();

        protected DiagnosticDescriptor Rule { get; set; }

        protected SyntaxKind Kind { get; set; }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics {
            get { return ImmutableArray.Create(Rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, Kind);
        }

        public abstract void AnalyzeNode(SyntaxNodeAnalysisContext context);

        protected bool HasAttribute(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax _class, string attributeName, out AttributeSyntax attribute)
        {
            if(_class.Identifier.ValueText == "SampleController") {
                int x = 0;
            }
            var fullName = $"{attributeName}Attribute";
            var attributes = _class.AttributeLists.SelectMany(e => e.Attributes);
            
            foreach(var attr in attributes) {
                var attrSymbol = context.SemanticModel.GetTypeInfo(attr).Type;
                var inherits = Inherits(attrSymbol, fullName);
                if(inherits) {
                    attribute = attr;
                    return true;
                }
            }
            attribute = null;
            return false;
            //    .FirstOrDefault(e => e.GetText().ToString() == attributeName || e.GetText().ToString() == fullName);
            //return attribute != null;
        }

        protected bool InheritsFrom(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax _class, string baseName)
        {
            var symbol = context.SemanticModel.GetDeclaredSymbol(_class);
            return Inherits(symbol, baseName);
        }

        private static bool Inherits(ITypeSymbol symbol, string baseName)
        {
            if(symbol.Name == baseName) {
                return true;
            }
            while(symbol.BaseType != null) {
                if(symbol.Name == baseName) {
                    return true;
                }
                symbol = symbol.BaseType;
            }
            return false;
        }

        public const string filename = @"C:\Users\Adrian\log.txt";

    }
}
