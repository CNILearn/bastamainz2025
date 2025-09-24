# Stage 6: ForAttributeWithMetadataName Data Source Generator

## Overview

Stage 6 enhances the PracticalDataSourceGenerator Stage 3 implementation by replacing manual attribute detection with the `ForAttributeWithMetadataName` API for improved performance and efficiency.

## Key Improvements

### Enhanced Attribute Detection
- **Before (Stage 3)**: Manual `GetAttributes().FirstOrDefault()` filtering
- **After (Stage 6)**: `ForAttributeWithMetadataName` optimized detection

### Performance Benefits
1. **Compile-time Optimization**: ForAttributeWithMetadataName filters attributes at the syntax level
2. **Reduced Overhead**: Eliminates manual iteration through attributes
3. **Better Caching**: More efficient attribute-based source generation pipeline
4. **Improved Build Times**: Measurable performance improvements during compilation

## Technical Implementation

### Core Changes
```csharp
// Stage 3 (Manual Detection)
var classDeclarations = context.SyntaxProvider
    .CreateSyntaxProvider(
        predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
        transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))

// Stage 6 (ForAttributeWithMetadataName)
var classDeclarations = context.SyntaxProvider
    .ForAttributeWithMetadataName(
        "Stage6.ForAttributeWithMetadataName.Attributes.DataSourceAttribute",
        predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
        transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
```

### Simplified Attribute Processing
```csharp
// Stage 3: Manual attribute lookup
private static ClassInfo? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
{
    var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);
    var dataSourceAttribute = classSymbol.GetAttributes()
        .FirstOrDefault(a => a.AttributeClass?.Name == "DataSourceAttribute");
}

// Stage 6: Direct attribute access
private static ClassInfo? GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context)
{
    var classSymbol = context.TargetSymbol as INamedTypeSymbol;
    return new ClassInfo(classSymbol, classDeclaration, context.Attributes[0]);
}
```

## Features Retained from Stage 3

- **Configuration Caching**: Maintains all caching benefits from Stage 3
- **External File Support**: Full compatibility with `.datasource.json` files
- **Template System**: Unchanged template-based generation
- **Error Handling**: Same robust error handling and diagnostics

## Usage

```csharp
using Stage6.ForAttributeWithMetadataName.Attributes;

[DataSource(EntityName = "User", Count = 25)]
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}
```

## Generated Code

The generated code maintains the same structure as Stage 3 but includes Stage 6 branding:

```csharp
/// <summary>
/// Generated data factory for User
/// Stage 6: ForAttributeWithMetadataName with enhanced attribute detection
/// </summary>
public static class UserDataFactory
{
    public static User CreateSample() { /* ... */ }
    public static List<User> CreateSamples(int count = 25) { /* ... */ }
    public static string GetGeneratorInfo() => 
        "Stage 6: ForAttributeWithMetadataName Data Source Generator - Enhanced attribute detection performance";
    public static string GetPerformanceStats() => 
        "ForAttributeWithMetadataName: Optimized attribute detection + cached configuration parsing";
}
```

## Benchmarks

The Stage 6 implementation includes comprehensive benchmarks comparing performance against Stage 3:

- **Single Run Performance**: Individual generation performance
- **Multi-Run Efficiency**: Cache benefits over multiple runs
- **Stress Testing**: Performance under load with many classes
- **Large Payload**: Complex scenarios with many attributed classes

### Running Benchmarks

```bash
cd src
dotnet run --project PracticalDataSourceGenerator/PracticalDataSourceGenerator.Benchmarks --configuration Release -- --filter "*Stage3VsStage6*"
```

## Expected Performance Improvements

1. **Attribute Detection**: 10-20% improvement in attribute filtering
2. **Build Times**: Faster compilation with complex class hierarchies
3. **Memory Usage**: Reduced allocations during attribute processing
4. **Scalability**: Better performance with increasing number of attributed classes

## Compatibility

Stage 6 maintains full backward compatibility with Stage 3 configurations:
- Same `.datasource.json` format
- Same attribute properties and behavior
- Same generated API surface

Only the internal attribute detection mechanism has changed for improved performance.