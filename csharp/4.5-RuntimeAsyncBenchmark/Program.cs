using BenchmarkDotNet.Running;

using System.Text;

namespace RuntimeAsyncBenchmarks;

/// <summary>
/// .NET 9 vs .NET 10 Runtime Async Performance Benchmark
/// 
/// This benchmark compares async performance across three configurations:
/// 1. .NET 9 (baseline)
/// 2. .NET 10 without Runtime Async
/// 3. .NET 10 with Runtime Async enabled
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("🚀 .NET 9 vs .NET 10 Runtime Async Performance Benchmark");
        Console.WriteLine("=========================================================");
        Console.WriteLine();

        if (args.Length > 0)
        {
            switch (args[0].ToLower())
            {
                case "--all":
                    RunAllBenchmarks();
                    break;
                case "--net9-vs-net10":
                    RunNet9VsNet10Benchmarks();
                    break;
                case "--runtime-async-enabled":
                    RunRuntimeAsyncEnabledBenchmarks();
                    break;
                case "--help":
                case "-h":
                    ShowHelp();
                    break;
                default:
                    Console.WriteLine($"❌ Unknown argument: {args[0]}");
                    ShowHelp();
                    break;
            }
        }
        else
        {
            ShowInteractiveMenu();
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("📖 Usage:");
        Console.WriteLine("  --all                   Run all benchmark configurations");
        Console.WriteLine("  --net9-vs-net10        Run .NET 9 vs .NET 10 (no runtime async) comparison");
        Console.WriteLine("  --runtime-async-enabled Run .NET 10 with runtime async enabled benchmarks");
        Console.WriteLine("  --help, -h             Show this help message");
        Console.WriteLine();
        Console.WriteLine("💡 Interactive mode: Run without arguments to see menu options");
    }

    private static void ShowInteractiveMenu()
    {
        Console.WriteLine("📋 Available Benchmark Configurations:");
        Console.WriteLine("======================================");
        Console.WriteLine();
        Console.WriteLine("1. 🔄 .NET 9 vs .NET 10 (no runtime async) - Framework comparison");
        Console.WriteLine("2. ⚡ .NET 10 with Runtime Async enabled - Feature performance");
        Console.WriteLine("3. 🏁 All benchmarks - Complete comparison");
        Console.WriteLine("4. ❌ Exit");
        Console.WriteLine();
        Console.Write("Select option (1-4): ");

        var input = Console.ReadLine();
        switch (input)
        {
            case "1":
                RunNet9VsNet10Benchmarks();
                break;
            case "2":
                RunRuntimeAsyncEnabledBenchmarks();
                break;
            case "3":
                RunAllBenchmarks();
                break;
            case "4":
                Console.WriteLine("👋 Goodbye!");
                return;
            default:
                Console.WriteLine("❌ Invalid selection. Please try again.");
                ShowInteractiveMenu();
                break;
        }
    }

    private static void RunNet9VsNet10Benchmarks()
    {
        Console.WriteLine("🔄 Running .NET 9 vs .NET 10 (no runtime async) benchmarks...");
        Console.WriteLine("================================================================");
        Console.WriteLine();
        
        var summary = BenchmarkRunner.Run<RuntimeAsyncBenchmarks>();
        
        Console.WriteLine();
        Console.WriteLine("📊 Benchmark completed! Check the results above for performance comparison.");
    }

    private static void RunRuntimeAsyncEnabledBenchmarks()
    {
        Console.WriteLine("⚡ Running .NET 10 with Runtime Async enabled benchmarks...");
        Console.WriteLine("============================================================");
        Console.WriteLine();
        Console.WriteLine("ℹ️  This benchmark requires DOTNET_RuntimeAsync=1 environment variable");
        Console.WriteLine();
        
        var summary = BenchmarkRunner.Run<RuntimeAsyncEnabledBenchmarks>();
        
        Console.WriteLine();
        Console.WriteLine("📊 Benchmark completed! Check the results above for runtime async performance.");
    }

    private static void RunAllBenchmarks()
    {
        Console.WriteLine("🏁 Running all benchmark configurations...");
        Console.WriteLine("==========================================");
        Console.WriteLine();
        Console.WriteLine("This will run:");
        Console.WriteLine("• .NET 9 vs .NET 10 comparison");
        Console.WriteLine("• .NET 10 with Runtime Async enabled");
        Console.WriteLine();
        
        Console.WriteLine("📈 Part 1: .NET 9 vs .NET 10 (no runtime async)");
        Console.WriteLine("================================================");
        RunNet9VsNet10Benchmarks();
        
        Console.WriteLine();
        Console.WriteLine("📈 Part 2: .NET 10 with Runtime Async enabled");
        Console.WriteLine("==============================================");
        RunRuntimeAsyncEnabledBenchmarks();
        
        Console.WriteLine();
        Console.WriteLine("✅ All benchmarks completed!");
        Console.WriteLine();
        Console.WriteLine("📋 Summary:");
        Console.WriteLine("• Compare the .NET 9 vs .NET 10 results to see framework improvements");
        Console.WriteLine("• Compare .NET 10 results with and without Runtime Async to see feature impact");
        Console.WriteLine("• Look for improvements in execution time and memory allocations");
    }
}