using BenchmarkDotNet.Attributes;

namespace RuntimeAsyncBenchmarks;

/// <summary>
/// Benchmark specifically for .NET 10 with Runtime Async enabled
/// This requires the DOTNET_RuntimeAsync=1 environment variable
/// </summary>
[MemoryDiagnoser]
[SimpleJob(baseline: true)]
public class RuntimeAsyncEnabledBenchmarks
{
    private const int TaskCount = 100;
    private const int DelayMs = 10;

    /// <summary>
    /// Benchmark: Simple async task workload with runtime async enabled
    /// </summary>
    [Benchmark(Description = "Simple Async Tasks (Runtime Async ON)")]
    public async Task SimpleAsyncTasksWithRuntimeAsync()
    {
        var tasks = new List<Task>();
        for (int i = 0; i < TaskCount; i++)
        {
            tasks.Add(SimpleAsyncTask(i));
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Benchmark: CPU-bound async workload with runtime async enabled
    /// </summary>
    [Benchmark(Description = "CPU-bound Async Tasks (Runtime Async ON)")]
    public async Task CpuBoundAsyncTasksWithRuntimeAsync()
    {
        var tasks = new List<Task<int>>();
        for (int i = 0; i < TaskCount; i++)
        {
            tasks.Add(CpuBoundAsyncTask(i));
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Benchmark: I/O simulation with runtime async enabled
    /// </summary>
    [Benchmark(Description = "I/O Simulation Tasks (Runtime Async ON)")]
    public async Task IoSimulationTasksWithRuntimeAsync()
    {
        var tasks = new List<Task>();
        for (int i = 0; i < TaskCount; i++)
        {
            tasks.Add(IoSimulationTask(i));
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Benchmark: Mixed async workload with runtime async enabled
    /// </summary>
    [Benchmark(Description = "Mixed Workload Tasks (Runtime Async ON)")]
    public async Task MixedWorkloadTasksWithRuntimeAsync()
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

