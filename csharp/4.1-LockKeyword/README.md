# C# Lock Keyword Sample - Traditional vs New Lock Type

This sample demonstrates the usage of the `lock` keyword with both traditional C# lock statement (Monitor-based) and the new `Lock` type introduced in .NET 10. The sample provides practical examples and performance benchmarks comparing both approaches.

## 🚀 Features

- **Traditional Lock Examples**: Demonstrates classic `lock` statement using Monitor internally
- **New Lock Type Examples**: Shows the new `Lock` type with the same `lock` keyword syntax
- **Thread-Safe Operations**: Counter, bank account, and producer-consumer implementations
- **Performance Benchmarks**: Comprehensive comparisons using BenchmarkDotNet
- **Multi-threading Scenarios**: Real-world concurrent programming examples
- **Modern C# Features**: Collection expressions, primary constructors, target-typed new

## 📋 Prerequisites

- **.NET 10 RC 1 SDK** or later
- **Visual Studio 2024** or **VS Code** with C# extension
- **C# 14 Language Support** (LangVersion=preview)

## 🏗️ Architecture

```
LockKeywordSample/
├── Models/                           # Thread-safe data structures
│   └── ThreadSafeModels.cs          # SharedCounter, BankAccount, ProducerConsumerBuffer
├── Examples/                        # Core demonstrations
│   └── LockExamples.cs              # Main examples comparing both lock types
├── Benchmarks/                      # Performance benchmarking
│   └── LockBenchmarks.cs            # BenchmarkDotNet tests
├── Program.cs                       # Main application
├── LockKeywordSample.csproj         # Project file
└── README.md                        # This file
```

## 🔧 Setup

### 1. Install Dependencies

```bash
# Navigate to project directory
cd src/LockKeywordSample

# Restore NuGet packages
dotnet restore
```

### 2. Run the Application

```bash
# Run the demonstration
dotnet run

# Run with benchmarks
dotnet run -- --benchmark
```

## 💡 Key Concepts

### Traditional Lock Statement

The traditional `lock` statement in C# uses the Monitor class internally:

```csharp
private readonly object _lockObject = new();

public void IncrementTraditional()
{
    lock (_lockObject)  // Uses Monitor internally
    {
        _value++;
    }
}
```

**Characteristics:**
- Uses any reference type as lock object
- Monitor-based implementation
- Re-entrant (same thread can acquire multiple times)
- Well-established and widely compatible
- Optimized over many .NET versions

### New Lock Type

.NET 10 introduces a new `Lock` type designed for better performance:

```csharp
private readonly Lock _lockType = new();

public void IncrementWithLockType()
{
    lock (_lockType)  // Uses new Lock type
    {
        _value++;
    }
}
```

**Characteristics:**
- Dedicated `Lock` type for synchronization
- Potentially better performance in high-contention scenarios
- Same familiar `lock` keyword syntax
- Re-entrant locking support
- Designed with modern threading patterns in mind

### Key Differences

| Aspect | Traditional Lock | New Lock Type |
|--------|------------------|---------------|
| **Syntax** | `lock (object)` | `lock (Lock)` |
| **Implementation** | Monitor-based | Optimized Lock type |
| **Performance** | Well-optimized | Better in specific scenarios |
| **Compatibility** | Full backward compatibility | .NET 10+ |
| **Memory** | Uses any reference type | Dedicated Lock instances |
| **Re-entrancy** | Supported | Supported |

## 🚀 Scenarios Where `System.Threading.Lock` Excels

The new `System.Threading.Lock` in .NET 10 can outperform traditional `Monitor`-based locks in specific scenarios because it's a purpose-built, minimal, stack-only synchronization primitive that avoids `Monitor`'s general-purpose overhead.

| Scenario | Why `Lock` Wins | Technical Reason |
|----------|----------------|------------------|
| **High-frequency, short critical sections** | Lower entry/exit overhead | `Lock.EnterScope()` is a `ref struct` with no heap allocation, no boxing, and minimal `try/finally` state machine overhead |
| **Low contention** | Avoids kernel transitions | `Monitor` has complex fairness and wait queue logic; `Lock` uses a leaner fast path for uncontended locks |
| **Tight loops in performance-sensitive code** | Reduced instruction count | Compiler emits direct calls to `Lock.EnterScope()` instead of `Monitor.Enter/Exit`, skipping bookkeeping |
| **No need for `Monitor` extras** | Stripped-down design | `Lock` doesn't support condition variables or recursion, avoiding the cost of supporting them |
| **Cache-friendly workloads** | Smaller memory footprint | `Lock` is a dedicated struct with predictable layout, improving CPU cache locality vs `Monitor`'s object header locking |

### ⚙️ Why It's Faster in These Cases

- **Special compiler lowering**: When you write `lock(myLock) { ... }` and `myLock` is a `System.Threading.Lock`, the compiler generates code that calls `EnterScope()` directly — no `Monitor` calls at all.

- **Stack-bound scope**: The `LockScope` returned by `EnterScope()` is a `ref struct`, so it's guaranteed to live on the stack and be disposed deterministically at scope exit.

- **No object header manipulation**: `Monitor` works by locking on an object's sync block index in the CLR header, which involves more indirection. `Lock` maintains its own lightweight state.

### ⚠️ When `Lock` Won't Help (or Might Be Slower)

- **If you need `Monitor` features** like `Pulse`, `Wait`, or recursion — `Lock` doesn't support them
- **Under heavy contention** where threads block often — the performance difference may shrink, as both will end up in OS waits
- **Async code** — `Lock` is synchronous only; for async you still need `SemaphoreSlim` or similar

💡 **Rule of thumb**: If you have **short, frequent, synchronous critical sections** and don't need advanced `Monitor` features, `System.Threading.Lock` can give you a measurable speed-up — especially in low-contention, high-throughput code paths.

## 🎯 Use Cases Demonstrated

### 1. Basic Counter Operations
- Simple increment operations
- Performance comparison in single-threaded scenarios
- Thread-safe counter implementation

### 2. Multi-threaded Operations
- High-contention scenarios with multiple threads
- Correctness verification (ensuring all operations complete)
- Performance measurement under concurrent access

### 3. Financial Operations
- Bank account with deposits and withdrawals
- Thread-safe balance management
- Real-world business logic scenarios

### 4. Producer-Consumer Pattern
- Thread-safe buffer implementation
- Multiple producers and consumers
- Complex synchronization scenarios

## 🔍 Sample Operations

### Basic Locking Demo
```csharp
// Traditional approach
lock (_lockObject)
{
    _value++;
}

// New Lock type approach
lock (_lockType)
{
    _value++;
}
```

### Multi-threaded Scenario
```csharp
// Create multiple tasks that increment counters
var tasks = new Task[threadCount];
for (int i = 0; i < threadCount; i++)
{
    tasks[i] = Task.Run(() =>
    {
        for (int j = 0; j < incrementsPerThread; j++)
        {
            counter.IncrementWithLockType(); // or IncrementTraditional()
        }
    });
}
await Task.WhenAll(tasks);
```

### Producer-Consumer Pattern
```csharp
// Producer
while (!buffer.TryAddWithLockType(item))
{
    Thread.Yield(); // Wait for space
}

// Consumer
if (buffer.TryRemoveWithLockType(out var item))
{
    // Process item
}
```

## 📊 Performance Benchmarks

The included benchmarks specifically target scenarios where `System.Threading.Lock` has advantages:

### Lock Type Advantage Scenarios
- **High-frequency, Short Critical Sections**: Tight loops with minimal work in lock
- **Low Contention**: Uncontended locks where Lock's fast path excels
- **Stack-only Operations**: Simple calculations without heap allocations
- **Cache-friendly Workloads**: Sequential memory access patterns

### Traditional Benchmark Categories
- **Simple Operations**: Basic increment operations
- **Complex Operations**: Multiple calculations within lock
- **Memory Allocation**: Lock overhead with allocations
- **Nested Locking**: Re-entrant lock scenarios

### Multi-threaded Contention
- **Low Contention**: 2 threads competing for lock
- **Medium Contention**: 4 threads competing for lock
- **High Contention**: 8+ threads competing for lock

### Expected Results
Based on `System.Threading.Lock` design characteristics:

- **High-frequency operations**: Lock type shows significant advantages (20-40% improvement)
- **Low contention**: Lock's fast path outperforms Monitor's overhead
- **Cache-friendly scenarios**: Better CPU cache locality with Lock's predictable layout
- **Heavy contention**: Performance differences may diminish as both use OS waits

## 📊 Sample Output

The application demonstrates all lock scenarios with timing information:

```
🔐 Basic Locking Comparison
==========================

📝 Single-threaded operations:
  Traditional lock: 145.2 μs
  Lock type:        132.7 μs
  Counter value:    1000

🧵 Multi-threaded Locking Comparison
===================================

🔄 Testing traditional lock with multiple threads:
  Threads: 10, Increments per thread: 1000
  Final value: 10000 (expected: 10000)
  Time taken: 23.4 ms

🔄 Testing Lock type with multiple threads:
  Threads: 10, Increments per thread: 1000
  Final value: 10000 (expected: 10000)
  Time taken: 19.8 ms

⚡ Performance comparison:
  Traditional lock: 23.4 ms
  Lock type:        19.8 ms
  Improvement:      +15.4%
```

## 🔗 Related Technologies

- **System.Threading**: Core threading primitives
- **Monitor Class**: Traditional lock implementation
- **Lock Type**: New .NET 10 synchronization primitive
- **BenchmarkDotNet**: Performance measurement framework
- **Task Parallel Library**: Modern async/await patterns

## 📚 Additional Resources

- [C# Lock Statement Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/lock)
- [.NET 10 Lock Type Documentation](https://docs.microsoft.com/en-us/dotnet/api/system.threading.lock)
- [Threading in .NET](https://docs.microsoft.com/en-us/dotnet/standard/threading/)
- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/)
- [.NET 10 Release Notes](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10)

## 🎯 Real-World Applications

### High-Performance Computing
- **Numerical Simulations**: Concurrent calculations with shared state
- **Data Processing**: Parallel processing pipelines
- **Scientific Computing**: Thread-safe mathematical operations

### Web Applications
- **Caching Systems**: Thread-safe cache implementations
- **Session Management**: Concurrent user session handling
- **Rate Limiting**: Thread-safe request throttling

### Financial Systems
- **Trading Platforms**: High-frequency transaction processing
- **Payment Processing**: Concurrent payment handling
- **Risk Management**: Thread-safe risk calculations

### Gaming
- **Game State Management**: Concurrent player state updates
- **Multiplayer Synchronization**: Thread-safe game world updates
- **Performance Optimization**: Low-latency game loops

## 🤝 Contributing

This sample is part of the .NET 10 samples collection. Feel free to submit issues and enhancement requests!

## 📄 License

This sample is licensed under the MIT License. See LICENSE file for details.

---

**Note**: This sample demonstrates the actual .NET 10 Lock type functionality available in .NET 10 RC 1. The Lock type provides a modern, potentially more performant alternative to traditional object-based locking while maintaining the familiar `lock` keyword syntax.