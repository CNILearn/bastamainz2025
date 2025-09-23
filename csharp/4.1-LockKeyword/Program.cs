using LockKeywordSample.Examples;
using LockKeywordSample.Benchmarks;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("🔐 C# Lock Keyword Sample - .NET 9");
Console.WriteLine("==========================================");
Console.WriteLine();

Console.WriteLine("📖 About Lock Keyword in .NET 9");
Console.WriteLine("---------------------------------");
Console.WriteLine(".NET 9 introduces a new Lock type that can be used with the lock keyword:");
Console.WriteLine("• Traditional lock: Uses Monitor class internally (object-based locking)");
Console.WriteLine("• New Lock type: Optimized lock implementation designed for better performance");
Console.WriteLine("• Both support the same lock keyword syntax");
Console.WriteLine("• Both provide thread-safe synchronization mechanisms");
Console.WriteLine();
Console.WriteLine("Key differences:");
Console.WriteLine("• Lock type may offer better performance in high-contention scenarios");
Console.WriteLine("• Lock type is designed with modern threading patterns in mind");
Console.WriteLine("• Traditional lock maintains backward compatibility");
Console.WriteLine("• Both support re-entrant locking");
Console.WriteLine();

// Run the main examples
await DemonstrateAllExamples();

// Ask user if they want to run benchmarks
Console.WriteLine("🏁 Performance Benchmarks");
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
    Console.WriteLine("\\n⏩ Skipping benchmarks. Run with --benchmark argument to include them.");
    ShowBenchmarkSummary();
}

Console.WriteLine("\\n✅ C# lock keyword demonstration completed!");
Console.WriteLine();
Console.WriteLine("🔍 Key Takeaways:");
Console.WriteLine("• Both traditional lock and Lock type provide thread-safe synchronization");
Console.WriteLine("• Lock type may offer performance benefits in high-contention scenarios");
Console.WriteLine("• Traditional lock remains fully supported and widely compatible");
Console.WriteLine("• Choice between them depends on specific performance requirements");
Console.WriteLine("• Both use the same familiar lock keyword syntax");

static async Task DemonstrateAllExamples()
{
    try
    {
        Console.WriteLine("🚀 Lock Keyword Demonstrations");
        Console.WriteLine("==============================");
        Console.WriteLine();

        await LockExamples.DemonstrateBasicLocking();
        await LockExamples.DemonstrateMultiThreadedLocking();
        await LockExamples.DemonstrateFinancialOperations();
        await LockExamples.DemonstrateProducerConsumer();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error running examples: {ex.Message}");
        Console.WriteLine("The sample is designed for .NET 10 RC 1 with the new Lock type.");
    }
}

static void ShowBenchmarkSummary()
{
    Console.WriteLine("\\n📊 Expected Benchmark Results Summary:");
    Console.WriteLine("=====================================");
    Console.WriteLine();
    Console.WriteLine("Based on typical performance characteristics:");
    Console.WriteLine();
    Console.WriteLine("🔄 Single-threaded Operations:");
    Console.WriteLine("  • Traditional lock: Baseline performance, well-optimized");
    Console.WriteLine("  • Lock type: Similar or slightly better performance");
    Console.WriteLine("  • Memory: Both approaches have similar allocation patterns");
    Console.WriteLine();
    Console.WriteLine("🧵 Multi-threaded Contention:");
    Console.WriteLine("  • Traditional lock: Good performance, proven in production");
    Console.WriteLine("  • Lock type: Potentially better performance under high contention");
    Console.WriteLine("  • Scalability: Lock type may scale better with more threads");
    Console.WriteLine();
    Console.WriteLine("💰 Financial Operations:");
    Console.WriteLine("  • Both provide equivalent thread safety guarantees");
    Console.WriteLine("  • Performance differences minimal for business logic");
    Console.WriteLine("  • Lock type may show benefits in high-frequency trading scenarios");
    Console.WriteLine();
    Console.WriteLine("🏭 Producer-Consumer Patterns:");
    Console.WriteLine("  • Traditional lock: Reliable performance for most scenarios");
    Console.WriteLine("  • Lock type: May excel in high-throughput scenarios");
    Console.WriteLine("  • Both support complex concurrent patterns effectively");
    Console.WriteLine();
    Console.WriteLine("🎯 Recommendations:");
    Console.WriteLine("  • Use traditional lock for general-purpose scenarios");
    Console.WriteLine("  • Consider Lock type for high-performance, high-contention scenarios");
    Console.WriteLine("  • Both provide the same thread safety guarantees");
    Console.WriteLine("  • Performance testing is recommended for critical applications");
}