using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

using Stage3.BasicCaching;
using Stage6.ForAttributeWithMetadataName;

using System.Collections.Immutable;
using System.Text;

namespace PracticalDataSourceGenerator.Benchmarks;

/// <summary>
/// Benchmark comparing Stage 3 (BasicCaching) vs Stage 6 (ForAttributeWithMetadataName) performance
/// 
/// This benchmark measures the performance improvements gained by using ForAttributeWithMetadataName
/// for attribute detection compared to the manual GetAttributes() approach in Stage 3.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class Stage3VsStage6Benchmark
{
    private const string SampleSourceCodeStage3 = """
        using Stage3.BasicCaching.Attributes;
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

        [DataSource(EntityName = "Product", Count = 15)]
        public class Product
        {
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string Description { get; set; } = string.Empty;
            public int StockQuantity { get; set; }
            public bool IsAvailable { get; set; }
        }

        [DataSource(EntityName = "Order", Count = 10)]
        public class Order
        {
            public string OrderNumber { get; set; } = string.Empty;
            public DateTime OrderDate { get; set; }
            public decimal Total { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
        }
        """;

    private const string SampleSourceCodeStage6 = """
        using Stage6.ForAttributeWithMetadataName.Attributes;
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

        [DataSource(EntityName = "Product", Count = 15)]
        public class Product
        {
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string Description { get; set; } = string.Empty;
            public int StockQuantity { get; set; }
            public bool IsAvailable { get; set; }
        }

        [DataSource(EntityName = "Order", Count = 10)]
        public class Order
        {
            public string OrderNumber { get; set; } = string.Empty;
            public DateTime OrderDate { get; set; }
            public decimal Total { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
        }
        """;

    private const string UserConfigJson = """
        {
          "entityType": "User",
          "defaultCount": 25,
          "templates": {
            "Name": [
              "Alice Johnson", "Bob Smith", "Carol Davis", "David Wilson", "Eve Brown",
              "Frank Miller", "Grace Lee", "Henry Taylor", "Ivy Chen", "Jack Anderson"
            ],
            "Email": [
              "alice@example.com", "bob@company.org", "carol@tech.net", "david@business.com", "eve@startup.io",
              "frank@enterprise.org", "grace@consulting.com", "henry@solutions.net", "ivy@consulting.com", "jack@services.org"
            ]
          }
        }
        """;

    private const string ProductConfigJson = """
        {
          "entityType": "Product",
          "defaultCount": 15,
          "templates": {
            "Name": [
              "Smart Watch", "Bluetooth Speaker", "Wireless Headphones", "Tablet Computer", "Fitness Tracker",
              "Electric Scooter", "Gaming Mouse", "Mechanical Keyboard", "4K Monitor", "Portable Charger"
            ],
            "Description": [
              "Latest technology mobile device", "High-quality audio experience", "Comfortable wireless listening",
              "Powerful tablet for productivity", "Monitor your health and fitness goals",
              "Eco-friendly urban transportation", "Precision gaming peripheral", "Tactile switches for enhanced typing experience",
              "Crystal clear 4K display", "Portable audio for any occasion"
            ]
          }
        }
        """;

    private readonly (string fileName, string content)[] _configFiles = new[]
    {
        ("User.datasource.json", UserConfigJson),
        ("Product.datasource.json", ProductConfigJson)
    };

    // Single Run Benchmarks - measure individual generation performance
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("SingleRun")]
    public string Stage3_SingleRun()
    {
        var (source, _) = RunStage3Generator(SampleSourceCodeStage3, _configFiles);
        return source.Length.ToString();
    }

    [Benchmark]
    [BenchmarkCategory("SingleRun")]
    public string Stage6_SingleRun()
    {
        var (source, _) = RunStage6Generator(SampleSourceCodeStage6, _configFiles);
        return source.Length.ToString();
    }

    // Multi-Run Benchmarks - measure cache benefits over multiple runs
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("MultiRun")]
    public string Stage3_MultiRun()
    {
        var results = new List<string>();

        // Run multiple times to test caching efficiency
        for (int i = 0; i < 5; i++)
        {
            var (source, _) = RunStage3Generator(SampleSourceCodeStage3, _configFiles);
            results.Add(source);
        }

        return string.Join("|", results.Select(r => r.Length.ToString()));
    }

    [Benchmark]
    [BenchmarkCategory("MultiRun")]
    public string Stage6_MultiRun()
    {
        var results = new List<string>();

        // Run multiple times to test ForAttributeWithMetadataName efficiency
        for (int i = 0; i < 5; i++)
        {
            var (source, _) = RunStage6Generator(SampleSourceCodeStage6, _configFiles);
            results.Add(source);
        }

        return string.Join("|", results.Select(r => r.Length.ToString()));
    }

    // Stress Test Benchmarks - measure performance under load
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("StressTest")]
    public string Stage3_StressTest()
    {
        var results = new List<string>();

        for (int i = 0; i < 20; i++)
        {
            var (source, _) = RunStage3Generator(SampleSourceCodeStage3, _configFiles);
            results.Add(source);
        }

        return results.Count.ToString();
    }

    [Benchmark]
    [BenchmarkCategory("StressTest")]
    public string Stage6_StressTest()
    {
        var results = new List<string>();

        for (int i = 0; i < 20; i++)
        {
            var (source, _) = RunStage6Generator(SampleSourceCodeStage6, _configFiles);
            results.Add(source);
        }

        return results.Count.ToString();
    }

    // Large Payload Benchmarks - test with more complex scenarios
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("LargePayload")]
    public string Stage3_LargePayload()
    {
        var largeSourceCode = GenerateLargeSourceCode("Stage3.BasicCaching.Attributes");
        var (source, _) = RunStage3Generator(largeSourceCode, _configFiles);
        return source.Length.ToString();
    }

    [Benchmark]
    [BenchmarkCategory("LargePayload")]
    public string Stage6_LargePayload()
    {
        var largeSourceCode = GenerateLargeSourceCode("Stage6.ForAttributeWithMetadataName.Attributes");
        var (source, _) = RunStage6Generator(largeSourceCode, _configFiles);
        return source.Length.ToString();
    }

    private static string GenerateLargeSourceCode(string attributeNamespace)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"using {attributeNamespace};");
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine("namespace BenchmarkTest;");
        sb.AppendLine();

        // Generate 50 classes to stress test attribute detection
        for (int i = 0; i < 50; i++)
        {
            sb.AppendLine($"[DataSource(EntityName = \"Entity{i}\", Count = {5 + (i % 10)})]");
            sb.AppendLine($"public class Entity{i}");
            sb.AppendLine("{");
            sb.AppendLine("    public string Name { get; set; } = string.Empty;");
            sb.AppendLine("    public int Id { get; set; }");
            sb.AppendLine("    public bool IsActive { get; set; }");
            sb.AppendLine("    public DateTime CreatedAt { get; set; }");
            sb.AppendLine("}");
            sb.AppendLine();
        }

        return sb.ToString();
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

        var generator = new BasicCachingDataSourceGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [generator.AsSourceGenerator()],
            additionalTexts: additionalTexts);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
        var result = driver.GetRunResult();
        var generatedSources = result.Results[0].GeneratedSources;
        var combinedSource = string.Join("\n\n", generatedSources.Select(s => s.SourceText.ToString()));

        return (combinedSource, result.Diagnostics);
    }

    private static (string generatedSource, ImmutableArray<Diagnostic> diagnostics) RunStage6Generator(
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

        var generator = new ForAttributeWithMetadataNameDataSourceGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [generator.AsSourceGenerator()],
            additionalTexts: additionalTexts);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
        var result = driver.GetRunResult();
        var generatedSources = result.Results[0].GeneratedSources;
        var combinedSource = string.Join("\n\n", generatedSources.Select(s => s.SourceText.ToString()));

        return (combinedSource, result.Diagnostics);
    }
}