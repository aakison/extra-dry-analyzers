using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ExtraDry.Analyzers
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
            if(rules.ContainsKey(code)) {
                Rule = rules[code];
            } else {
                Rule = new DiagnosticDescriptor($"DRY{code}", title, message, category.ToString(), severity,
                    isEnabledByDefault: true, description: description);
                rules.TryAdd(code, Rule);
            }
        }

        /// <summary>
        /// Diagnostics are run in parallel for performance reasons, so our caching mechanism must be thread safe.
        /// </summary>
        private static readonly ConcurrentDictionary<int, DiagnosticDescriptor> rules = new ConcurrentDictionary<int, DiagnosticDescriptor>();

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
            return HasAnyAttribute(context, _class, out attribute, attributeName);
        }

        protected bool HasAnyAttribute(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax _class, out AttributeSyntax attribute, params string[] attributeNames)
        {
            if(attributeNames == null || !attributeNames.Any() || _class == null) {
                attribute = null;
                return false;
            }
            var fullNames = attributeNames.Select(e => e.EndsWith("Attribute") ? e : $"{e}Attribute");
            var attributes = _class.AttributeLists.SelectMany(e => e.Attributes) ?? Array.Empty<AttributeSyntax>();
            return AnyAttributeMatches(context, out attribute, fullNames, attributes);
        }

        protected bool HasAttribute(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method, string attributeName, out AttributeSyntax attribute)
        {
            return HasAnyAttribute(context, method, out attribute, attributeName);
        }

        protected bool HasAnyAttribute(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method, out AttributeSyntax attribute, params string[] attributeNames)
        {
            var fullNames = attributeNames.Select(e => e.EndsWith("Attribute") ? e : $"{e}Attribute");
            var attributes = method.AttributeLists.SelectMany(e => e.Attributes);
            return AnyAttributeMatches(context, out attribute, fullNames, attributes);
        }

        protected bool HasAttribute(SyntaxNodeAnalysisContext context, EnumDeclarationSyntax _enum, string attributeName, out AttributeSyntax attribute)
        {
            return HasAnyAttribute(context, _enum, out attribute, attributeName);
        }

        protected bool HasAnyAttribute(SyntaxNodeAnalysisContext context, EnumDeclarationSyntax _enum, out AttributeSyntax attribute, params string[] attributeNames)
        {
            if(attributeNames == null || !attributeNames.Any() || _enum == null) {
                attribute = null;
                return false;
            }
            var fullNames = attributeNames.Select(e => e.EndsWith("Attribute") ? e : $"{e}Attribute");
            var attributes = _enum.AttributeLists.SelectMany(e => e.Attributes) ?? Array.Empty<AttributeSyntax>();
            return AnyAttributeMatches(context, out attribute, fullNames, attributes);
        }


        protected bool HasAttribute(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax property, string attributeName, out AttributeSyntax attribute)
        {
            return HasAnyAttribute(context, property, out attribute, attributeName);
        }

        protected bool HasAnyAttribute(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax property, out AttributeSyntax attribute, params string[] attributeNames)
        {
            var fullNames = attributeNames.Select(e => e.EndsWith("Attribute") ? e : $"{e}Attribute");
            var attributes = property.AttributeLists.SelectMany(e => e.Attributes);
            return AnyAttributeMatches(context, out attribute, fullNames, attributes);
        }

        private static bool AnyAttributeMatches(SyntaxNodeAnalysisContext context, out AttributeSyntax attribute, IEnumerable<string> fullNames, IEnumerable<AttributeSyntax> attributes)
        {
            foreach(var attr in attributes) {
                var attrSymbol = context.SemanticModel?.GetTypeInfo(attr).Type;
                var inherits = fullNames?.Any(e => Inherits(attrSymbol, e)) ?? false;
                if(inherits) {
                    attribute = attr;
                    return true;
                }
            }
            attribute = null;
            return false;
        }

        protected ExpressionSyntax FirstArgument(AttributeSyntax attribute)
        {
            if(attribute == null) {
                return null;
            }
            var first = attribute?.ArgumentList?.Arguments.FirstOrDefault();
            if(first == null || first.NameEquals != null) {
                return null;
            }
            else {
                return first.Expression;
            }
        }

        protected ParameterSyntax FirstParameter(MethodDeclarationSyntax method)
        {
            if(method == null) {
                return null;
            }
            var first = method.ParameterList?.Parameters.FirstOrDefault();
            if(first == null) {
                return null;
            }
            else {
                return first;
            }
        }

        protected ParameterSyntax FirstTypeParameter(SyntaxNodeAnalysisContext context, BaseMethodDeclarationSyntax method, string typeName)
        {
            if(method == null) {
                return null;
            }
            // Use parallel arrays to correlate semantic model with syntax model for paramter.
            var parameters = method.ParameterList?.Parameters.ToList();
            var semanticMethod = context.SemanticModel.GetDeclaredSymbol(method);
            var semanticParameters = semanticMethod.ConstructedFrom.Parameters.ToList();
            for(int i = 0; i < semanticParameters.Count; ++i) {
                var semanticParam = semanticParameters[i];
                var isChild = Inherits(semanticParam.Type, typeName);
                if(isChild) {
                    return parameters[i];
                }
            }
            return null;
        }

        protected bool PropertyInheritsFrom(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax property, string typeName)
        {
            if(property == null) {
                return false;
            }
            var semanticProperty = context.SemanticModel.GetDeclaredSymbol(property);
            if(semanticProperty == null) {
                return false;
            }
            return Inherits(semanticProperty.Type, typeName);
        }

        protected ExpressionSyntax NamedArgument(AttributeSyntax attribute, string argumentName)
        {
            if(attribute == null) {
                return null;
            }
            var list = attribute?.ArgumentList?.Arguments;
            if(list == null) {
                return null;
            }
            foreach(var item in list) {
                if(item.NameEquals?.Name.Identifier.ValueText == argumentName) {
                    return item.Expression;
                }
            }
            return null;
        }

        protected static bool HasVisibility(MemberDeclarationSyntax member, Visibility visibility)
        {
            var kind = SyntaxKind.PublicKeyword;
            switch(visibility) {
                case Visibility.Public:
                    kind = SyntaxKind.PublicKeyword;
                    break;
                case Visibility.Private:
                    kind = SyntaxKind.PrivateKeyword;
                    break;
                case Visibility.Protected:
                    kind = SyntaxKind.ProtectedKeyword;
                    break;
                case Visibility.Internal:
                    kind = SyntaxKind.InternalKeyword;
                    break;
            };
            return member.ChildTokens()?.Any(e => e.Kind() == kind) ?? false;
        }

        protected static bool IsAbstract(MemberDeclarationSyntax member)
        {
            return member.ChildTokens()?.Any(e => e.Kind() == SyntaxKind.AbstractKeyword) ?? false;
        }

        protected static bool IsStatic(MethodDeclarationSyntax method)
        {
            return method.ChildTokens()?.Any(e => e.Kind() == SyntaxKind.StaticKeyword) ?? false;
        }

        protected bool InheritsFrom(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax _class, string baseName)
        {
            var symbol = context.SemanticModel.GetDeclaredSymbol(_class);
            return Inherits(symbol, baseName);
        }

        private static bool Inherits(ITypeSymbol symbol, string baseName)
        {
            if(symbol?.Name == baseName) {
                return true;
            }
            while(symbol?.BaseType != null) {
                if(symbol?.Name == baseName) {
                    return true;
                }
                symbol = symbol?.BaseType;
            }
            return false;
        }

        protected bool Implements(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax _class, string interfaceName)
        {
            var symbol = context.SemanticModel.GetDeclaredSymbol(_class);
            return Implements(symbol, interfaceName);
        }

        private static bool Implements(ITypeSymbol symbol, string interfaceName)
        {
            while(symbol?.BaseType != null) {
                if(symbol.Interfaces.Any(e => e.Name == interfaceName)) {
                    return true;
                }
                symbol = symbol?.BaseType;
            }
            return false;
        }

        protected static bool InNamespace(ClassDeclarationSyntax _class, string namespaceName)
        {
            var _namespace = (_class.Parent as NamespaceDeclarationSyntax);
            return InNamespace(_namespace?.Name, namespaceName);
        }

        private static bool InNamespace(SyntaxNode _node, string namespaceName)
        {
            if(_node == null) {
                return false;
            }
            if(_node is IdentifierNameSyntax identifier) {
                return identifier.Identifier.ValueText == namespaceName;
            }
            if(_node is QualifiedNameSyntax qualified) {
                return InNamespace(qualified.Left, namespaceName) || InNamespace(qualified.Right, namespaceName);
            }
            return false;
        }

        protected static ClassDeclarationSyntax ClassForMember(SyntaxNode member)
        {
            return member.FirstAncestorOrSelf<ClassDeclarationSyntax>(e => e is ClassDeclarationSyntax);
        }

        protected bool AnyReturnMatches(MethodDeclarationSyntax method, out IdentifierNameSyntax identifier, params string[] objectNames)
        {
            identifier = null;
            if(method.ReturnType is IdentifierNameSyntax ident) {
                if(objectNames.Any(e => e == ident.Identifier.ValueText)) {
                    identifier = ident;
                    return true;
                }
            }
            return false;
        }

    }
}
