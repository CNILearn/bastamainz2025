using LockKeywordSample.Models;

namespace LockKeywordSample.Examples;

/// <summary>
/// Demonstrates lock keyword usage with both traditional lock (Monitor) and new Lock type
/// </summary>
public static class LockExamples
{
    /// <summary>
    /// Demonstrates basic lock usage comparison
    /// </summary>
    public static async Task DemonstrateBasicLocking()
    {
        Console.WriteLine("🔐 Basic Locking Comparison");
        Console.WriteLine("==========================");
        Console.WriteLine();

        var counter = new SharedCounter();
        
        Console.WriteLine("📝 Single-threaded operations:");
        
        // Traditional lock
        var start = DateTime.UtcNow;
        for (int i = 0; i < 1000; i++)
        {
            counter.IncrementTraditional();
        }
        var traditionalTime = DateTime.UtcNow - start;
        
        counter.Reset();
        
        // Lock type
        start = DateTime.UtcNow;
        for (int i = 0; i < 1000; i++)
        {
            counter.IncrementWithLockType();
        }
        var lockTypeTime = DateTime.UtcNow - start;
        
        Console.WriteLine($"  Traditional lock: {traditionalTime.TotalMicroseconds:F1} μs");
        Console.WriteLine($"  Lock type:        {lockTypeTime.TotalMicroseconds:F1} μs");
        Console.WriteLine($"  Counter value:    {counter.Value}");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates multi-threaded operations
    /// </summary>
    public static async Task DemonstrateMultiThreadedLocking()
    {
        Console.WriteLine("🧵 Multi-threaded Locking Comparison");
        Console.WriteLine("===================================");
        Console.WriteLine();

        const int threadCount = 10;
        const int incrementsPerThread = 1000;
        
        // Test traditional lock
        Console.WriteLine("🔄 Testing traditional lock with multiple threads:");
        var counterTraditional = new SharedCounter();
        var tasksTraditional = new Task[threadCount];
        
        var startTime = DateTime.UtcNow;
        for (int i = 0; i < threadCount; i++)
        {
            tasksTraditional[i] = Task.Run(() =>
            {
                for (int j = 0; j < incrementsPerThread; j++)
                {
                    counterTraditional.IncrementTraditional();
                }
            });
        }
        
        await Task.WhenAll(tasksTraditional);
        var traditionalTime = DateTime.UtcNow - startTime;
        
        Console.WriteLine($"  Threads: {threadCount}, Increments per thread: {incrementsPerThread}");
        Console.WriteLine($"  Final value: {counterTraditional.Value} (expected: {threadCount * incrementsPerThread})");
        Console.WriteLine($"  Time taken: {traditionalTime.TotalMilliseconds:F1} ms");
        Console.WriteLine();
        
        // Test Lock type
        Console.WriteLine("🔄 Testing Lock type with multiple threads:");
        var counterLockType = new SharedCounter();
        var tasksLockType = new Task[threadCount];
        
        startTime = DateTime.UtcNow;
        for (int i = 0; i < threadCount; i++)
        {
            tasksLockType[i] = Task.Run(() =>
            {
                for (int j = 0; j < incrementsPerThread; j++)
                {
                    counterLockType.IncrementWithLockType();
                }
            });
        }
        
        await Task.WhenAll(tasksLockType);
        var lockTypeTime = DateTime.UtcNow - startTime;
        
        Console.WriteLine($"  Threads: {threadCount}, Increments per thread: {incrementsPerThread}");
        Console.WriteLine($"  Final value: {counterLockType.Value} (expected: {threadCount * incrementsPerThread})");
        Console.WriteLine($"  Time taken: {lockTypeTime.TotalMilliseconds:F1} ms");
        Console.WriteLine();
        
        // Compare performance
        var improvement = ((traditionalTime.TotalMilliseconds - lockTypeTime.TotalMilliseconds) / traditionalTime.TotalMilliseconds) * 100;
        Console.WriteLine($"⚡ Performance comparison:");
        Console.WriteLine($"  Traditional lock: {traditionalTime.TotalMilliseconds:F1} ms");
        Console.WriteLine($"  Lock type:        {lockTypeTime.TotalMilliseconds:F1} ms");
        Console.WriteLine($"  Improvement:      {improvement:+0.0;-0.0;0}%");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates financial operations with both lock types
    /// </summary>
    public static async Task DemonstrateFinancialOperations()
    {
        Console.WriteLine("💰 Financial Operations Example");
        Console.WriteLine("==============================");
        Console.WriteLine();

        const decimal initialBalance = 10000m;
        const int operationsPerThread = 100;
        const int threadCount = 20;
        
        // Traditional lock approach
        Console.WriteLine("🏦 Testing traditional lock with bank operations:");
        var accountTraditional = new BankAccount(initialBalance);
        var random = new Random(42); // Fixed seed for reproducible results
        
        var tasksTraditional = new Task[threadCount];
        var startTime = DateTime.UtcNow;
        
        for (int i = 0; i < threadCount; i++)
        {
            int threadId = i;
            tasksTraditional[i] = Task.Run(() =>
            {
                var threadRandom = new Random(42 + threadId);
                for (int j = 0; j < operationsPerThread; j++)
                {
                    if (threadRandom.Next(2) == 0)
                    {
                        accountTraditional.DepositTraditional(threadRandom.Next(1, 100));
                    }
                    else
                    {
                        accountTraditional.WithdrawTraditional(threadRandom.Next(1, 50));
                    }
                }
            });
        }
        
        await Task.WhenAll(tasksTraditional);
        var traditionalTime = DateTime.UtcNow - startTime;
        
        Console.WriteLine($"  Initial balance: ${initialBalance:F2}");
        Console.WriteLine($"  Final balance:   ${accountTraditional.Balance:F2}");
        Console.WriteLine($"  Operations:      {threadCount * operationsPerThread}");
        Console.WriteLine($"  Time taken:      {traditionalTime.TotalMilliseconds:F1} ms");
        Console.WriteLine();
        
        // Lock type approach
        Console.WriteLine("🏦 Testing Lock type with bank operations:");
        var accountLockType = new BankAccount(initialBalance);
        
        var tasksLockType = new Task[threadCount];
        startTime = DateTime.UtcNow;
        
        for (int i = 0; i < threadCount; i++)
        {
            int threadId = i;
            tasksLockType[i] = Task.Run(() =>
            {
                var threadRandom = new Random(42 + threadId);
                for (int j = 0; j < operationsPerThread; j++)
                {
                    if (threadRandom.Next(2) == 0)
                    {
                        accountLockType.DepositWithLockType(threadRandom.Next(1, 100));
                    }
                    else
                    {
                        accountLockType.WithdrawWithLockType(threadRandom.Next(1, 50));
                    }
                }
            });
        }
        
        await Task.WhenAll(tasksLockType);
        var lockTypeTime = DateTime.UtcNow - startTime;
        
        Console.WriteLine($"  Initial balance: ${initialBalance:F2}");
        Console.WriteLine($"  Final balance:   ${accountLockType.Balance:F2}");
        Console.WriteLine($"  Operations:      {threadCount * operationsPerThread}");
        Console.WriteLine($"  Time taken:      {lockTypeTime.TotalMilliseconds:F1} ms");
        Console.WriteLine();
        
        // Performance comparison
        var improvement = ((traditionalTime.TotalMilliseconds - lockTypeTime.TotalMilliseconds) / traditionalTime.TotalMilliseconds) * 100;
        Console.WriteLine($"⚡ Performance comparison:");
        Console.WriteLine($"  Traditional lock: {traditionalTime.TotalMilliseconds:F1} ms");
        Console.WriteLine($"  Lock type:        {lockTypeTime.TotalMilliseconds:F1} ms");
        Console.WriteLine($"  Improvement:      {improvement:+0.0;-0.0;0}%");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates producer-consumer pattern with both lock types
    /// </summary>
    public static async Task DemonstrateProducerConsumer()
    {
        Console.WriteLine("🏭 Producer-Consumer Pattern Example");
        Console.WriteLine("===================================");
        Console.WriteLine();

        const int bufferSize = 100;
        const int itemsPerProducer = 500;
        const int producerCount = 5;
        const int consumerCount = 3;
        
        // Traditional lock approach
        Console.WriteLine("📦 Testing traditional lock with producer-consumer:");
        var bufferTraditional = new ProducerConsumerBuffer<int>(bufferSize);
        var producedCountTraditional = 0;
        var consumedCountTraditional = 0;
        
        var startTime = DateTime.UtcNow;
        var allTasks = new List<Task>();
        
        // Producer tasks
        for (int i = 0; i < producerCount; i++)
        {
            int producerId = i;
            allTasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < itemsPerProducer; j++)
                {
                    while (!bufferTraditional.TryAddTraditional(producerId * 1000 + j))
                    {
                        Thread.Yield(); // Wait for space
                    }
                    Interlocked.Increment(ref producedCountTraditional);
                }
            }));
        }
        
        // Consumer tasks
        for (int i = 0; i < consumerCount; i++)
        {
            allTasks.Add(Task.Run(() =>
            {
                while (consumedCountTraditional < producerCount * itemsPerProducer)
                {
                    if (bufferTraditional.TryRemoveTraditional(out var item))
                    {
                        Interlocked.Increment(ref consumedCountTraditional);
                    }
                    else
                    {
                        Thread.Yield(); // Wait for items
                    }
                }
            }));
        }
        
        await Task.WhenAll(allTasks);
        var traditionalTime = DateTime.UtcNow - startTime;
        
        Console.WriteLine($"  Producers: {producerCount}, Consumers: {consumerCount}");
        Console.WriteLine($"  Items produced: {producedCountTraditional}");
        Console.WriteLine($"  Items consumed: {consumedCountTraditional}");
        Console.WriteLine($"  Time taken:     {traditionalTime.TotalMilliseconds:F1} ms");
        Console.WriteLine();
        
        // Lock type approach
        Console.WriteLine("📦 Testing Lock type with producer-consumer:");
        var bufferLockType = new ProducerConsumerBuffer<int>(bufferSize);
        var producedCountLockType = 0;
        var consumedCountLockType = 0;
        
        startTime = DateTime.UtcNow;
        allTasks.Clear();
        
        // Producer tasks
        for (int i = 0; i < producerCount; i++)
        {
            int producerId = i;
            allTasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < itemsPerProducer; j++)
                {
                    while (!bufferLockType.TryAddWithLockType(producerId * 1000 + j))
                    {
                        Thread.Yield(); // Wait for space
                    }
                    Interlocked.Increment(ref producedCountLockType);
                }
            }));
        }
        
        // Consumer tasks
        for (int i = 0; i < consumerCount; i++)
        {
            allTasks.Add(Task.Run(() =>
            {
                while (consumedCountLockType < producerCount * itemsPerProducer)
                {
                    if (bufferLockType.TryRemoveWithLockType(out var item))
                    {
                        Interlocked.Increment(ref consumedCountLockType);
                    }
                    else
                    {
                        Thread.Yield(); // Wait for items
                    }
                }
            }));
        }
        
        await Task.WhenAll(allTasks);
        var lockTypeTime = DateTime.UtcNow - startTime;
        
        Console.WriteLine($"  Producers: {producerCount}, Consumers: {consumerCount}");
        Console.WriteLine($"  Items produced: {producedCountLockType}");
        Console.WriteLine($"  Items consumed: {consumedCountLockType}");
        Console.WriteLine($"  Time taken:     {lockTypeTime.TotalMilliseconds:F1} ms");
        Console.WriteLine();
        
        // Performance comparison
        var improvement = ((traditionalTime.TotalMilliseconds - lockTypeTime.TotalMilliseconds) / traditionalTime.TotalMilliseconds) * 100;
        Console.WriteLine($"⚡ Performance comparison:");
        Console.WriteLine($"  Traditional lock: {traditionalTime.TotalMilliseconds:F1} ms");
        Console.WriteLine($"  Lock type:        {lockTypeTime.TotalMilliseconds:F1} ms");
        Console.WriteLine($"  Improvement:      {improvement:+0.0;-0.0;0}%");
        Console.WriteLine();
    }
}