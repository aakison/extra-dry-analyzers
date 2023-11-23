using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace ExtraDry.Analyzers;

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
        if(rules.TryGetValue(code, out DiagnosticDescriptor value)) {
            Rule = value;
        } else {
            Rule = new DiagnosticDescriptor($"DRY{code}", title, message, category.ToString(), severity,
                isEnabledByDefault: true, description: description);
            rules.TryAdd(code, Rule);
        }
    }

    /// <summary>
    /// Diagnostics are run in parallel for performance reasons, so our caching mechanism must be thread safe.
    /// </summary>
    private static readonly ConcurrentDictionary<int, DiagnosticDescriptor> rules = new();

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

    protected static bool HasAttribute(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax _class, string attributeName, out AttributeSyntax attribute)
    {
        return HasAnyAttribute(context, _class, out attribute, attributeName);
    }

    protected static bool HasAnyAttribute(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax _class, out AttributeSyntax attribute, params string[] attributeNames)
    {
        if(attributeNames == null || attributeNames.Length == 0 || _class == null) {
            attribute = null;
            return false;
        }
        var fullNames = attributeNames.Select(e => e.EndsWith("Attribute") ? e : $"{e}Attribute");
        var attributes = _class.AttributeLists.SelectMany(e => e.Attributes) ?? Array.Empty<AttributeSyntax>();
        return AnyAttributeMatches(context, out attribute, fullNames, attributes);
    }

    protected static bool HasAttribute(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax method, string attributeName, out AttributeSyntax attribute)
    {
        return HasAnyAttribute(context, method, out attribute, attributeName);
    }

    protected static bool HasAnyAttribute(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax method, out AttributeSyntax attribute, params string[] attributeNames)
    {
        if(attributeNames == null || attributeNames.Length == 0 || method == null) {
            attribute = null;
            return false;
        }
        var fullNames = attributeNames.Select(e => e.EndsWith("Attribute") ? e : $"{e}Attribute");
        var attributes = method.AttributeLists.SelectMany(e => e.Attributes);
        return AnyAttributeMatches(context, out attribute, fullNames, attributes);
    }

    protected static bool HasAttribute(SyntaxNodeAnalysisContext context, ParameterSyntax parameter, string attributeName, out AttributeSyntax attribute)
    {
        return HasAnyAttribute(context, parameter, out attribute, attributeName);
    }

    protected static bool HasAnyAttribute(SyntaxNodeAnalysisContext context, ParameterSyntax parameter, out AttributeSyntax attribute, params string[] attributeNames)
    {
        if(attributeNames == null || attributeNames.Length == 0 || parameter == null) {
            attribute = null;
            return false;
        }
        var fullNames = attributeNames.Select(e => e.EndsWith("Attribute") ? e : $"{e}Attribute");
        var attributes = parameter.AttributeLists.SelectMany(e => e.Attributes);
        return AnyAttributeMatches(context, out attribute, fullNames, attributes);
    }

    protected static bool HasAttribute(SyntaxNodeAnalysisContext context, EnumDeclarationSyntax _enum, string attributeName, out AttributeSyntax attribute)
    {
        return HasAnyAttribute(context, _enum, out attribute, attributeName);
    }

    protected static bool HasAnyAttribute(SyntaxNodeAnalysisContext context, EnumDeclarationSyntax _enum, out AttributeSyntax attribute, params string[] attributeNames)
    {
        if(attributeNames == null || attributeNames.Length == 0 || _enum == null) {
            attribute = null;
            return false;
        }
        var fullNames = attributeNames.Select(e => e.EndsWith("Attribute") ? e : $"{e}Attribute");
        var attributes = _enum.AttributeLists.SelectMany(e => e.Attributes) ?? Array.Empty<AttributeSyntax>();
        return AnyAttributeMatches(context, out attribute, fullNames, attributes);
    }


    protected static bool HasAttribute(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax property, string attributeName, out AttributeSyntax attribute)
    {
        return HasAnyAttribute(context, property, out attribute, attributeName);
    }

    protected static bool HasAnyAttribute(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax property, out AttributeSyntax attribute, params string[] attributeNames)
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

    protected static ExpressionSyntax FirstArgument(AttributeSyntax attribute)
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

    protected static ExpressionSyntax NthArgument(AttributeSyntax attribute, int index)
    {
        if(attribute == null) {
            return null;
        }
        if(index >= attribute.ArgumentList?.Arguments.Count) {
            return null;
        }
        var nth = attribute?.ArgumentList?.Arguments.Skip(index).FirstOrDefault();
        if(nth == null || nth.NameEquals != null) {
            return null;
        }
        else {
            return nth.Expression;
        }
    }

    protected static ParameterSyntax FirstParameter(MethodDeclarationSyntax method)
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

    protected static ParameterSyntax[] Parameters(MethodDeclarationSyntax method)
    {
        return method?.ParameterList?.Parameters.ToArray() ?? Array.Empty<ParameterSyntax>();
    }

    protected static ParameterSyntax FirstTypeParameter(SyntaxNodeAnalysisContext context, BaseMethodDeclarationSyntax method, string typeName)
    {
        if(method == null) {
            return null;
        }
        // Use parallel arrays to correlate semantic model with syntax model for parameter.
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

    protected static bool PropertyInheritsFrom(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax property, string typeName)
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

    protected static ExpressionSyntax NamedArgument(AttributeSyntax attribute, string argumentName)
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

    protected static bool HasVisibility(CSharpSyntaxNode member, Visibility visibility)
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
        return member.ChildTokens()?.Any(e => e.IsKind(kind)) ?? false;
    }

    protected static bool IsAbstract(MemberDeclarationSyntax member)
    {
        return member.ChildTokens()?.Any(e => e.IsKind(SyntaxKind.AbstractKeyword)) ?? false;
    }

    protected static bool IsStatic(MethodDeclarationSyntax method)
    {
        return method.ChildTokens()?.Any(e => e.IsKind(SyntaxKind.StaticKeyword)) ?? false;
    }

    protected static bool InheritsFrom(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax _class, string baseName)
    {
        if(_class == null) {
            return false;
        }
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

    protected static bool Implements(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax _class, string interfaceName)
    {
        if(_class == null) {
            return false;
        }
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
        if(_class == null) {
            return false;
        }
        var _namespace = (_class.Parent as NamespaceDeclarationSyntax)?.Name
            ?? (_class.Parent as FileScopedNamespaceDeclarationSyntax)?.Name;
        return InNamespace(_namespace, namespaceName);
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
        return member.FirstAncestorOrSelf<ClassDeclarationSyntax>(e => e is not null);
    }

    protected static bool AnyReturnMatches(MethodDeclarationSyntax method, out IdentifierNameSyntax identifier, params string[] objectNames)
    {
        identifier = null;
        var name = method.ReturnType.ToString();
        if(objectNames.Any(e => e == name)) {
            identifier = method.ReturnType as IdentifierNameSyntax;
            return true;
        }
        return false;
    }

    protected static bool ReturnMatches(MethodDeclarationSyntax method, out IdentifierNameSyntax identifier, Regex regex)
    {
        identifier = null;
        var name = method.ReturnType.ToString();
        if(regex.IsMatch(name)) {
            identifier = method.ReturnType as IdentifierNameSyntax;
            return true;
        }
        return false;
    }

    protected static bool HasProperty(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax @class, string propertyName)
    {
        var symbol = context.SemanticModel?.GetDeclaredSymbol(@class);
        if(symbol == null) {
            return false;
        }
        var baseClass = symbol;
        while(baseClass.Name != "Object") {
            var members = baseClass.MemberNames;
            if(members.Any(e => e == propertyName)) {
                return true;
            }
            baseClass = baseClass.BaseType;
        }
        return false;
    }

}
