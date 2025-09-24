using BenchmarkDotNet.Attributes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

using System.Collections.Immutable;
using System.Text;

namespace PracticalDataSourceGenerator.Benchmarks;

/// <summary>
/// Comprehensive benchmark comparing all stages: Stage 2 through Stage 5
/// 
/// This benchmark measures the performance improvements gained by the optimization strategies
/// introduced in Stage 5 compared to all previous implementations.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class Stage5OptimizedBenchmark
{
    private const string SampleSourceCode = """
        using {namespace}.Attributes;
        using System;

        namespace BenchmarkTest;

        [DataSource(EntityName = "User", Count = 25)]
        public class User
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public int Age { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        [DataSource(EntityName = "Product", Count = 25)]
        public class Product
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string Category { get; set; }
            public int StockQuantity { get; set; }
        }

        [DataSource(EntityName = "Order", Count = 50)]
        public class Order
        {
            public string Id { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public decimal Total { get; set; }
            public DateTime OrderDate { get; set; }
            public string CustomerId { get; set; } = string.Empty;
        }
        """;

    private static (string generatedSource, ImmutableArray<Diagnostic> diagnostics) RunStage2Generator(
        string source,
        params (string fileName, string content)[] additionalFiles)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source.Replace("{namespace}", "Stage2.FileIntegration"));

        PortableExecutableReference[] references = [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
        ];

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var additionalTexts = additionalFiles
            .Select(f => new TestAdditionalText(f.fileName, f.content))
            .ToImmutableArray<AdditionalText>();

        var generator = new Stage2.FileIntegration.FileIntegrationDataSourceGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create([generator.AsSourceGenerator()], additionalTexts);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var _, out var _);
        var result = driver.GetRunResult();
        var combinedSource = string.Join("\n\n", result.Results[0].GeneratedSources.Select(s => s.SourceText.ToString()));
        return (combinedSource, result.Diagnostics);
    }

    private static (string generatedSource, ImmutableArray<Diagnostic> diagnostics) RunStage3Generator(
        string source,
        params (string fileName, string content)[] additionalFiles)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source.Replace("{namespace}", "Stage3.BasicCaching"));

        PortableExecutableReference[] references = [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
        ];

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var additionalTexts = additionalFiles
            .Select(f => new TestAdditionalText(f.fileName, f.content))
            .ToImmutableArray<AdditionalText>();

        var generator = new Stage3.BasicCaching.BasicCachingDataSourceGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create([generator.AsSourceGenerator()], additionalTexts);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var _, out var _);
        var result = driver.GetRunResult();
        var combinedSource = string.Join("\n\n", result.Results[0].GeneratedSources.Select(s => s.SourceText.ToString()));
        return (combinedSource, result.Diagnostics);
    }

    private static (string generatedSource, ImmutableArray<Diagnostic> diagnostics) RunStage4Generator(
        string source,
        params (string fileName, string content)[] additionalFiles)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source.Replace("{namespace}", "Stage4.AdvancedCaching"));

        PortableExecutableReference[] references = [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
        ];

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var additionalTexts = additionalFiles
            .Select(f => new TestAdditionalText(f.fileName, f.content))
            .ToImmutableArray<AdditionalText>();

        var generator = new Stage4.AdvancedCaching.AdvancedCachingDataSourceGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create([generator.AsSourceGenerator()], additionalTexts);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var _, out var _);
        var result = driver.GetRunResult();
        var combinedSource = string.Join("\n\n", result.Results[0].GeneratedSources.Select(s => s.SourceText.ToString()));
        return (combinedSource, result.Diagnostics);
    }

    private static (string generatedSource, ImmutableArray<Diagnostic> diagnostics) RunStage5Generator(
        string source,
        params (string fileName, string content)[] additionalFiles)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source.Replace("{namespace}", "Stage5.Optimized"));

        PortableExecutableReference[] references = [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
        ];

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var additionalTexts = additionalFiles
            .Select(f => new TestAdditionalText(f.fileName, f.content))
            .ToImmutableArray<AdditionalText>();

        var generator = new Stage5.Optimized.OptimizedDataSourceGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create([generator.AsSourceGenerator()], additionalTexts);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var _, out var _);
        var result = driver.GetRunResult();
        var combinedSource = string.Join("\n\n", result.Results[0].GeneratedSources.Select(s => s.SourceText.ToString()));
        return (combinedSource, result.Diagnostics);
    }

    private (string fileName, string content)[] CreateConfigurationFiles()
    {
        var userConfig = """
            {
              "entityType": "User",
              "defaultCount": 25,
              "templates": {
                "Name": ["Alice Johnson", "Bob Smith", "Carol Williams"],
                "Email": ["alice.johnson@example.com", "bob.smith@example.com", "carol.williams@example.com"]
              },
              "dependencies": ["UserRole", "UserPreferences"],
              "externalDependencies": {
                "userApiEndpoint": "https://api.example.com/users",
                "authenticationService": "OAuth2",
                "region": "us-west-2"
              }
            }
            """;

        var productConfig = """
            {
              "entityType": "Product",
              "defaultCount": 25,
              "templates": {
                "Name": ["Ultra Gaming Laptop", "Professional Webcam", "Wireless Keyboard"],
                "Description": ["High-performance gaming laptop", "4K professional webcam", "Wireless mechanical keyboard"],
                "Category": ["Electronics", "Accessories", "Computer Hardware"]
              },
              "dependencies": ["ProductCategory", "ProductInventory", "ProductPricing"],
              "externalDependencies": {
                "inventoryApiEndpoint": "https://api.example.com/inventory",
                "pricingService": "DynamicPricing",
                "region": "us-west-2",
                "currency": "USD"
              }
            }
            """;

        var orderConfig = """
            {
              "entityType": "Order",
              "defaultCount": 50,
              "templates": {
                "Id": ["ORD-2024-001", "ORD-2024-002", "ORD-2024-003"],
                "Status": ["Pending", "Processing", "Shipped", "Delivered"]
              },
              "dependencies": ["OrderItem", "OrderPayment", "OrderShipping", "OrderTracking"],
              "externalDependencies": {
                "paymentApiEndpoint": "https://api.example.com/payments",
                "shippingService": "FastShipping",
                "region": "us-west-2",
                "currency": "USD",
                "trackingService": "GlobalTracking"
              }
            }
            """;

        return [
            ("User.datasource.json", userConfig),
            ("Product.datasource.json", productConfig),
            ("Order.datasource.json", orderConfig)
        ];
    }

    // Single Run Benchmarks - measure individual generation performance
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("SingleRun")]
    public string Stage2_SingleRun()
    {
        var configFiles = CreateConfigurationFiles();
        var (source, _) = RunStage2Generator(SampleSourceCode, configFiles);
        return source;
    }

    [Benchmark]
    [BenchmarkCategory("SingleRun")]
    public string Stage3_SingleRun()
    {
        var configFiles = CreateConfigurationFiles();
        var (source, _) = RunStage3Generator(SampleSourceCode, configFiles);
        return source;
    }

    [Benchmark]
    [BenchmarkCategory("SingleRun")]
    public string Stage4_SingleRun()
    {
        var configFiles = CreateConfigurationFiles();
        var (source, _) = RunStage4Generator(SampleSourceCode, configFiles);
        return source;
    }

    [Benchmark]
    [BenchmarkCategory("SingleRun")]
    public string Stage5_SingleRun()
    {
        var configFiles = CreateConfigurationFiles();
        var (source, _) = RunStage5Generator(SampleSourceCode, configFiles);
        return source;
    }

    // Multi-Run Benchmarks - measure cache benefits over multiple runs
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("MultiRun")]
    public string Stage2_MultiRun()
    {
        var configFiles = CreateConfigurationFiles();
        var results = new List<string>();

        // Run multiple times to test caching efficiency
        for (int i = 0; i < 5; i++)
        {
            var (source, _) = RunStage2Generator(SampleSourceCode, configFiles);
            results.Add(source);
        }

        return string.Join("|", results.Select(r => r.Length.ToString()));
    }

    [Benchmark]
    [BenchmarkCategory("MultiRun")]
    public string Stage3_MultiRun()
    {
        var configFiles = CreateConfigurationFiles();
        var results = new List<string>();

        for (int i = 0; i < 5; i++)
        {
            var (source, _) = RunStage3Generator(SampleSourceCode, configFiles);
            results.Add(source);
        }

        return string.Join("|", results.Select(r => r.Length.ToString()));
    }

    [Benchmark]
    [BenchmarkCategory("MultiRun")]
    public string Stage4_MultiRun()
    {
        var configFiles = CreateConfigurationFiles();
        var results = new List<string>();

        for (int i = 0; i < 5; i++)
        {
            var (source, _) = RunStage4Generator(SampleSourceCode, configFiles);
            results.Add(source);
        }

        return string.Join("|", results.Select(r => r.Length.ToString()));
    }

    [Benchmark]
    [BenchmarkCategory("MultiRun")]
    public string Stage5_MultiRun()
    {
        var configFiles = CreateConfigurationFiles();
        var results = new List<string>();

        for (int i = 0; i < 5; i++)
        {
            var (source, _) = RunStage5Generator(SampleSourceCode, configFiles);
            results.Add(source);
        }

        return string.Join("|", results.Select(r => r.Length.ToString()));
    }

    // Large-Scale Benchmarks - test with complex scenarios
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("LargeScale")]
    public string Stage2_LargeScale()
    {
        var configFiles = CreateLargeConfigFiles();
        var (source, _) = RunStage2Generator(SampleSourceCode, configFiles);
        return source;
    }

    [Benchmark]
    [BenchmarkCategory("LargeScale")]
    public string Stage3_LargeScale()
    {
        var configFiles = CreateLargeConfigFiles();
        var (source, _) = RunStage3Generator(SampleSourceCode, configFiles);
        return source;
    }

    [Benchmark]
    [BenchmarkCategory("LargeScale")]
    public string Stage4_LargeScale()
    {
        var configFiles = CreateLargeConfigFiles();
        var (source, _) = RunStage4Generator(SampleSourceCode, configFiles);
        return source;
    }

    [Benchmark]
    [BenchmarkCategory("LargeScale")]
    public string Stage5_LargeScale()
    {
        var configFiles = CreateLargeConfigFiles();
        var (source, _) = RunStage5Generator(SampleSourceCode, configFiles);
        return source;
    }

    // Stress Test Benchmarks - high-frequency operations
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("StressTest")]
    public string Stage2_StressTest()
    {
        var configFiles = CreateConfigurationFiles();
        var results = new List<string>();

        // High-frequency generation to test performance under stress
        for (int i = 0; i < 20; i++)
        {
            var (source, _) = RunStage2Generator(SampleSourceCode, configFiles);
            results.Add(source);
        }

        return results.Count.ToString();
    }

    [Benchmark]
    [BenchmarkCategory("StressTest")]
    public string Stage3_StressTest()
    {
        var configFiles = CreateConfigurationFiles();
        var results = new List<string>();

        for (int i = 0; i < 20; i++)
        {
            var (source, _) = RunStage3Generator(SampleSourceCode, configFiles);
            results.Add(source);
        }

        return results.Count.ToString();
    }

    [Benchmark]
    [BenchmarkCategory("StressTest")]
    public string Stage4_StressTest()
    {
        var configFiles = CreateConfigurationFiles();
        var results = new List<string>();

        for (int i = 0; i < 20; i++)
        {
            var (source, _) = RunStage4Generator(SampleSourceCode, configFiles);
            results.Add(source);
        }

        return results.Count.ToString();
    }

    [Benchmark]
    [BenchmarkCategory("StressTest")]
    public string Stage5_StressTest()
    {
        var configFiles = CreateConfigurationFiles();
        var results = new List<string>();

        for (int i = 0; i < 20; i++)
        {
            var (source, _) = RunStage5Generator(SampleSourceCode, configFiles);
            results.Add(source);
        }

        return results.Count.ToString();
    }

    private (string fileName, string content)[] CreateLargeConfigFiles()
    {
        // Create larger JSON files to test the performance impact of scale
        var largeUserConfig = """
            {
              "entityType": "User",
              "defaultCount": 200,
              "templates": {
                "Name": [
          """ + string.Join(",\n          ", Enumerable.Range(1, 200).Select(i => $"\"User{i:D3} Name\"")) + """
                ],
                "Email": [
          """ + string.Join(",\n          ", Enumerable.Range(1, 200).Select(i => $"\"user{i:D3}@example{i % 50}.com\"")) + """
                ]
              },
              "dependencies": ["UserRole", "UserPreferences", "UserProfile", "UserSettings"],
              "externalDependencies": {
                "userApiEndpoint": "https://api.example.com/users",
                "authenticationService": "OAuth2",
                "region": "us-west-2",
                "database": "UserDB",
                "cacheService": "Redis"
              }
            }
            """;

        var largeProductConfig = """
            {
              "entityType": "Product",
              "defaultCount": 300,
              "templates": {
                "Name": [
          """ + string.Join(",\n          ", Enumerable.Range(1, 300).Select(i => $"\"Product{i:D3}\"")) + """
                ],
                "Description": [
          """ + string.Join(",\n          ", Enumerable.Range(1, 100).Select(i => $"\"High-quality product {i} with advanced features\"")) + """
                ],
                "Category": [
          """ + string.Join(",\n          ", Enumerable.Range(1, 50).Select(i => $"\"Category{i:D2}\"")) + """
                ]
              },
              "dependencies": ["ProductCategory", "ProductInventory", "ProductPricing", "ProductImages", "ProductReviews"],
              "externalDependencies": {
                "inventoryApiEndpoint": "https://api.example.com/inventory",
                "pricingService": "DynamicPricing",
                "region": "us-west-2",
                "currency": "USD",
                "imageService": "CloudImages",
                "reviewService": "ReviewAPI"
              }
            }
            """;

        return [
            ("User.datasource.json", largeUserConfig),
            ("Product.datasource.json", largeProductConfig)
        ];
    }
}

internal class TestAdditionalText(string path, string text) : AdditionalText
{
    public override string Path { get; } = path;

    public override SourceText? GetText(CancellationToken cancellationToken = default)
    {
        return SourceText.From(text, Encoding.UTF8);
    }
}