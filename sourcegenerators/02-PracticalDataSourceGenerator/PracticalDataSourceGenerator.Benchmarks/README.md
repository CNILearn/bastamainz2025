# Practical Data Source Generator - Performance Benchmarks

This project provides comprehensive performance benchmarking to measure the improvements gained by the caching strategies in Stage 3 compared to the file-based approach in Stage 2.

## Overview

The `Stage2VsStage3Benchmark` class measures performance across multiple scenarios:

### Benchmark Categories

#### SingleRun
Compares single execution performance between Stage 2 and Stage 3:
- **Stage2_FileIntegration_SingleRun**: Baseline performance for file-based generation
- **Stage3_BasicCaching_SingleRun**: Performance with basic caching enabled

#### MultipleRuns  
Simulates multiple consecutive builds to highlight caching benefits:
- **Stage2_FileIntegration_MultipleRuns**: Shows repeated file I/O overhead
- **Stage3_BasicCaching_MultipleRuns**: Demonstrates caching efficiency across builds

#### LargeFiles
Tests performance impact with larger configuration files:
- **Stage2_FileIntegration_LargeConfigFiles**: Baseline with large JSON configs
- **Stage3_BasicCaching_LargeConfigFiles**: Caching benefits with larger datasets

## Running Benchmarks

### Prerequisites
- .NET 9.0 SDK
- BenchmarkDotNet 0.14.0

### Execution
```bash
# Navigate to the benchmark project
cd src/PracticalDataSourceGenerator/PracticalDataSourceGenerator.Benchmarks

# Run all benchmarks
dotnet run -c Release

# Run specific benchmark category
dotnet run -c Release -- --filter "*SingleRun*"
dotnet run -c Release -- --filter "*MultipleRuns*"
dotnet run -c Release -- --filter "*LargeFiles*"
```

### Expected Performance Improvements

**Single Run**: Stage 3 should perform similarly to Stage 2 on first execution
**Multiple Runs**: Stage 3 should significantly outperform Stage 2 due to configuration caching
**Large Files**: Stage 3 should show more pronounced benefits with larger configuration files

## Benchmark Metrics

The benchmark measures:
- **Execution Time**: Method execution duration
- **Memory Allocation**: Memory usage during generation
- **Relative Performance**: Stage 3 performance relative to Stage 2 baseline

## Sample Results Format

```
|                                 Method |      Mean |     Error |    StdDev | Ratio |   Gen0 |   Gen1 | Allocated | Alloc Ratio |
|--------------------------------------- |----------:|----------:|----------:|------:|-------:|-------:|----------:|------------:|
|    Stage2_FileIntegration_SingleRun    | 1.234 ms | 0.012 ms | 0.011 ms |  1.00 |  0.234 |  0.012 |   1.23 KB |        1.00 |
|       Stage3_BasicCaching_SingleRun    | 1.189 ms | 0.009 ms | 0.008 ms |  0.96 |  0.225 |  0.011 |   1.18 KB |        0.96 |
| Stage2_FileIntegration_MultipleRuns    | 6.170 ms | 0.061 ms | 0.057 ms |  1.00 |  1.170 |  0.060 |   6.15 KB |        1.00 |
|    Stage3_BasicCaching_MultipleRuns    | 2.345 ms | 0.023 ms | 0.021 ms |  0.38 |  0.445 |  0.023 |   2.34 KB |        0.38 |
```

## Performance Analysis

The benchmarks help validate:

1. **Caching Effectiveness**: Stage 3 should show significant improvements in multi-run scenarios
2. **Memory Efficiency**: Reduced allocations due to cached configuration objects
3. **Scale Benefits**: Larger configuration files should demonstrate greater caching advantages
4. **Overhead Assessment**: Minimal performance penalty for caching infrastructure

## Technical Details

### Test Data
- **3 Entity Types**: User, Product, Order with realistic properties
- **Complex JSON Configurations**: Multiple template arrays with 10-15 values each
- **Large File Tests**: Auto-generated configurations with 100+ template values

### Measurement Strategy
- Uses BenchmarkDotNet for accurate performance measurement
- Includes memory allocation tracking
- Measures both warm-up and steady-state performance
- Baseline comparisons with statistical analysis

This benchmark suite provides quantitative evidence of the performance improvements achieved through the caching strategies introduced in Stage 3.