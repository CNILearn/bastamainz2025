# Multi-Stage Practical Source Generator Demo

This repository demonstrates the evolution of a C# source generator through multiple stages, showcasing progressive improvements in caching strategies, data source integration, and performance optimization.

## Overview

The **PracticalDataSourceGenerator** project is a comprehensive educational demonstration that shows how to build increasingly sophisticated source generators. Each stage builds upon the previous one, adding new capabilities while demonstrating measurable performance improvements.

## Repository Structure

```
src/PracticalDataSourceGenerator/
├── Stage1.Basic/                    # Baseline implementation
├── Stage2.FileIntegration/          # Multiple data sources
├── Stage3.BasicCaching/             # Basic caching strategies
├── Stage4.AdvancedCaching/          # Advanced optimizations (planned)
├── Stage5.Optimized/                # Final optimized version (planned)
└── README.md                        # This file
```

## Progressive Stages

### 🚀 Stage 1: Basic Data Source Generator
**Purpose**: Establish baseline functionality and performance metrics

- **Features**: Basic incremental source generation with attribute-based configuration
- **Data Sources**: Single (attributes only)
- **Caching**: None - recalculates everything every build
- **Performance**: Baseline for comparison

**Key Learning**: Foundation of incremental source generators, attribute processing

### 📁 Stage 2: File Integration Data Source Generator  
**Purpose**: Demonstrate multiple data source integration

- **Features**: Combines attributes with external JSON configuration files
- **Data Sources**: Multiple (attributes + JSON files)
- **Caching**: None - reparses files every build
- **Performance**: Slower than Stage 1 due to file I/O operations

**Key Learning**: AdditionalFiles provider usage, external data integration patterns

### ⚡ Stage 3: Basic Caching Data Source Generator
**Purpose**: Introduce caching mechanisms for performance improvement

- **Features**: Content-hash based caching, incremental provider optimization
- **Data Sources**: Multiple (cached attributes + JSON files)
- **Caching**: Basic - caches parsed configurations based on content changes
- **Performance**: Significantly improved for unchanged configurations

**Key Learning**: Incremental provider chaining, content change detection, basic caching strategies

### 🏎️ Stage 4: Advanced Caching
**Purpose**: Implement sophisticated caching strategies for optimal performance

- **Features**: Multi-level caching hierarchies, cross-build persistence, advanced change detection
- **Data Sources**: Multiple with external API simulation and dependency tracking
- **Caching**: Advanced - L1 (in-memory), L2 (persistent), L3 (distributed simulation)
- **Performance**: Optimized for complex scenarios with comprehensive monitoring

**Key Learning**: Multi-level cache architecture, dependency tracking, performance metrics, hierarchical invalidation

### 🏆 Stage 5: Optimized (Planned)
**Purpose**: Final optimized version with comprehensive benchmarking

- **Features**: All optimizations, comprehensive performance monitoring
- **Data Sources**: Multiple with full optimization
- **Caching**: Complete optimization with benchmarking
- **Performance**: Peak performance with detailed metrics

## Performance Comparison

| Stage | Build Time | Memory Usage | File I/O | Configuration Parsing | Caching |
|-------|------------|--------------|----------|----------------------|---------|
| Stage 1 | Baseline | Low | None | N/A | None |
| Stage 2 | +40% | Medium | Every build | Every build | None |
| Stage 3 | +10% | Medium | On changes | On changes | Basic |
| Stage 4 | -20% | Medium-High | Optimized | Cached | Advanced |
| Stage 5 | TBD | TBD | Minimal | Optimized | Complete |

*Performance metrics are illustrative and will vary based on project size and complexity*

## Getting Started

### Prerequisites
- .NET 9.0 SDK or later
- C# 12.0 language support
- MSBuild-based project system

### Building All Stages

```bash
# Navigate to the source directory
cd src

# Build all stages
dotnet build PracticalDataSourceGenerator/Stage1.Basic/Stage1.Basic.csproj
dotnet build PracticalDataSourceGenerator/Stage2.FileIntegration/Stage2.FileIntegration.csproj
dotnet build PracticalDataSourceGenerator/Stage3.BasicCaching/Stage3.BasicCaching.csproj

# Run sample applications
dotnet run --project PracticalDataSourceGenerator/Stage1.Basic.Sample
dotnet run --project PracticalDataSourceGenerator/Stage2.FileIntegration.Sample
dotnet run --project PracticalDataSourceGenerator/Stage3.BasicCaching.Sample

# Run tests
dotnet test PracticalDataSourceGenerator/Stage1.Basic.Tests
dotnet test PracticalDataSourceGenerator/Stage2.FileIntegration.Tests
dotnet test PracticalDataSourceGenerator/Stage3.BasicCaching.Tests
```

### Quick Start with a Single Stage

```bash
# Try Stage 3 (Basic Caching) - most feature-complete
cd src/PracticalDataSourceGenerator/Stage3.BasicCaching.Sample
dotnet run
```

## Educational Value

This demo teaches:

1. **Incremental Source Generator Patterns**: Best practices for `IIncrementalGenerator`
2. **Multiple Data Source Integration**: Combining attributes, files, and external data
3. **Caching Strategies**: From simple to sophisticated caching mechanisms
4. **Performance Optimization**: Measurable improvements through caching
5. **Real-world Patterns**: Practical source generator development techniques

## Code Generation Examples

Each stage generates progressively more sophisticated data factories:

### Stage 1: Basic Generation
```csharp
[DataSource(EntityName = "User", Count = 5)]
public class User { /* properties */ }

// Generates:
public static class UserDataFactory
{
    public static User CreateSample() { /* basic implementation */ }
    public static List<User> CreateSamples(int count = 5) { /* basic implementation */ }
}
```

### Stage 2: With External Configuration
```csharp
// User.datasource.json provides templates
// Generates enhanced factory with template-based data:
Name = new[] { "Alice Johnson", "Bob Smith", "Carol Williams" }[_random.Next(3)]
```

### Stage 3: With Caching Information
```csharp
// Generates additional diagnostic methods:
public static string GetCachingStats() { /* caching information */ }
public static string GetConfigurationInfo() { /* cached config details */ }
```

## Advanced Features Demonstrated

- **Incremental Providers**: Efficient provider chaining for optimal performance
- **Content Change Detection**: Hash-based detection for cache invalidation
- **Diagnostic Integration**: Rich error reporting and performance metrics
- **Template Systems**: Flexible data generation with external templates
- **Fallback Mechanisms**: Graceful degradation when external resources unavailable

## Use Cases

This demo is valuable for:

- **Learning Source Generators**: Progressive complexity for educational purposes
- **Performance Optimization**: Understanding caching impact on build times
- **Architecture Patterns**: Real-world source generator design patterns
- **Benchmarking**: Baseline for custom source generator performance testing

## Contributing

This is an educational demonstration. The focus is on clear, well-documented progression through increasingly sophisticated source generator techniques.

## Documentation

Each stage includes:
- Detailed README explaining concepts and implementation
- Comprehensive code comments
- Sample applications demonstrating usage
- Unit tests validating functionality
- Performance characteristics documentation

## Next Steps

- Complete Stage 5 (Optimized) implementation
- Add comprehensive performance measurement tools
- Create production-ready caching strategies
- Add cross-platform build validation

---

This multi-stage demo provides a comprehensive learning path for source generator development, from basic concepts to advanced optimization techniques.