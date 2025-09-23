using BenchmarkDotNet.Attributes;
using LockKeywordSample.Models;

namespace LockKeywordSample.Benchmarks;

/// <summary>
/// Benchmarks comparing traditional lock (Monitor) vs new Lock type performance
/// Specifically targets scenarios where System.Threading.Lock has advantages:
/// - High-frequency, short critical sections
/// - Low contention scenarios  
/// - Tight loops in performance-sensitive code
/// - Cache-friendly workloads
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class LockBenchmarks
{
    private SharedCounter _counter = null!;
    private BankAccount _account = null!;
    private ProducerConsumerBuffer<int> _buffer = null!;
    private readonly object _lockObject = new();
    private readonly Lock _lockType = new();

    [GlobalSetup]
    public void Setup()
    {
        _counter = new SharedCounter();
        _account = new BankAccount(100000m);
        _buffer = new ProducerConsumerBuffer<int>(1000);
    }

    [Benchmark(Baseline = true)]
    public void SimpleIncrementTraditional()
    {
        _counter.IncrementTraditional();
    }

    [Benchmark]
    public void SimpleIncrementLockType()
    {
        _counter.IncrementWithLockType();
    }

    [Benchmark]
    public void ComplexOperationTraditional()
    {
        _counter.ComplexOperationTraditional(100);
    }

    [Benchmark]
    public void ComplexOperationLockType()
    {
        _counter.ComplexOperationWithLockType(100);
    }

    [Benchmark]
    public void BankDepositTraditional()
    {
        _account.DepositTraditional(1.0m);
    }

    [Benchmark]
    public void BankDepositLockType()
    {
        _account.DepositWithLockType(1.0m);
    }

    [Benchmark]
    public void BankWithdrawTraditional()
    {
        _account.WithdrawTraditional(1.0m);
    }

    [Benchmark]
    public void BankWithdrawLockType()
    {
        _account.WithdrawWithLockType(1.0m);
    }

    [Benchmark]
    public void BufferAddTraditional()
    {
        _buffer.TryAddTraditional(42);
    }

    [Benchmark]
    public void BufferAddLockType()
    {
        _buffer.TryAddWithLockType(42);
    }

    [Benchmark]
    public void BufferRemoveTraditional()
    {
        _buffer.TryRemoveTraditional(out _);
    }

    [Benchmark]
    public void BufferRemoveLockType()
    {
        _buffer.TryRemoveWithLockType(out _);
    }

    // Raw lock overhead comparison
    private int _counter1 = 0;
    private int _counter2 = 0;

    [Benchmark]
    public void RawLockTraditional()
    {
        lock (_lockObject)
        {
            _counter1++;
        }
    }

    [Benchmark]
    public void RawLockType()
    {
        lock (_lockType)
        {
            _counter2++;
        }
    }

    // High-frequency, short critical sections - where Lock type has advantages
    [Benchmark]
    public void HighFrequencyShortSectionTraditional()
    {
        // Simulates tight loop with minimal work in critical section
        for (int i = 0; i < 100; i++)
        {
            lock (_lockObject)
            {
                _counter1 = (_counter1 + 1) % 1000;
            }
        }
    }

    [Benchmark]
    public void HighFrequencyShortSectionLockType()
    {
        // Simulates tight loop with minimal work in critical section
        for (int i = 0; i < 100; i++)
        {
            lock (_lockType)
            {
                _counter2 = (_counter2 + 1) % 1000;
            }
        }
    }

    // Cache-friendly workloads - Lock type has better memory footprint
    private readonly int[] _cacheData = new int[64]; // Cache line size

    [Benchmark]
    public void CacheFriendlyTraditional()
    {
        lock (_lockObject)
        {
            // Access sequential memory locations
            for (int i = 0; i < _cacheData.Length; i++)
            {
                _cacheData[i] = i * 2;
            }
        }
    }

    [Benchmark]
    public void CacheFriendlyLockType()
    {
        lock (_lockType)
        {
            // Access sequential memory locations
            for (int i = 0; i < _cacheData.Length; i++)
            {
                _cacheData[i] = i * 2;
            }
        }
    }

    // Stack-only operations - Lock.EnterScope() advantage
    [Benchmark]
    public void StackOnlyOperationTraditional()
    {
        lock (_lockObject)
        {
            // Simple stack-based calculations
            int a = 10, b = 20, c = 30;
            int result = a + b * c - (a % b);
            _counter1 = result;
        }
    }

    [Benchmark]
    public void StackOnlyOperationLockType()
    {
        lock (_lockType)
        {
            // Simple stack-based calculations
            int a = 10, b = 20, c = 30;
            int result = a + b * c - (a % b);
            _counter2 = result;
        }
    }

    // Memory allocation comparison
    [Benchmark]
    public void MemoryAllocationTraditional()
    {
        lock (_lockObject)
        {
            var data = new int[100];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = i;
            }
        }
    }

    [Benchmark]
    public void MemoryAllocationLockType()
    {
        lock (_lockType)
        {
            var data = new int[100];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = i;
            }
        }
    }

    // Nested lock scenarios
    [Benchmark]
    public void NestedLockTraditional()
    {
        lock (_lockObject)
        {
            lock (_lockObject) // Re-entrant lock
            {
                _counter1++;
            }
        }
    }

    [Benchmark]
    public void NestedLockType()
    {
        lock (_lockType)
        {
            lock (_lockType) // Re-entrant lock
            {
                _counter2++;
            }
        }
    }
}

/// <summary>
/// Low contention benchmarks - where Lock type has significant advantages
/// Lock uses a leaner fast path for uncontended locks compared to Monitor
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class LowContentionBenchmarks
{
    private readonly object _lockObject = new();
    private readonly Lock _lockType = new();
    private volatile int _sharedValue1 = 0;
    private volatile int _sharedValue2 = 0;

    [Benchmark(Baseline = true)]
    public void LowContentionTraditional()
    {
        // Single threaded, uncontended locks - Monitor overhead
        for (int i = 0; i < 1000; i++)
        {
            lock (_lockObject)
            {
                _sharedValue1++;
            }
        }
    }

    [Benchmark]
    public void LowContentionLockType()
    {
        // Single threaded, uncontended locks - Lock fast path
        for (int i = 0; i < 1000; i++)
        {
            lock (_lockType)
            {
                _sharedValue2++;
            }
        }
    }

    [Benchmark]
    public void MinimalCriticalSectionTraditional()
    {
        // Absolutely minimal work in critical section
        for (int i = 0; i < 10000; i++)
        {
            lock (_lockObject)
            {
                // No-op - just lock overhead
            }
        }
    }

    [Benchmark]
    public void MinimalCriticalSectionLockType()
    {
        // Absolutely minimal work in critical section
        for (int i = 0; i < 10000; i++)
        {
            lock (_lockType)
            {
                // No-op - just lock overhead
            }
        }
    }
}

/// <summary>
/// Multi-threaded benchmarks comparing lock performance under contention
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class ContentionBenchmarks
{
    private SharedCounter _counterTraditional = null!;
    private SharedCounter _counterLockType = null!;
    private readonly object _lockObject = new();
    private readonly Lock _lockType = new();
    private volatile int _sharedValue1 = 0;
    private volatile int _sharedValue2 = 0;

    [GlobalSetup]
    public void Setup()
    {
        _counterTraditional = new SharedCounter();
        _counterLockType = new SharedCounter();
    }

    [Benchmark(Baseline = true)]
    [Arguments(2)]
    [Arguments(4)]
    [Arguments(8)]
    public async Task HighContentionTraditional(int threadCount)
    {
        var tasks = new Task[threadCount];
        const int iterationsPerThread = 1000;

        for (int i = 0; i < threadCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < iterationsPerThread; j++)
                {
                    lock (_lockObject)
                    {
                        _sharedValue1++;
                    }
                }
            });
        }

        await Task.WhenAll(tasks);
    }

    [Benchmark]
    [Arguments(2)]
    [Arguments(4)]
    [Arguments(8)]
    public async Task HighContentionLockType(int threadCount)
    {
        var tasks = new Task[threadCount];
        const int iterationsPerThread = 1000;

        for (int i = 0; i < threadCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < iterationsPerThread; j++)
                {
                    lock (_lockType)
                    {
                        _sharedValue2++;
                    }
                }
            });
        }

        await Task.WhenAll(tasks);
    }
}

/// <summary>
/// Static class to run the benchmarks
/// </summary>
public static class BenchmarkRunner
{
    public static void RunBenchmarks()
    {
        Console.WriteLine("🏁 Running Lock Keyword Benchmarks");
        Console.WriteLine("==================================");
        Console.WriteLine("This may take several minutes...\\n");

        Console.WriteLine("🔄 Running single-threaded benchmarks...");
        var summary1 = BenchmarkDotNet.Running.BenchmarkRunner.Run<LockBenchmarks>();
        
        Console.WriteLine("\\n⚡ Running low contention benchmarks (where Lock type excels)...");
        var summary2 = BenchmarkDotNet.Running.BenchmarkRunner.Run<LowContentionBenchmarks>();
        
        Console.WriteLine("\\n🧵 Running multi-threaded contention benchmarks...");
        var summary3 = BenchmarkDotNet.Running.BenchmarkRunner.Run<ContentionBenchmarks>();
        
        Console.WriteLine("\\n📊 Benchmark Summary:");
        Console.WriteLine("====================");
        Console.WriteLine("🚀 Scenarios Where System.Threading.Lock Excels:");
        Console.WriteLine();
        Console.WriteLine("• High-frequency, short critical sections:");
        Console.WriteLine("  - Lock.EnterScope() is a ref struct with no heap allocation");
        Console.WriteLine("  - No boxing, minimal try/finally state machine overhead");
        Console.WriteLine();
        Console.WriteLine("• Low contention scenarios:");
        Console.WriteLine("  - Monitor has complex fairness and wait queue logic");
        Console.WriteLine("  - Lock uses leaner fast path for uncontended locks");
        Console.WriteLine();
        Console.WriteLine("• Tight loops in performance-sensitive code:");
        Console.WriteLine("  - Compiler emits direct calls to Lock.EnterScope()");
        Console.WriteLine("  - Reduced instruction count vs Monitor.Enter/Exit");
        Console.WriteLine();
        Console.WriteLine("• Cache-friendly workloads:");
        Console.WriteLine("  - Lock is dedicated struct with predictable layout");
        Console.WriteLine("  - Better CPU cache locality vs Monitor object header locking");
        Console.WriteLine();
        Console.WriteLine("⚠️  When Lock Won't Help:");
        Console.WriteLine("• If you need Monitor features (Pulse, Wait, recursion)");
        Console.WriteLine("• Under heavy contention (both end up in OS waits)");
        Console.WriteLine("• Async code (Lock is synchronous only)");
        Console.WriteLine();
        Console.WriteLine("💡 Rule of thumb: Use Lock for short, frequent, synchronous");
        Console.WriteLine("   critical sections in low-contention, high-throughput scenarios.");
        Console.WriteLine("\\nSee detailed results above for specific performance metrics.");
    }
}