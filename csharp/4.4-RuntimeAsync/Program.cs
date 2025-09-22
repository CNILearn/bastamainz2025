using System.Diagnostics;

namespace RuntimeAsyncFeature;

/// <summary>
/// Demonstrates the .NET 10 Runtime Async preview feature.
/// This sample showcases the performance differences when the runtime-async feature is enabled vs disabled.
/// 
/// Key Configuration Requirements:
/// - EnablePreviewFeatures: true
/// - Features: runtime-async=on
/// - Environment Variable: DOTNET_RuntimeAsync=1
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("🚀 .NET 10 Runtime Async Feature Demo");
        Console.WriteLine("=====================================");
        Console.WriteLine();
        
        // Check if runtime async is enabled via environment variable
        var runtimeAsyncEnabled = Environment.GetEnvironmentVariable("DOTNET_RuntimeAsync") == "1";
        Console.WriteLine($"🔧 Runtime Async Status: {(runtimeAsyncEnabled ? "✅ ENABLED" : "❌ DISABLED")}");
        Console.WriteLine($"🔧 Environment Variable DOTNET_RuntimeAsync: {Environment.GetEnvironmentVariable("DOTNET_RuntimeAsync") ?? "Not Set"}");
        Console.WriteLine();
        
        if (!runtimeAsyncEnabled)
        {
            Console.WriteLine("⚠️  To enable Runtime Async feature, run with:");
            Console.WriteLine("   DOTNET_RuntimeAsync=1 dotnet run");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("✨ Runtime Async optimization is enabled!");
        }

        // Run a simple async demonstration
        await RunSimpleAsyncDemo();
        
        Console.WriteLine();
        Console.WriteLine("✅ Runtime Async Feature demonstration completed!");
    }

    private static async Task RunSimpleAsyncDemo()
    {
        Console.WriteLine("🎯 Simple Async Workload Demo");
        Console.WriteLine("=============================");
        Console.WriteLine();

        var stopwatch = Stopwatch.StartNew();
        
        // Create 100 simple async tasks
        var tasks = new List<Task>();
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(SimpleAsyncTask(i));
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        Console.WriteLine($"   ✅ Completed 100 async tasks in {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"   📊 Average per task: {stopwatch.ElapsedMilliseconds / 100.0:F2} ms");
    }

    private static async Task SimpleAsyncTask(int id)
    {
        await Task.Delay(10); // Simulate some async work
        await Task.Yield();   // Allow other tasks to proceed
        var result = id * id; // Simple computation
    }
}