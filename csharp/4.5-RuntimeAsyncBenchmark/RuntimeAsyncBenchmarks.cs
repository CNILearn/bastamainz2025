using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Environments;
using System.Diagnostics;

namespace RuntimeAsyncBenchmarks;

/// <summary>
/// Simple benchmark class for .NET 9 vs .NET 10 comparison
/// Uses simple job configuration to avoid complex runtime detection issues
/// </summary>
[MemoryDiagnoser]
[SimpleJob(baseline: true)]
public class RuntimeAsyncBenchmarks
{
    private const int TaskCount = 100;
    private const int DelayMs = 10;

    /// <summary>
    /// Benchmark: Simple async task workload - 100 concurrent tasks with delays and yields
    /// This mirrors the workload from the RuntimeAsyncFeature sample
    /// </summary>
    [Benchmark(Description = "Simple Async Tasks (100 concurrent)")]
    public async Task SimpleAsyncTasks()
    {
        var tasks = new List<Task>();
        for (int i = 0; i < TaskCount; i++)
        {
            tasks.Add(SimpleAsyncTask(i));
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Benchmark: CPU-bound async workload with more computation
    /// </summary>
    [Benchmark(Description = "CPU-bound Async Tasks")]
    public async Task CpuBoundAsyncTasks()
    {
        var tasks = new List<Task<int>>();
        for (int i = 0; i < TaskCount; i++)
        {
            tasks.Add(CpuBoundAsyncTask(i));
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Benchmark: I/O simulation with task yields
    /// </summary>
    [Benchmark(Description = "I/O Simulation Tasks")]
    public async Task IoSimulationTasks()
    {
        var tasks = new List<Task>();
        for (int i = 0; i < TaskCount; i++)
        {
            tasks.Add(IoSimulationTask(i));
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Benchmark: Mixed async workload (I/O + CPU)
    /// </summary>
    [Benchmark(Description = "Mixed Workload Tasks")]
    public async Task MixedWorkloadTasks()
    {
        var tasks = new List<Task>();
        for (int i = 0; i < TaskCount; i++)
        {
            if (i % 2 == 0)
                tasks.Add(SimpleAsyncTask(i));
            else
                tasks.Add(CpuBoundAsyncTask(i));
        }
        await Task.WhenAll(tasks);
    }

    private static async Task SimpleAsyncTask(int id)
    {
        await Task.Delay(DelayMs); // Simulate some async work
        await Task.Yield();        // Allow other tasks to proceed
        var result = id * id;      // Simple computation
    }

    private static async Task<int> CpuBoundAsyncTask(int id)
    {
        await Task.Yield(); // Yield to allow scheduling
        
        // CPU-bound work
        int result = 0;
        for (int i = 0; i < 1000; i++)
        {
            result += (id + i) * (id + i);
        }
        
        await Task.Yield(); // Yield again
        return result;
    }

    private static async Task IoSimulationTask(int id)
    {
        // Simulate multiple I/O operations
        await Task.Delay(5);  // First I/O
        await Task.Yield();
        await Task.Delay(5);  // Second I/O
        await Task.Yield();
        
        var result = id % 100; // Simple computation
    }
}

