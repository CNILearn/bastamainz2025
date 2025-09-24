using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace PracticalDataSourceGenerator.Benchmarks;

/// <summary>
/// Benchmark comparing Stage 2 (FileIntegration) vs Stage 3 (BasicCaching) performance
/// 
/// This benchmark measures the performance improvements gained by the caching strategies
/// introduced in Stage 3 compared to the file-based approach in Stage 2.
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class Stage2VsStage3Benchmark
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

        [DataSource(EntityName = "Product", Count = 15)]
        public class Product
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public string Category { get; set; }
            public int StockQuantity { get; set; }
        }

        [DataSource(EntityName = "Order", Count = 20)]
        public class Order
        {
            public string OrderId { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string CustomerName { get; set; }
            public string ShippingAddress { get; set; }
        }
        """;

    private const string UserConfigJson = """
        {
          "entityType": "User",
          "defaultCount": 25,
          "templates": {
            "Name": [
              "Alice Johnson", "Bob Smith", "Carol Williams", "David Brown", "Emma Davis",
              "Frank Miller", "Grace Wilson", "Henry Moore", "Ivy Taylor", "Jack Anderson",
              "Kate Thompson", "Liam Garcia", "Mia Rodriguez", "Noah Martinez", "Olivia Lee"
            ],
            "Email": [
              "alice@example.com", "bob@company.org", "carol@business.net", 
              "david@enterprise.com", "emma@startup.io", "frank@corporation.com",
              "grace@services.org", "henry@solutions.net", "ivy@consulting.com", 
              "jack@ventures.io", "kate@tech.com", "liam@digital.org"
            ]
          }
        }
        """;

    private const string ProductConfigJson = """
        {
          "entityType": "Product",
          "defaultCount": 30,
          "templates": {
            "Name": [
              "Laptop Pro", "Smartphone X", "Tablet Ultra", "Headphones Elite", 
              "Monitor 4K", "Keyboard Mechanical", "Mouse Gaming", "Webcam HD",
              "Speaker Bluetooth", "Charger Wireless", "Cable USB-C", "Stand Adjustable"
            ],
            "Description": [
              "High-performance device", "Professional grade equipment", "Consumer electronics",
              "Business solution", "Gaming accessory", "Productivity tool"
            ],
            "Category": [
              "Electronics", "Computers", "Accessories", "Mobile", "Gaming", "Office"
            ]
          }
        }
        """;

    private const string OrderConfigJson = """
        {
          "entityType": "Order",
          "defaultCount": 50,
          "templates": {
            "OrderId": [
              "ORD-2024-001", "ORD-2024-002", "ORD-2024-003", "ORD-2024-004",
              "ORD-2024-005", "ORD-2024-006", "ORD-2024-007", "ORD-2024-008"
            ],
            "CustomerName": [
              "ACME Corporation", "TechStart Inc", "Global Solutions Ltd", 
              "Innovation Partners", "Digital Dynamics", "Future Systems"
            ],
            "ShippingAddress": [
              "123 Main St, City, State 12345", "456 Oak Ave, Town, State 67890",
              "789 Pine Rd, Village, State 11111", "321 Elm St, Metro, State 22222"
            ]
          }
        }
        """;

    private readonly (string fileName, string content)[] _configFiles = [
        ("User.datasource.json", UserConfigJson),
        ("Product.datasource.json", ProductConfigJson), 
        ("Order.datasource.json", OrderConfigJson)
    ];

    // Number of simulated incremental edit iterations
    private const int IncrementalEditIterations = 15;

    // Use large config files for incremental edits to amplify parsing cost
    private readonly (string fileName, string content)[] _largeConfigFiles;

    public Stage2VsStage3Benchmark()
    {
        _largeConfigFiles = CreateLargeConfigFiles();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("SingleRun")]
    public string Stage2_FileIntegration_SingleRun()
    {
        return RunStage2Generator(SampleSourceCode.Replace("{namespace}", "Stage2.FileIntegration"), _configFiles);
    }

    [Benchmark]
    [BenchmarkCategory("SingleRun")]
    public string Stage3_BasicCaching_SingleRun()
    {
        return RunStage3Generator(SampleSourceCode.Replace("{namespace}", "Stage3.BasicCaching"), _configFiles);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("MultipleRuns")]
    public string Stage2_FileIntegration_MultipleRuns()
    {
        var results = new List<string>();
        
        // Simulate multiple builds by running the generator multiple times
        // This shows how Stage 2 re-parses files every time
        for (int i = 0; i < 5; i++)
        {
            results.Add(RunStage2Generator(SampleSourceCode.Replace("{namespace}", "Stage2.FileIntegration"), _configFiles));
        }
        
        return string.Join("", results);
    }

    [Benchmark]
    [BenchmarkCategory("MultipleRuns")]
    public string Stage3_BasicCaching_MultipleRuns()
    {
        var results = new List<string>();
        
        // Simulate multiple builds by running the generator multiple times
        // This shows how Stage 3 benefits from caching on subsequent runs
        for (int i = 0; i < 5; i++)
        {
            results.Add(RunStage3Generator(SampleSourceCode.Replace("{namespace}", "Stage3.BasicCaching"), _configFiles));
        }
        
        return string.Join("", results);
    }

    // New benchmark category explicitly simulating incremental edit cycles.
    // Each iteration changes source (adds a comment) forcing regeneration while additional files stay constant.
    // Stage 2 re-parses JSON each time inside Execute; Stage 3 reuses cached parsed configurations.
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("IncrementalEdits")]
    public int Stage2_FileIntegration_IncrementalEdits()
    {
        return RunIncrementalEditScenario(stage: 2, useLargeFiles: true);
    }

    [Benchmark]
    [BenchmarkCategory("IncrementalEdits")]
    public int Stage3_BasicCaching_IncrementalEdits()
    {
        return RunIncrementalEditScenario(stage: 3, useLargeFiles: true);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("LargeFiles")]
    public string Stage2_FileIntegration_LargeConfigFiles()
    {
        var largeConfigFiles = CreateLargeConfigFiles();
        return RunStage2Generator(SampleSourceCode.Replace("{namespace}", "Stage2.FileIntegration"), largeConfigFiles);
    }

    [Benchmark]
    [BenchmarkCategory("LargeFiles")]
    public string Stage3_BasicCaching_LargeConfigFiles()
    {
        var largeConfigFiles = CreateLargeConfigFiles();
        return RunStage3Generator(SampleSourceCode.Replace("{namespace}", "Stage3.BasicCaching"), largeConfigFiles);
    }

    private int RunIncrementalEditScenario(int stage, bool useLargeFiles = false)
    {
        string ns = stage == 2 ? "Stage2.FileIntegration" : "Stage3.BasicCaching";
        string baseSource = SampleSourceCode.Replace("{namespace}", ns);

        var configFiles = useLargeFiles ? _largeConfigFiles : _configFiles;

        // Initial compilation
        var syntaxTree = CSharpSyntaxTree.ParseText(baseSource);
        
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

        var additionalTexts = configFiles
            .Select(f => new TestAdditionalText(f.fileName, f.content))
            .ToImmutableArray<AdditionalText>();

        ISourceGenerator generator = stage == 2
            ? new Stage2.FileIntegration.FileIntegrationDataSourceGenerator().AsSourceGenerator()
            : new Stage3.BasicCaching.BasicCachingDataSourceGenerator().AsSourceGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [generator],
            additionalTexts: additionalTexts);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var _);

        int totalLength = 0;
        for (int i = 0; i < IncrementalEditIterations; i++)
        {
            // Simulate an incremental edit (whitespace/comment change)
            string editedSource = baseSource + "\n// incremental edit " + i.ToString();
            var editedTree = CSharpSyntaxTree.ParseText(editedSource);
            var newCompilation = CSharpCompilation.Create(
                "TestAssembly",
                [editedTree],
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            driver = driver.RunGeneratorsAndUpdateCompilation(newCompilation, out updatedCompilation, out var _);
            var runResult = driver.GetRunResult();
            var generatedSources = runResult.Results[0].GeneratedSources;
            totalLength += generatedSources.Sum(s => s.SourceText.Length);
        }
        return totalLength;
    }

    private string RunStage2Generator(string sourceCode, (string fileName, string content)[] configFiles)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        
        PortableExecutableReference[] references = [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
        ];

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var additionalTexts = configFiles
            .Select(f => new TestAdditionalText(f.fileName, f.content))
            .ToImmutableArray<AdditionalText>();

        var generator = new Stage2.FileIntegration.FileIntegrationDataSourceGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [generator.AsSourceGenerator()],
            additionalTexts: additionalTexts);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var _, out var _);
        var result = driver.GetRunResult();
        var generatedSources = result.Results[0].GeneratedSources;
        
        return string.Join("\n", generatedSources.Select(s => s.SourceText.ToString()));
    }

    private string RunStage3Generator(string sourceCode, (string fileName, string content)[] configFiles)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        
        PortableExecutableReference[] references = [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
        ];

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var additionalTexts = configFiles
            .Select(f => new TestAdditionalText(f.fileName, f.content))
            .ToImmutableArray<AdditionalText>();

        var generator = new Stage3.BasicCaching.BasicCachingDataSourceGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [generator.AsSourceGenerator()],
            additionalTexts: additionalTexts);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var _, out var _);
        var result = driver.GetRunResult();
        var generatedSources = result.Results[0].GeneratedSources;
        
        return string.Join("\n", generatedSources.Select(s => s.SourceText.ToString()));
    }

    private (string fileName, string content)[] CreateLargeConfigFiles()
    {
        // Create larger JSON files to test the performance impact of file size
        var largeUserConfig = """
            {
              "entityType": "User",
              "defaultCount": 100,
              "templates": {
                "Name": [
        """ + string.Join(",\n          ", Enumerable.Range(1, 100).Select(i => $"\"User{i:D3} Name\"")) + """
                ],
                "Email": [
        """ + string.Join(",\n          ", Enumerable.Range(1, 100).Select(i => $"\"user{i:D3}@example{i % 10}.com\"")) + """
                ]
              }
            }
            """;

        var largeProductConfig = """
            {
              "entityType": "Product",
              "defaultCount": 200,
              "templates": {
                "Name": [
        """ + string.Join(",\n          ", Enumerable.Range(1, 150).Select(i => $"\"Product{i:D3}\"")) + """
                ],
                "Description": [
        """ + string.Join(",\n          ", Enumerable.Range(1, 50).Select(i => $"\"High-quality product {i} with advanced features\"")) + """
                ],
                "Category": [
        """ + string.Join(",\n          ", Enumerable.Range(1, 20).Select(i => $"\"Category{i:D2}\"")) + """
                ]
              }
            }
            """;

        return [
            ("User.datasource.json", largeUserConfig),
            ("Product.datasource.json", largeProductConfig)
        ];
    }

    private class TestAdditionalText(string path, string content) : AdditionalText
    {
        private readonly string _content = content;

        public override string Path { get; } = path;

        public override SourceText? GetText(System.Threading.CancellationToken cancellationToken = default)
        {
            return SourceText.From(_content, Encoding.UTF8);
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("PracticalDataSourceGenerator Benchmarks");
        Console.WriteLine("=======================================");
        Console.WriteLine();
        Console.WriteLine("Available benchmarks:");
        Console.WriteLine("1. Stage 2 vs Stage 3 comparison");
        Console.WriteLine("2. Stage 4 Advanced Caching (all stages comparison)");
        Console.WriteLine("3. Stage 5 Optimized (comprehensive performance comparison)");
        Console.WriteLine();
        Console.WriteLine("New category: IncrementalEdits (simulated edit cycles)");
        Console.WriteLine();

        if (args.Length > 0)
        {
            switch (args[0].ToLower())
            {
                case "stage4":
                    Console.WriteLine("Running Stage 4 Advanced Caching Benchmarks...");
                    BenchmarkRunner.Run<Stage4AdvancedCachingBenchmark>();
                    break;
                case "stage5":
                    Console.WriteLine("Running Stage 5 Optimized Benchmarks...");
                    BenchmarkRunner.Run<Stage5OptimizedBenchmark>();
                    break;
                case "all":
                    Console.WriteLine("Running all benchmarks...");
                    BenchmarkRunner.Run<Stage2VsStage3Benchmark>();
                    BenchmarkRunner.Run<Stage4AdvancedCachingBenchmark>();
                    BenchmarkRunner.Run<Stage5OptimizedBenchmark>();
                    break;
                default:
                    Console.WriteLine("Running Stage 2 vs Stage 3 Benchmarks...");
                    BenchmarkRunner.Run<Stage2VsStage3Benchmark>();
                    break;
            }
        }
        else
        {
            Console.WriteLine("Running Stage 5 Optimized Benchmarks (default)...");
            Console.WriteLine("Use 'dotnet run stage4' for Stage 4 benchmarks.");
            Console.WriteLine("Use 'dotnet run all' to run all benchmarks.");
            BenchmarkRunner.Run<Stage5OptimizedBenchmark>();
        }
    }
}