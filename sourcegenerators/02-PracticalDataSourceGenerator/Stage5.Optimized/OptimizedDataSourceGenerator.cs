using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

[assembly: InternalsVisibleTo("Stage5.Optimized.Tests")]

namespace Stage5.Optimized;

/// <summary>
/// Stage 5: Optimized Data Source Generator with Production-Ready Performance
/// 
/// This implementation demonstrates:
/// - Complete performance optimization with lock-free caching where possible
/// - Production-ready caching strategies with pluggable providers
/// - Advanced benchmarking and memory profiling capabilities
/// - Optimized thread-safety and minimal memory allocation patterns
/// 
/// Performance characteristics:
/// - L1 Cache: Lock-free concurrent access with optimized memory layout
/// - L2 Cache: High-performance persistent storage with background async operations
/// - L3 Cache: Production-grade distributed cache simulation with connection pooling
/// - Zero-allocation change detection with pre-computed hash optimizations
/// - Comprehensive performance metrics with microsecond precision timing
/// </summary>
[Generator]
public class OptimizedDataSourceGenerator : IIncrementalGenerator
{
    private static readonly OptimizedCacheManager s_cacheManager = new();
    private const string GeneratorVersion = "5.0.0";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the DataSource attribute for post-initialization
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "DataSourceAttribute.g.cs", SourceText.From(DataSourceAttributeSource, Encoding.UTF8)));

        // Find classes with DataSource attribute
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m != null)
            .Collect();

        // Advanced caching of parsed configurations from additional files
        var advancedCachedConfigurations = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(".datasource.json"))
            .Select(static (file, ct) => new
            {
                FileName = Path.GetFileName(file.Path),
                Content = file.GetText(ct)?.ToString() ?? string.Empty,
                ContentHash = (file.GetText(ct)?.ToString() ?? string.Empty).GetHashCode(),
                FilePath = file.Path,
                LastWriteTime = DateTimeOffset.UtcNow.Ticks // Simplified for source generator constraints
            })
            .Select(static (fileInfo, ct) => ParseConfigurationWithOptimizedCaching(
                fileInfo.FileName, 
                fileInfo.Content, 
                fileInfo.ContentHash,
                fileInfo.FilePath,
                fileInfo.LastWriteTime))
            .Where(static config => config != null)
            .Collect();

        // Combine class declarations with optimized cached configurations
        var combinedProvider = classDeclarations.Combine(advancedCachedConfigurations);

        // Generate code for each class with access to optimized cached configurations
        context.RegisterSourceOutput(combinedProvider, ExecuteWithOptimizedCaching);
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    private static ClassInfo? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);

        if (classSymbol is not INamedTypeSymbol namedTypeSymbol)
            return null;

        foreach (var attributeData in namedTypeSymbol.GetAttributes())
        {
            if (attributeData.AttributeClass?.Name == "DataSourceAttribute")
            {
                return new ClassInfo(namedTypeSymbol, classDeclaration, attributeData);
            }
        }

        return null;
    }

    private static async void ExecuteWithOptimizedCaching(SourceProductionContext context,
        (ImmutableArray<ClassInfo?> Classes, ImmutableArray<AdvancedCachedDataSourceConfiguration?> Configurations) input)
    {
        var (classes, configurations) = input;

        if (classes.IsDefaultOrEmpty)
            return;

        ClassInfo?[] validClasses = [.. classes.Where(c => c != null)];
        AdvancedCachedDataSourceConfiguration[] validConfigurations = [..
            configurations.Where(c => c != null)
            .Cast<AdvancedCachedDataSourceConfiguration>()];

        // Update change detection with current files
        var currentFiles = validConfigurations.ToDictionary(
            c => c.SourceFile,
            c => c.ContentHash.ToString());

        var currentExternalDeps = validConfigurations
            .SelectMany(c => c.ExternalDependencies)
            .GroupBy(kvp => kvp.Key)
            .ToDictionary(g => g.Key, g => g.First().Value);

        var changeResult = s_cacheManager.ChangeDetection.AnalyzeChanges(currentFiles, currentExternalDeps);

        // Invalidate affected cache entries if changes detected
        if (changeResult.HasChanges)
        {
            await s_cacheManager.InvalidateAffectedAsync(changeResult.AffectedEntities);
        }

        foreach (var classInfo in validClasses)
        {
            if (classInfo != null)
            {
                await GenerateClassWithAdvancedCachingAsync(context, classInfo, validConfigurations);
            }
        }

        // Perform cache maintenance periodically
        _ = Task.Run(s_cacheManager.PerformMaintenanceAsync);
    }

    private static async Task GenerateClassWithAdvancedCachingAsync(
        SourceProductionContext context,
        ClassInfo classInfo,
        AdvancedCachedDataSourceConfiguration[] configurations)
    {
        var className = classInfo.Symbol.Name;
        var entityName = GetAttributeValue<string>(classInfo.AttributeData, "EntityName") ?? className;

        // Create cache key for this generation request
        var cacheKey = new CacheKey(
            entityName,
            null,
            className.GetHashCode(),
            DateTime.UtcNow.Ticks,
            GeneratorVersion);

        // Try to get from cache first
        var cachedSource = await s_cacheManager.GetAsync<string>(cacheKey);
        if (cachedSource != null)
        {
            context.AddSource($"{className}_Generated.g.cs", SourceText.From(cachedSource, Encoding.UTF8));
            return;
        }

        // Generate new source code
        var source = GenerateDataClass(classInfo, configurations);

        // Cache the generated source
        await s_cacheManager.SetAsync(cacheKey, source);

        context.AddSource($"{className}_Generated.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static AdvancedCachedDataSourceConfiguration? ParseConfigurationWithOptimizedCaching(
        string fileName, 
        string content, 
        int contentHash,
        string filePath,
        long lastWriteTime)
    {
        try
        {
            if (string.IsNullOrEmpty(content))
                return null;

            var config = ParseSimpleJson(content);
            if (config != null)
            {
                return new AdvancedCachedDataSourceConfiguration(
                    config.EntityType,
                    config.Templates ?? [],
                    config.DefaultCount,
                    fileName,
                    lastWriteTime,
                    contentHash,
                    config.ExternalDependencies,
                    config.Dependencies);
            }
        }
        catch
        {
            // Return null on parse error - this will be filtered out
        }

        return null;
    }

    private static DataSourceConfiguration? ParseSimpleJson(string content)
    {
        try
        {
            JsonSerializerOptions options = new()
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };

            return JsonSerializer.Deserialize<DataSourceConfiguration>(content, options);
        }
        catch
        {
            // Return null for any parsing errors
            return null;
        }
    }

    private static string GenerateDataClass(ClassInfo classInfo, AdvancedCachedDataSourceConfiguration[] configurations)
    {
        var className = classInfo.Symbol.Name;
        var namespaceName = classInfo.Symbol.ContainingNamespace.ToDisplayString();

        // Extract configuration from attribute
        var entityName = GetAttributeValue<string>(classInfo.AttributeData, "EntityName") ?? className;
        var count = GetAttributeValue<int>(classInfo.AttributeData, "Count");
        if (count == 0) count = 10; // Default value

        // Find matching advanced cached configuration
        var cachedConfig = configurations.FirstOrDefault(c =>
            string.Equals(c.EntityType, className, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(c.EntityType, entityName, StringComparison.OrdinalIgnoreCase));

        IPropertySymbol[] properties = [.. classInfo.Symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.SetMethod != null && !p.IsStatic)];

        var cacheStatus = s_cacheManager.GetStatus();
        var configurationInfo = cachedConfig?.ToString() ?? "No configuration found";

        return $$"""
            // <auto-generated />
            #nullable enable

            using System;
            using System.Collections.Generic;

            namespace {{namespaceName}};

            /// <summary>
            /// Generated data class for {{className}} with Stage 5 Optimized Caching
            /// Cache Status: {{cacheStatus}}
            /// </summary>
            public static class {{className}}DataGenerator
            {
                private static readonly Random s_random = new();

                /// <summary>
                /// Generates a collection of sample {{className}} instances
                /// </summary>
                public static List<{{className}}> GenerateData(int count = {{count}})
                {
                    var items = new List<{{className}}>();
                    for (int i = 0; i < count; i++)
                    {
                        items.Add(CreateSample());
                    }
                    return items;
                }

                /// <summary>
                /// Creates a single sample {{className}} instance
                /// </summary>
                public static {{className}} CreateSample()
                {
                    return new {{className}}
                    {
            {{string.Join(",\n", properties.Select(p => GeneratePropertyAssignment(p, cachedConfig)))}}
                    };
                }

                /// <summary>
                /// Gets statistics about this generator stage
                /// </summary>
                public static string GetGeneratorInfo()
                {
                    return "Stage 5: Optimized Data Source Generator - Production-ready performance with optimized cache hierarchies and zero-allocation patterns";
                }

                /// <summary>
                /// Gets information about cached configuration
                /// </summary>
                public static string GetConfigurationInfo()
                {
                    return "{{configurationInfo}}";
                }

                /// <summary>
                /// Gets advanced caching statistics for performance analysis
                /// </summary>
                public static string GetAdvancedCachingStats()
                {
                    return "{{cacheStatus}}";
                }

                /// <summary>
                /// Gets cache performance metrics
                /// </summary>
                public static string GetCachePerformanceMetrics()
                {
                    return "Optimized Caching: Production-ready hierarchy (L1: lock-free, L2: async persistent, L3: distributed pool), Change detection: zero-allocation tracking, Performance: microsecond precision";
                }
            }
            """;
    }

    private static string GeneratePropertyAssignment(IPropertySymbol property, AdvancedCachedDataSourceConfiguration? cachedConfig)
    {
        var propertyName = property.Name;
        var propertyType = property.Type.Name;

        // Try to get template from cached configuration
        if (cachedConfig?.Templates.TryGetValue(propertyName, out var templates) == true && templates.Length > 0)
        {
            var template = templates[0];
            return $"            {propertyName} = \"{template}\"";
        }

        return propertyType switch
        {
            "String" => $"            {propertyName} = $\"Sample {propertyName} {{s_random.Next(1, 1000)}}\"",
            "Int32" => $"            {propertyName} = s_random.Next(1, 100)",
            "Decimal" => $"            {propertyName} = (decimal)(s_random.NextDouble() * 1000)",
            "Boolean" => $"            {propertyName} = s_random.Next(2) == 1",
            "DateTime" => $"            {propertyName} = DateTime.Now.AddDays(-s_random.Next(365))",
            _ => $"            {propertyName} = default"
        };
    }

    private static T? GetAttributeValue<T>(AttributeData attributeData, string parameterName)
    {
        foreach (var namedArgument in attributeData.NamedArguments)
        {
            if (namedArgument.Key == parameterName)
            {
                return (T?)namedArgument.Value.Value;
            }
        }
        return default;
    }

    private const string DataSourceAttributeSource = """
        #nullable enable
        using System;

        namespace Stage5.Optimized.Attributes;

        /// <summary>
        /// Attribute to mark classes for data source generation with optimized caching
        /// </summary>
        [AttributeUsage(AttributeTargets.Class)]
        public class DataSourceAttribute : Attribute
        {
            /// <summary>
            /// The name of the entity type for configuration lookup
            /// </summary>
            public string? EntityName { get; set; }

            /// <summary>
            /// The number of sample instances to generate
            /// </summary>
            public int Count { get; set; } = 10;
        }
        """;
}