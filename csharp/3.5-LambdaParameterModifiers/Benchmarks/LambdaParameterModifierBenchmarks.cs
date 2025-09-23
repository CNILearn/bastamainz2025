using BenchmarkDotNet.Attributes;

using LambdaParameterModifiers.Models;

namespace LambdaParameterModifiers.Benchmarks;

/// <summary>
/// Benchmarks comparing lambda parameter modifiers performance
/// Demonstrates the impact of ref, in, and out modifiers on memory operations
/// </summary>
[MemoryDiagnoser]
[SimpleJob]
public class LambdaParameterModifierBenchmarks
{
    private Vector3D[] _vectors = null!;
    private Matrix4x4[] _matrices = null!;
    private LargeDataBlock[] _dataBlocks = null!;
    private float[] _floatData = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Initialize test data
        _vectors = new Vector3D[10000];
        _matrices = new Matrix4x4[1000];
        _dataBlocks = new LargeDataBlock[100];
        _floatData = new float[50000];

        for (int i = 0; i < _vectors.Length; i++)
        {
            _vectors[i] = new Vector3D(i * 0.1f, i * 0.2f, i * 0.3f);
        }

        for (int i = 0; i < _matrices.Length; i++)
        {
            _matrices[i] = Matrix4x4.Identity;
            _matrices[i].M11 = 1.0f + i * 0.001f;
        }

        for (int i = 0; i < _dataBlocks.Length; i++)
        {
            _dataBlocks[i].Fill((byte)(i % 256));
        }

        for (int i = 0; i < _floatData.Length; i++)
        {
            _floatData[i] = MathF.Sin(i * 0.001f);
        }
    }

    /// <summary>
    /// Benchmark: Vector normalization by value (creates copies)
    /// </summary>
    [Benchmark(Baseline = true)]
    public void VectorNormalization_ByValue()
    {
        var normalize = (Vector3D vector) =>
        {
            var magnitude = vector.Magnitude;
            if (magnitude > 0)
            {
                return new Vector3D(
                    vector.X / magnitude,
                    vector.Y / magnitude,
                    vector.Z / magnitude
                );
            }
            return vector;
        };

        for (int i = 0; i < _vectors.Length; i++)
        {
            _vectors[i] = normalize(_vectors[i]);
        }
    }

    /// <summary>
    /// Benchmark: Vector normalization with ref modifier (in-place modification)
    /// </summary>
    [Benchmark]
    public void VectorNormalization_WithRef()
    {
        var normalize = (ref Vector3D vector) =>
        {
            var magnitude = vector.Magnitude;
            if (magnitude > 0)
            {
                vector.X /= magnitude;
                vector.Y /= magnitude;
                vector.Z /= magnitude;
            }
        };

        for (int i = 0; i < _vectors.Length; i++)
        {
            normalize(ref _vectors[i]);
        }
    }

    /// <summary>
    /// Benchmark: Matrix analysis by value (copies large structure)
    /// </summary>
    [Benchmark]
    public float MatrixAnalysis_ByValue()
    {
        var calculateTrace = (Matrix4x4 matrix) =>
        {
            return matrix.M11 + matrix.M22 + matrix.M33 + matrix.M44;
        };

        float totalTrace = 0;
        for (int i = 0; i < _matrices.Length; i++)
        {
            totalTrace += calculateTrace(_matrices[i]);
        }
        return totalTrace;
    }

    /// <summary>
    /// Benchmark: Matrix analysis with in modifier (avoids copying)
    /// </summary>
    [Benchmark]
    public float MatrixAnalysis_WithIn()
    {
        var calculateTrace = (in Matrix4x4 matrix) =>
        {
            return matrix.M11 + matrix.M22 + matrix.M33 + matrix.M44;
        };

        float totalTrace = 0;
        for (int i = 0; i < _matrices.Length; i++)
        {
            totalTrace += calculateTrace(in _matrices[i]);
        }
        return totalTrace;
    }

    /// <summary>
    /// Benchmark: Data block processing by value (expensive copy)
    /// </summary>
    [Benchmark]
    public long DataBlockProcessing_ByValue()
    {
        var processBlock = (LargeDataBlock block) =>
        {
            var span = block.AsSpan();
            long sum = 0;
            foreach (var b in span)
            {
                sum += b;
            }
            return sum;
        };

        long totalSum = 0;
        for (int i = 0; i < _dataBlocks.Length; i++)
        {
            totalSum += processBlock(_dataBlocks[i]);
        }
        return totalSum;
    }

    /// <summary>
    /// Benchmark: Data block processing with in modifier (no copy)
    /// </summary>
    [Benchmark]
    public long DataBlockProcessing_WithIn()
    {
        var processBlock = (in LargeDataBlock block) =>
        {
            var span = block.AsSpan();
            long sum = 0;
            foreach (var b in span)
            {
                sum += b;
            }
            return sum;
        };

        long totalSum = 0;
        for (int i = 0; i < _dataBlocks.Length; i++)
        {
            totalSum += processBlock(in _dataBlocks[i]);
        }
        return totalSum;
    }

    /// <summary>
    /// Benchmark: Vector operations returning multiple values with tuples
    /// </summary>
    [Benchmark]
    public int VectorDecomposition_WithTuples()
    {
        var decompose = (Vector3D vector) =>
        {
            var magnitude = vector.Magnitude;
            var normalized = magnitude > 0 ? 
                new Vector3D(vector.X / magnitude, vector.Y / magnitude, vector.Z / magnitude) :
                new Vector3D(0, 0, 0);
            var isZero = magnitude < float.Epsilon;
            return (magnitude, normalized, isZero);
        };

        int count = 0;
        for (int i = 0; i < _vectors.Length; i++)
        {
            var (mag, norm, zero) = decompose(_vectors[i]);
            if (!zero) count++;
        }
        return count;
    }

    /// <summary>
    /// Benchmark: Vector operations with out parameters (more efficient)
    /// </summary>
    [Benchmark]
    public int VectorDecomposition_WithOut()
    {
        var decompose = (Vector3D vector, out float magnitude, out Vector3D normalized, out bool isZero) =>
        {
            magnitude = vector.Magnitude;
            isZero = magnitude < float.Epsilon;
            
            if (isZero)
            {
                normalized = new Vector3D(0, 0, 0);
            }
            else
            {
                normalized = new Vector3D(
                    vector.X / magnitude,
                    vector.Y / magnitude,
                    vector.Z / magnitude
                );
            }
        };

        int count = 0;
        for (int i = 0; i < _vectors.Length; i++)
        {
            decompose(_vectors[i], out _, out _, out var zero);
            if (!zero) count++;
        }
        return count;
    }

    /// <summary>
    /// Benchmark: Memory copy operations with traditional array indexing
    /// </summary>
    [Benchmark]
    public void MemoryCopy_Traditional()
    {
        var copyAndTransform = (float[] source, float[] destination, float multiplier) =>
        {
            for (int i = 0; i < source.Length; i++)
            {
                destination[i] = source[i] * multiplier;
            }
        };

        var temp = new float[_floatData.Length];
        copyAndTransform(_floatData, temp, 1.5f);
    }

    /// <summary>
    /// Benchmark: Memory copy operations with span and ref modifiers
    /// </summary>
    [Benchmark]
    public void MemoryCopy_WithSpanAndRef()
    {
        var copyAndTransform = (ReadOnlySpan<float> source, Span<float> destination, float multiplier) =>
        {
            for (int i = 0; i < source.Length; i++)
            {
                destination[i] = source[i] * multiplier;
            }
        };

        var temp = new float[_floatData.Length];
        var sourceSpan = _floatData.AsSpan();
        var destSpan = temp.AsSpan();
        copyAndTransform(sourceSpan, destSpan, 1.5f);
    }

    /// <summary>
    /// Benchmark: Complex lambda with multiple parameter modifiers
    /// </summary>
    [Benchmark]
    public void ComplexOperation_WithMixedModifiers()
    {
        var complexOperation = (in Vector3D input, ref Vector3D output, out float magnitude, float scale) =>
        {
            magnitude = input.Magnitude;
            output.X = input.X * scale;
            output.Y = input.Y * scale;
            output.Z = input.Z * scale;
        };

        var results = new Vector3D[_vectors.Length];
        for (int i = 0; i < _vectors.Length; i++)
        {
            complexOperation(in _vectors[i], ref results[i], out _, 2.0f);
        }
    }
}

/// <summary>
/// Static class to run the benchmarks
/// </summary>
public static class BenchmarkRunner
{
    public static void RunBenchmarks()
    {
        Console.WriteLine("🏁 Running Lambda Parameter Modifier Benchmarks");
        Console.WriteLine("===============================================");
        Console.WriteLine("This may take several minutes...\n");

        var summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<LambdaParameterModifierBenchmarks>();
        
        Console.WriteLine("\n📊 Benchmark Summary:");
        Console.WriteLine("====================");
        Console.WriteLine("Key insights:");
        Console.WriteLine("• 'ref' modifiers eliminate copying for in-place modifications");
        Console.WriteLine("• 'in' modifiers prevent copying large readonly structures");
        Console.WriteLine("• 'out' modifiers can be more efficient than returning tuples");
        Console.WriteLine("• Span-based operations with modifiers optimize memory access");
        Console.WriteLine("\nSee detailed results above for specific performance metrics.");
    }
}

// BenchmarkDotNet lifecycle:
// 1. Constructor (object creation)
// 2. [GlobalSetup] (one-time setup - EXCLUDED from timing)
// 3. [Benchmark] methods (measured)
// 4. [GlobalCleanup] (cleanup - EXCLUDED from timing)