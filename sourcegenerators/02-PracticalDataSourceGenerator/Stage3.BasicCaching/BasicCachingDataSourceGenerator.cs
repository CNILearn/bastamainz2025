using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace Stage3.BasicCaching;

/// <summary>
/// Stage 3: Basic Caching Data Source Generator
/// 
/// This implementation demonstrates:
/// - Basic caching strategies in source generator pipelines
/// - Incremental generator optimizations with cached values
/// - Caching of external file content and parsed configurations
/// - Performance improvements through avoiding redundant operations
/// 
/// Performance characteristics:
/// - Cached external file parsing: Files only parsed when content changes
/// - Cached configuration objects: Configurations reused across builds
/// - Incremental provider chaining: Only regenerates when dependencies change
/// - Measurable build performance improvements over Stage 2
/// </summary>
[Generator]
public class BasicCachingDataSourceGenerator : IIncrementalGenerator
{
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

        // Cache parsed configurations from additional files
        IncrementalValueProvider<ImmutableArray<CachedDataSourceConfiguration?>> cachedConfigurations = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(".datasource.json"))
            .Select(static (file, ct) => new
            {
                FileName = Path.GetFileName(file.Path),
                Content = file.GetText(ct)?.ToString() ?? string.Empty,
                // Use content hash instead of file system access for caching
                ContentHash = (file.GetText(ct)?.ToString() ?? string.Empty).GetHashCode()
            })
            .Select(static (fileInfo, ct) => ParseConfigurationWithCaching(fileInfo.FileName, fileInfo.Content, fileInfo.ContentHash))
            .Where(static config => config != null)
            .Collect();

        // Combine class declarations with cached configurations
        var combinedProvider = classDeclarations.Combine(cachedConfigurations);

        // Generate code for each class with access to cached configurations
        context.RegisterSourceOutput(combinedProvider, Execute);
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    private static ClassInfo? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;

        if (classSymbol == null)
            return null;

        // Check if the class has the DataSource attribute
        var dataSourceAttribute = classSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "DataSourceAttribute");

        if (dataSourceAttribute == null)
            return null;

        return new ClassInfo(
            classSymbol,
            classDeclaration,
            dataSourceAttribute);
    }

    private static void Execute(SourceProductionContext context, 
        (ImmutableArray<ClassInfo?> Classes, ImmutableArray<CachedDataSourceConfiguration?> Configurations) input)
    {
        var (classes, configurations) = input;
        
        if (classes.IsDefaultOrEmpty)
            return;

        ClassInfo?[] validClasses = [.. classes.Where(c => c != null)];
        CachedDataSourceConfiguration[] validConfigurations = [.. 
            configurations.Where(c => c != null)
            .Cast<CachedDataSourceConfiguration>()];

        foreach (var classInfo in validClasses)
        {
            if (classInfo != null)
            {
                var source = GenerateDataClass(classInfo, validConfigurations);
                context.AddSource($"{classInfo.Symbol.Name}_Generated.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }
    }

    private static CachedDataSourceConfiguration? ParseConfigurationWithCaching(string fileName, string content, int contentHash)
    {
        try
        {
            if (string.IsNullOrEmpty(content))
                return null;

            var config = ParseSimpleJson(content);
            if (config != null)
            {
                return new CachedDataSourceConfiguration(
                    config.EntityType,
                    config.Templates ?? [],
                    config.DefaultCount,
                    fileName,
                    0, // No file system access allowed
                    contentHash); // Use content hash for change detection
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
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;

            var config = new DataSourceConfiguration();

            // Parse entityType
            if (root.TryGetProperty("entityType", out var entityTypeElement))
            {
                config.EntityType = entityTypeElement.GetString() ?? string.Empty;
            }

            // Parse defaultCount
            if (root.TryGetProperty("defaultCount", out var defaultCountElement))
            {
                if (defaultCountElement.TryGetInt32(out var count))
                {
                    config.DefaultCount = count;
                }
            }

            // Parse templates object
            if (root.TryGetProperty("templates", out var templatesElement))
            {
                var templates = new Dictionary<string, string[]>();

                foreach (var property in templatesElement.EnumerateObject())
                {
                    if (property.Value.ValueKind == JsonValueKind.Array)
                    {
                        var values = new List<string>();
                        foreach (var arrayElement in property.Value.EnumerateArray())
                        {
                            if (arrayElement.ValueKind == JsonValueKind.String)
                            {
                                var value = arrayElement.GetString();
                                if (value != null)
                                {
                                    values.Add(value);
                                }
                            }
                        }

                        if (values.Count > 0)
                        {
                            templates[property.Name] = values.ToArray();
                        }
                    }
                }

                if (templates.Count > 0)
                {
                    config.Templates = templates;
                }
            }

            return config;
        }
        catch (JsonException)
        {
            // Return null for invalid JSON - will be handled by caller
            return null;
        }
        catch (System.Exception)
        {
            // Return null for any other parsing errors
            return null;
        }
    }

    private static string GenerateDataClass(ClassInfo classInfo, CachedDataSourceConfiguration[] configurations)
    {
        var className = classInfo.Symbol.Name;
        var namespaceName = classInfo.Symbol.ContainingNamespace.ToDisplayString();
        
        // Extract configuration from attribute
        var entityName = GetAttributeValue<string>(classInfo.AttributeData, "EntityName") ?? className;
        var count = GetAttributeValue<int>(classInfo.AttributeData, "Count");
        if (count == 0) count = 10; // Default value

        // Find matching cached configuration
        var cachedConfig = configurations.FirstOrDefault(c => 
            string.Equals(c.EntityType, className, System.StringComparison.OrdinalIgnoreCase) ||
            string.Equals(c.EntityType, entityName, System.StringComparison.OrdinalIgnoreCase));

        IPropertySymbol[] properties = [.. classInfo.Symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.SetMethod != null && !p.IsStatic)];

        string[] propertyGenerators = [.. properties.Select(p => GeneratePropertyAssignment(p, cachedConfig))];

        var configurationInfo = cachedConfig != null 
            ? $"Cached configuration: {cachedConfig.SourceFile}, Templates: {cachedConfig.Templates.Count}, Hash: {cachedConfig.ContentHash}"
            : "No cached configuration found";

        return $$"""
            // <auto-generated/>
            // Stage 3: Basic Caching Data Source Generator
            // Generated from: {{className}}
            // {{configurationInfo}}
            // Caching enabled - configurations cached based on file content changes
            #nullable enable

            using System;
            using System.Collections.Generic;
            using System.Linq;

            namespace {{namespaceName}};

            /// <summary>
            /// Generated data factory for {{entityName}}
            /// Stage 3: Basic caching with optimized file operations
            /// </summary>
            public static class {{className}}DataFactory
            {
                private static readonly Random _random = new();

                /// <summary>
                /// Creates a single sample {{entityName}}
                /// </summary>
                public static {{className}} CreateSample()
                {
                    return new {{className}}
                    {
                        {{string.Join(",\n            ", propertyGenerators)}}
                    };
                }

                /// <summary>
                /// Creates multiple sample {{entityName}} instances
                /// </summary>
                public static List<{{className}}> CreateSamples(int count = {{count}})
                {
                    var items = new List<{{className}}>();
                    for (int i = 0; i < count; i++)
                    {
                        items.Add(CreateSample());
                    }
                    return items;
                }

                /// <summary>
                /// Gets statistics about this generator stage
                /// </summary>
                public static string GetGeneratorInfo()
                {
                    return "Stage 3: Basic Caching Data Source Generator - Cached configurations, improved build performance";
                }

                /// <summary>
                /// Gets information about cached configuration
                /// </summary>
                public static string GetConfigurationInfo()
                {
                    return "{{configurationInfo}}";
                }

                /// <summary>
                /// Gets caching statistics for performance analysis
                /// </summary>
                public static string GetCachingStats()
                {
                    return "Caching: Configuration parsed once and reused until file changes detected";
                }
            }
            """;
    }

    private static string GeneratePropertyAssignment(IPropertySymbol property, CachedDataSourceConfiguration? cachedConfig)
    {
        var propertyName = property.Name;
        var typeName = property.Type.ToDisplayString();
        
        // Check if we have cached templates for this property
        var template = cachedConfig?.Templates?.ContainsKey(propertyName) == true 
            ? cachedConfig.Templates[propertyName] 
            : null;
        
        if (template != null && template.Length > 0)
        {
            return $"{propertyName} = {GenerateFromTemplate(property.Type, template)}";
        }
        
        return $"{propertyName} = {GenerateTestValue(property.Type, propertyName)}";
    }

    private static string GenerateFromTemplate(ITypeSymbol type, string[] templates)
    {
        var typeName = type.ToDisplayString();
        
        if (typeName == "string")
        {
            var templatesList = string.Join("\", \"", templates);
            return $"new[] {{ \"{templatesList}\" }}[_random.Next({templates.Length})]";
        }
        
        // For non-string types, fall back to regular generation
        return GenerateTestValue(type, "");
    }

    private static string GenerateTestValue(ITypeSymbol type, string propertyName)
    {
        var typeName = type.ToDisplayString();
        
        return typeName switch
        {
            "string" => $"$\"Sample{propertyName}_{{_random.Next(1, 1000)}}\"",
            "int" => "_random.Next(1, 100)",
            "long" => "_random.NextInt64(1, 1000)",
            "decimal" => "((decimal)_random.NextDouble() * 1000)",
            "double" => "_random.NextDouble() * 1000",
            "float" => "((float)_random.NextDouble() * 1000)",
            "bool" => "_random.Next(0, 2) == 1",
            "System.Guid" => "Guid.NewGuid()",
            "System.DateTime" => "DateTime.Now.AddDays(_random.Next(-365, 365))",
            "System.DateTime?" => "_random.Next(0, 2) == 1 ? DateTime.Now.AddDays(_random.Next(-365, 365)) : null",
            _ when type.TypeKind == TypeKind.Enum => GenerateEnumValue(type),
            _ when typeName.EndsWith("?") => "null",
            _ => "default"
        };
    }

    private static string GenerateEnumValue(ITypeSymbol enumType)
    {
        IFieldSymbol[] enumMembers = [..
            enumType.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(f => f.IsStatic && f.HasConstantValue) ];
               
        if (enumMembers.Length == 0)
            return "default";

        return $"({enumType.ToDisplayString()})_random.Next(0, {enumMembers.Length})";
    }

    private static T? GetAttributeValue<T>(AttributeData attributeData, string propertyName)
    {
        var namedArgument = attributeData.NamedArguments
            .FirstOrDefault(na => na.Key == propertyName);

        if (namedArgument.Key == propertyName && namedArgument.Value.Value is T value)
            return value;

        return default;
    }

    private const string DataSourceAttributeSource = """
        // <auto-generated/>
        #nullable enable

        using System;

        namespace Stage3.BasicCaching.Attributes;

        /// <summary>
        /// Marks a class for data source generation in Stage 3
        /// Stage 3: Basic caching with improved performance
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public sealed class DataSourceAttribute : Attribute
        {
            /// <summary>
            /// The name of the entity being generated
            /// </summary>
            public string? EntityName { get; set; }

            /// <summary>
            /// Default number of items to generate in collections
            /// </summary>
            public int Count { get; set; } = 10;

            /// <summary>
            /// Optional reference to external configuration file
            /// </summary>
            public string? ConfigurationFile { get; set; }
        }
        """;
}
