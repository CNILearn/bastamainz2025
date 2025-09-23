using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stage4.AdvancedCaching;

internal class ClassInfo(INamedTypeSymbol symbol, ClassDeclarationSyntax declaration, AttributeData attributeData)
{
    public INamedTypeSymbol Symbol { get; } = symbol;
    public ClassDeclarationSyntax Declaration { get; } = declaration;
    public AttributeData AttributeData { get; } = attributeData;
}