# Stage 3: Basic Caching Data Source Generator

## Overview

Stage 3 introduces basic caching mechanisms to improve source generator performance. This stage demonstrates how to implement caching strategies in incremental source generators to avoid redundant operations and improve build times.

## Key Features

- **Content-Based Caching**: Configurations cached based on content hash changes
- **Incremental Provider Optimization**: Uses advanced incremental provider chaining
- **Cache-Aware Generation**: Only regenerates when source data actually changes
- **Performance Monitoring**: Includes caching statistics for performance analysis

## Caching Strategy

Stage 3 implements several caching optimizations:

### 1. Content Hash Caching
```csharp
// Cache configurations based on content changes
var cachedConfigurations = context.AdditionalTextsProvider
    .Where(static file => file.Path.EndsWith(".datasource.json"))
    .Select(static (file, ct) => new {
        FileName = Path.GetFileName(file.Path),
        Content = file.GetText(ct)?.ToString() ?? string.Empty,
        ContentHash = (file.GetText(ct)?.ToString() ?? string.Empty).GetHashCode()
    })
    .Select(static (fileInfo, ct) => ParseConfigurationWithCaching(...))
```

### 2. Cached Configuration Objects
```csharp
internal class CachedDataSourceConfiguration
{
    public string EntityType { get; }
    public Dictionary<string, string[]> Templates { get; }
    public string SourceFile { get; }
    public int ContentHash { get; } // For change detection
}
```

### 3. Incremental Provider Chaining
- Configurations are parsed once and cached
- Only regenerates when file content actually changes
- Avoids redundant JSON parsing operations

## Generated Code Enhancements

Stage 3 adds caching information to generated factories:

```csharp
public static class UserDataFactory
{
    public static string GetCachingStats()
    {
        return "Caching: Configuration parsed once and reused until file changes detected";
    }
    
    public static string GetConfigurationInfo()
    {
        return "Cached configuration: User.datasource.json, Templates: 2, Hash: 123456";
    }
}
```

## Performance Characteristics

- **First Build**: Same as Stage 2 (parses all configurations)
- **Subsequent Builds**: Only reparses configurations if content changes
- **Memory Usage**: Slightly higher due to cached objects
- **Build Time**: Significantly improved for unchanged configurations

## Performance Comparison

| Stage | Configuration Parsing | File I/O Operations | Build Performance |
|-------|---------------------|-------------------|------------------|
| Stage 1 | N/A | None | Baseline |
| Stage 2 | Every build | Every build | Slower |
| Stage 3 | Only on changes | Only on changes | **Faster** |

## Building and Running

```bash
# Build the generator
dotnet build Stage3.BasicCaching/Stage3.BasicCaching.csproj

# Build and run the sample
dotnet run --project Stage3.BasicCaching.Sample/Stage3.BasicCaching.Sample.csproj

# Run tests
dotnet test Stage3.BasicCaching.Tests/Stage3.BasicCaching.Tests.csproj
```

## Learning Notes

Stage 3 demonstrates incremental source generator patterns but reveals important considerations:

- **Incremental Provider Complexity**: Advanced caching requires careful provider chaining
- **Content Change Detection**: Hash-based change detection for file content
- **Performance Trade-offs**: Memory usage vs. build performance optimization

## Next Stage

Stage 4 will implement advanced caching strategies including:
- Multi-level caching hierarchies
- Cross-build caching persistence
- Advanced change detection algorithms
- Comprehensive performance benchmarking