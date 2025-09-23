# .NET 10 Runtime Async Feature Sample

This sample demonstrates the new **Runtime Async** preview feature introduced in .NET 10 RC 1. The Runtime Async feature provides performance optimizations for asynchronous operations, reducing overhead and improving throughput for high-concurrency async workloads.

## 🚀 What is Runtime Async?

The Runtime Async feature is a preview optimization in .NET 10 that enhances the performance of async operations by:

- **Reducing async method overhead** - Optimized state machine generation for async methods
- **Improving task scheduling** - Better context switching and thread pool utilization
- **Reducing memory allocations** - Optimized async state machines with fewer allocations
- **Enhanced throughput** - Better performance for high-concurrency async workloads

## 📋 Prerequisites

- **.NET 10 RC 1 SDK** or later (version 10.0.100-rc.1.25451.107)
- **Visual Studio 2024** or **VS Code** with C# extension

## 🔧 Project Configuration

This sample includes the required project configurations to enable the Runtime Async feature:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <LangVersion>preview</LangVersion>
    <EnablePreviewFeatures>true</EnablePreviewFeatures>
  </PropertyGroup>
</Project>
```

### Key Configuration Elements:

- **`<TargetFramework>net10.0</TargetFramework>`** - Targets .NET 10
- **`<EnablePreviewFeatures>true</EnablePreviewFeatures>`** - Enables preview features
- **`<LangVersion>preview</LangVersion>`** - Uses preview C# language features

## 🎯 Running the Sample

### Method 1: Without Runtime Async (Baseline)

```bash
cd src/RuntimeAsyncFeature
dotnet run
```

**Expected Output:**
```
🚀 .NET 10 Runtime Async Feature Demo
=====================================

🔧 Runtime Async Status: ❌ DISABLED
🔧 Environment Variable DOTNET_RuntimeAsync: Not Set

⚠️  To enable Runtime Async feature, run with:
   DOTNET_RuntimeAsync=1 dotnet run

🎯 Simple Async Workload Demo
=============================

   ✅ Completed 100 async tasks in 13 ms
   📊 Average per task: 0.13 ms

✅ Runtime Async Feature demonstration completed!
```

### Method 2: With Runtime Async Enabled

```bash
cd src/RuntimeAsyncFeature
DOTNET_RuntimeAsync=1 dotnet run
```

**Expected Output:**
```
🚀 .NET 10 Runtime Async Feature Demo
=====================================

🔧 Runtime Async Status: ✅ ENABLED
🔧 Environment Variable DOTNET_RuntimeAsync: 1

✨ Runtime Async optimization is enabled!
🎯 Simple Async Workload Demo
=============================

   ✅ Completed 100 async tasks in 10 ms
   📊 Average per task: 0.10 ms

✅ Runtime Async Feature demonstration completed!
```

## 🔍 What the Sample Demonstrates

### Async Workload Testing

The sample runs a controlled async workload consisting of:

1. **100 Concurrent Async Tasks** - Each task performs:
   - `Task.Delay(10)` - Simulates I/O-bound async work
   - `Task.Yield()` - Allows other tasks to proceed
   - Simple computation - Basic CPU work

2. **Performance Measurement** - Times the entire workload and calculates:
   - Total execution time
   - Average time per async task
   - Throughput comparison between enabled/disabled states

### Runtime Async Detection

The sample automatically detects if the Runtime Async feature is enabled by checking the `DOTNET_RuntimeAsync` environment variable and provides clear feedback about the current state.

## 📊 Performance Benefits

When Runtime Async is enabled, you should observe:

- **Reduced latency** for individual async operations
- **Improved throughput** for concurrent async workloads
- **Lower memory allocations** in async state machines
- **Better CPU utilization** in high-concurrency scenarios

The actual performance improvements depend on the workload characteristics and system configuration.

## 🏗️ Sample Architecture

```
RuntimeAsyncFeature/
├── RuntimeAsyncFeature.csproj    # Project configuration with preview features
├── Program.cs                    # Main demonstration code
└── README.md                    # This documentation
```

### Code Structure

- **`Program.Main()`** - Entry point, detects Runtime Async status
- **`RunSimpleAsyncDemo()`** - Executes the async workload demonstration
- **`SimpleAsyncTask()`** - Individual async task implementation

## 💡 Usage Tips

### Enabling Runtime Async

The Runtime Async feature is controlled by the environment variable:

```bash
# Enable Runtime Async
export DOTNET_RuntimeAsync=1
dotnet run

# Or inline
DOTNET_RuntimeAsync=1 dotnet run
```

### Comparing Performance

To compare performance between enabled and disabled states:

```bash
# Baseline (disabled)
time dotnet run

# With Runtime Async (enabled)  
time DOTNET_RuntimeAsync=1 dotnet run
```

### Production Considerations

⚠️ **Important**: Runtime Async is a **preview feature** in .NET 10 RC 1. Consider the following for production use:

- **Thorough testing** - Test your specific workloads with the feature enabled
- **Performance validation** - Measure actual performance improvements in your scenarios
- **Compatibility** - Ensure compatibility with your async patterns and libraries
- **Monitoring** - Monitor application behavior when the feature is enabled

## 🔗 Related Technologies

- **async/await** - C# asynchronous programming model
- **Task Parallel Library (TPL)** - Foundation for async operations
- **ThreadPool** - Underlying thread management
- **.NET 10 Preview Features** - Other experimental .NET 10 capabilities

## 📚 Additional Resources

- [.NET 10 RC 1 Release Notes](https://github.com/dotnet/core/tree/main/release-notes/10.0)
- [Async Programming in C#](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/)
- [Task-based Asynchronous Pattern (TAP)](https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap)

## 🎯 Real-World Applications

Runtime Async optimizations are particularly beneficial for:

- **Web APIs** - High-throughput web services with many concurrent requests
- **Microservices** - Services with extensive async I/O operations
- **Database Applications** - Applications with heavy async database access
- **Real-time Systems** - Systems requiring low-latency async operations
- **Background Services** - Services processing async workloads

## 🤝 Contributing

This sample is part of the .NET 10 Samples Collection. To contribute:

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../../LICENSE) file for details.

---

**Note**: This sample demonstrates the .NET 10 Runtime Async preview feature. The feature is experimental and subject to change in future releases. Always test thoroughly before using in production environments.