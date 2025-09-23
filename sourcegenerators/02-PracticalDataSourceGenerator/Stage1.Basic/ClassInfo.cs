using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stage1.Basic;

internal class ClassInfo
{
    public INamedTypeSymbol Symbol { get; }
    public ClassDeclarationSyntax Declaration { get; }
    public AttributeData AttributeData { get; }

    // cannot use class records because of missing .NET support: IsExternalInit is not defined
    public ClassInfo(INamedTypeSymbol symbol, ClassDeclarationSyntax declaration, AttributeData attributeData)
    {
        Symbol = symbol;
        Declaration = declaration;
        AttributeData = attributeData;
    }
}