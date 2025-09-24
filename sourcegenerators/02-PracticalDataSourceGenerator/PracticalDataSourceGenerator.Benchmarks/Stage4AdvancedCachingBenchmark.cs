using BenchmarkDotNet.Attributes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace PracticalDataSourceGenerator.Benchmarks;

/// <summary>
/// Comprehensive benchmark comparing all stages: Stage 2 (FileIntegration), Stage 3 (BasicCaching), and Stage 4 (AdvancedCaching)
/// 
/// This benchmark measures the performance improvements gained by the advanced caching strategies
/// introduced in Stage 4 compared to earlier implementations.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class Stage4AdvancedCachingBenchmark
{
    private const string SampleSourceCode = """
        using {namespace}.Attributes;
        using System;

        namespace TestNamespace;

        [DataSource(EntityName = "User", Count = 10)]
        public class User
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public int Age { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        [DataSource(EntityName = "Product", Count = 25)]
        public class Product
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public string Category { get; set; }
            public int StockQuantity { get; set; }
        }

        [DataSource(EntityName = "Order", Count = 50)]
        public class Order
        {
            public string OrderId { get; set; }
            public string Status { get; set; }
            public decimal TotalAmount { get; set; }
            public string ShippingAddress { get; set; }
            public DateTime OrderDate { get; set; }
        }
        """;

    private const string UserConfigJson = """
        {
          "entityType": "User",
          "defaultCount": 25,
          "templates": {
            "Name": [
              "Alice Johnson", "Bob Smith", "Carol Williams", "David Brown", "Emma Davis",
              "Frank Wilson", "Grace Miller", "Henry Garcia", "Ivy Martinez", "Jack Anderson"
            ],
            "Email": [
              "alice.johnson@example.com", "bob.smith@example.com", "carol.williams@example.com",
              "david.brown@example.com", "emma.davis@example.com", "frank.wilson@example.com"
            ]
          },
          "dependencies": ["UserRole", "UserPreferences"],
          "externalDependencies": {
            "userApiEndpoint": "https://api.example.com/users",
            "authenticationService": "OAuth2",
            "region": "us-west-2"
          }
        }
        """;

    private const string ProductConfigJson = """
        {
          "entityType": "Product",
          "defaultCount": 50,
          "templates": {
            "Name": [
              "Ultra Gaming Laptop", "Professional Wireless Mouse", "Mechanical Keyboard Pro",
              "4K Monitor Deluxe", "Noise-Canceling Headphones", "Smartphone Pro Max"
            ],
            "Description": [
              "High-performance device for professionals", "Premium quality with extended warranty",
              "Cutting-edge technology", "User-friendly design", "Industry-leading features"
            ],
            "Category": [
              "Electronics", "Computers", "Accessories", "Mobile", "Gaming", "Office", "Audio"
            ]
          },
          "dependencies": ["ProductCategory", "Supplier", "Inventory"],
          "externalDependencies": {
            "productApiEndpoint": "https://api.example.com/products",
            "inventoryService": "InventoryManagement",
            "pricingEngine": "DynamicPricing",
            "region": "global"
          }
        }
        """;

    private const string OrderConfigJson = """
        {
          "entityType": "Order",
          "defaultCount": 100,
          "templates": {
            "OrderId": [
              "ORD-2024-001", "ORD-2024-002", "ORD-2024-003", "ORD-2024-004", "ORD-2024-005"
            ],
            "Status": [
              "Pending", "Processing", "Shipped", "Delivered", "Cancelled", "Returned"
            ],
            "ShippingAddress": [
              "123 Main St, Anytown, USA", "456 Oak Ave, Springfield, USA", 
              "789 Pine Rd, Riverside, USA", "321 Elm Dr, Lakewood, USA"
            ]
          },
          "dependencies": ["User", "Product", "Payment", "Shipping"],
          "externalDependencies": {
            "orderApiEndpoint": "https://api.example.com/orders",
            "paymentProcessor": "StripePayments",
            "shippingProvider": "FedEx",
            "inventoryService": "InventoryManagement",
            "region": "us-east-1"
          }
        }
        """;

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Single Run")]
    public string Stage2_FileIntegration_SingleRun()
    {
        var source = SampleSourceCode.Replace("{namespace}", "Stage2.FileIntegration");
        var (generatedSource, _) = RunStage2Generator(source,
            ("UserConfiguration.datasource.json", UserConfigJson),
            ("ProductConfiguration.datasource.json", ProductConfigJson),
            ("OrderConfiguration.datasource.json", OrderConfigJson));
        return generatedSource;
    }

    [Benchmark]
    [BenchmarkCategory("Single Run")]
    public string Stage3_BasicCaching_SingleRun()
    {
        var source = SampleSourceCode.Replace("{namespace}", "Stage3.BasicCaching");
        var (generatedSource, _) = RunStage3Generator(source,
            ("UserConfiguration.datasource.json", UserConfigJson),
            ("ProductConfiguration.datasource.json", ProductConfigJson),
            ("OrderConfiguration.datasource.json", OrderConfigJson));
        return generatedSource;
    }

    [Benchmark]
    [BenchmarkCategory("Single Run")]
    public string Stage4_AdvancedCaching_SingleRun()
    {
        var source = SampleSourceCode.Replace("{namespace}", "Stage4.AdvancedCaching");
        var (generatedSource, _) = RunStage4Generator(source,
            ("UserConfiguration.datasource.json", UserConfigJson),
            ("ProductConfiguration.datasource.json", ProductConfigJson),
            ("OrderConfiguration.datasource.json", OrderConfigJson));
        return generatedSource;
    }

    [Benchmark]
    [BenchmarkCategory("Multi Run")]
    public string Stage2_FileIntegration_MultiRun()
    {
        var source = SampleSourceCode.Replace("{namespace}", "Stage2.FileIntegration");
        string result = "";
        for (int i = 0; i < 5; i++)
        {
            var (generatedSource, _) = RunStage2Generator(source,
                ("UserConfiguration.datasource.json", UserConfigJson),
                ("ProductConfiguration.datasource.json", ProductConfigJson),
                ("OrderConfiguration.datasource.json", OrderConfigJson));
            result = generatedSource;
        }
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Multi Run")]
    public string Stage3_BasicCaching_MultiRun()
    {
        var source = SampleSourceCode.Replace("{namespace}", "Stage3.BasicCaching");
        string result = "";
        for (int i = 0; i < 5; i++)
        {
            var (generatedSource, _) = RunStage3Generator(source,
                ("UserConfiguration.datasource.json", UserConfigJson),
                ("ProductConfiguration.datasource.json", ProductConfigJson),
                ("OrderConfiguration.datasource.json", OrderConfigJson));
            result = generatedSource;
        }
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Multi Run")]
    public string Stage4_AdvancedCaching_MultiRun()
    {
        var source = SampleSourceCode.Replace("{namespace}", "Stage4.AdvancedCaching");
        string result = "";
        for (int i = 0; i < 5; i++)
        {
            var (generatedSource, _) = RunStage4Generator(source,
                ("UserConfiguration.datasource.json", UserConfigJson),
                ("ProductConfiguration.datasource.json", ProductConfigJson),
                ("OrderConfiguration.datasource.json", OrderConfigJson));
            result = generatedSource;
        }
        return result;
    }

    [Benchmark]
    [BenchmarkCategory("Large Configuration")]
    public string Stage2_FileIntegration_LargeConfig()
    {
        var source = GenerateLargeSource("Stage2.FileIntegration");
        var configs = GenerateLargeConfigurations();
        var (generatedSource, _) = RunStage2Generator(source, configs);
        return generatedSource;
    }

    [Benchmark]
    [BenchmarkCategory("Large Configuration")]
    public string Stage3_BasicCaching_LargeConfig()
    {
        var source = GenerateLargeSource("Stage3.BasicCaching");
        var configs = GenerateLargeConfigurations();
        var (generatedSource, _) = RunStage3Generator(source, configs);
        return generatedSource;
    }

    [Benchmark]
    [BenchmarkCategory("Large Configuration")]
    public string Stage4_AdvancedCaching_LargeConfig()
    {
        var source = GenerateLargeSource("Stage4.AdvancedCaching");
        var configs = GenerateLargeConfigurations();
        var (generatedSource, _) = RunStage4Generator(source, configs);
        return generatedSource;
    }

    private static string GenerateLargeSource(string namespaceName)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"using {namespaceName}.Attributes;");
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine("namespace TestNamespace;");
        sb.AppendLine();

        // Generate 10 entities
        for (int i = 0; i < 10; i++)
        {
            sb.AppendLine($"[DataSource(EntityName = \"Entity{i}\", Count = {10 + i * 5})]");
            sb.AppendLine($"public class Entity{i}");
            sb.AppendLine("{");
            
            // Each entity has 5 properties
            for (int j = 0; j < 5; j++)
            {
                var propertyType = j switch
                {
                    0 => "string",
                    1 => "int",
                    2 => "decimal",
                    3 => "bool",
                    4 => "DateTime",
                    _ => "string"
                };
                sb.AppendLine($"    public {propertyType} Property{j} {{ get; set; }}");
            }
            
            sb.AppendLine("}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static (string fileName, string content)[] GenerateLargeConfigurations()
    {
        var configs = new List<(string, string)>();
        
        for (int i = 0; i < 10; i++)
        {
            var config = $$"""
            {
              "entityType": "Entity{{i}}",
              "defaultCount": {{20 + i * 10}},
              "templates": {
                "Property0": ["Value{{i}}_0", "Value{{i}}_1", "Value{{i}}_2"],
                "Property1": ["{{i * 100}}", "{{i * 100 + 50}}", "{{i * 100 + 100}}"]
              },
              "dependencies": ["Dependency{{i}}_1", "Dependency{{i}}_2"],
              "externalDependencies": {
                "apiEndpoint": "https://api.example.com/entity{{i}}",
                "service": "Service{{i}}",
                "region": "region-{{i}}"
              }
            }
            """;
            
            configs.Add(($"Entity{i}Configuration.datasource.json", config));
        }
        
        return configs.ToArray();
    }

    private static (string generatedSource, ImmutableArray<Diagnostic> diagnostics) RunStage2Generator(
        string source,
        params (string fileName, string content)[] additionalFiles)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
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
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
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
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
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

    private class TestAdditionalText(string path, string content) : AdditionalText
    {
        private readonly string _content = content;
        public override string Path { get; } = path;
        public override SourceText? GetText(CancellationToken cancellationToken = default) =>
            SourceText.From(_content, Encoding.UTF8);
    }
}