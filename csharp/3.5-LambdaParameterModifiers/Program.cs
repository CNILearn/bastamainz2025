using LambdaParameterModifiers.Examples;
using LambdaParameterModifiers.Benchmarks;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("🚀 C# 14 Lambda Parameter Modifiers Demo - .NET 10 RC 1");
Console.WriteLine("=========================================================");
Console.WriteLine();

Console.WriteLine("📖 About C# 14 Lambda Parameter Modifiers");
Console.WriteLine("------------------------------------------");
Console.WriteLine("C# 14 introduces parameter modifiers for lambda expressions:");
Console.WriteLine("• 'ref' modifier: Allows lambdas to modify parameters in-place");
Console.WriteLine("• 'in' modifier: Passes large structures by reference (readonly)");
Console.WriteLine("• 'out' modifier: Enables lambdas to return multiple values efficiently");
Console.WriteLine();
Console.WriteLine("These modifiers are particularly beneficial for:");
Console.WriteLine("• Low-level memory manipulation");
Console.WriteLine("• High-performance computing scenarios");
Console.WriteLine("• Large data structure processing");
Console.WriteLine("• Systems programming with spans and unsafe code");
Console.WriteLine();

// Run the main examples
await DemonstrateAllExamples();

// Ask user if they want to run benchmarks
Console.WriteLine("\n🏁 Performance Benchmarks");
Console.WriteLine("=========================");
Console.WriteLine("Would you like to run performance benchmarks?");
Console.WriteLine("(This will take several minutes to complete)");
Console.Write("Run benchmarks? (y/n): ");

var runBenchmarks = false;
if (args.Length > 0 && args[0].ToLower() == "--benchmark")
{
    runBenchmarks = true;
    Console.WriteLine("y (from command line argument)");
}
else
{
    var input = Console.ReadLine();
    runBenchmarks = input?.StartsWith("y", StringComparison.CurrentCultureIgnoreCase) == true;
}

if (runBenchmarks)
{
    Console.WriteLine();
    BenchmarkRunner.RunBenchmarks();
}
else
{
    Console.WriteLine("\n⏩ Skipping benchmarks. Run with --benchmark argument to include them.");
    ShowBenchmarkSummary();
}

Console.WriteLine("\n✅ C# 14 lambda parameter modifiers demonstration completed!");
Console.WriteLine();
Console.WriteLine("🔍 Key Takeaways:");
Console.WriteLine("• Lambda 'ref' parameters enable efficient in-place modifications");
Console.WriteLine("• Lambda 'in' parameters avoid copying large readonly structures");
Console.WriteLine("• Lambda 'out' parameters provide efficient multi-value returns");
Console.WriteLine("• These modifiers significantly improve performance in memory-intensive scenarios");
Console.WriteLine("• Ideal for systems programming, game development, and high-performance computing");

await Task.Delay(100); // Ensure async method signature

static async Task DemonstrateAllExamples()
{
    try
    {
        // Demonstrate ref modifier
        LambdaParameterModifierExamples.DemonstrateRefModifier();
        await Task.Delay(500);

        // Demonstrate in modifier  
        LambdaParameterModifierExamples.DemonstrateInModifier();
        await Task.Delay(500);

        // Demonstrate out modifier
        LambdaParameterModifierExamples.DemonstrateOutModifier();
        await Task.Delay(500);

        // Demonstrate advanced scenarios
        LambdaParameterModifierExamples.DemonstrateAdvancedScenarios();
        await Task.Delay(500);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ Error during demonstration: {ex.Message}");
        Console.WriteLine("This may be due to .NET version compatibility issues.");
        Console.WriteLine("The sample is designed for .NET 10 RC 1 with C# 14 features.");
    }
}

static void ShowBenchmarkSummary()
{
    Console.WriteLine("\n📊 Expected Benchmark Results Summary:");
    Console.WriteLine("=====================================");
    Console.WriteLine();
    Console.WriteLine("Based on typical performance characteristics:");
    Console.WriteLine();
    Console.WriteLine("🔄 Vector Operations:");
    Console.WriteLine("  • By Value (baseline): ~100% time, creates object copies");
    Console.WriteLine("  • With 'ref': ~60-80% time, in-place modification");
    Console.WriteLine("  • Memory: 'ref' uses ~50% less allocations");
    Console.WriteLine();
    Console.WriteLine("📖 Large Structure Access:");
    Console.WriteLine("  • By Value: ~100% time, copies 1KB+ structures");
    Console.WriteLine("  • With 'in': ~20-40% time, no copying");
    Console.WriteLine("  • Memory: 'in' eliminates large structure allocations");
    Console.WriteLine();
    Console.WriteLine("📤 Multi-Value Returns:");
    Console.WriteLine("  • Tuples: ~100% time, heap allocations for reference types");
    Console.WriteLine("  • With 'out': ~70-90% time, stack-based returns");
    Console.WriteLine("  • Memory: 'out' reduces garbage collection pressure");
    Console.WriteLine();
    Console.WriteLine("🎯 Span Operations:");
    Console.WriteLine("  • Traditional arrays: ~100% time, bounds checking overhead");
    Console.WriteLine("  • Span with 'ref': ~80-95% time, optimized memory access");
    Console.WriteLine("  • Memory: Spans enable zero-copy operations");
    Console.WriteLine();
    Console.WriteLine("💡 Performance gains are most significant with:");
    Console.WriteLine("  • Large structures (>64 bytes)");
    Console.WriteLine("  • High-frequency operations (millions of calls)");
    Console.WriteLine("  • Memory-constrained environments");
    Console.WriteLine("  • Real-time systems requiring predictable performance");
}