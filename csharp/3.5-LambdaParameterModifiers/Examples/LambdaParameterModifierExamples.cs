using LambdaParameterModifiers.Models;

namespace LambdaParameterModifiers.Examples;

/// <summary>
/// Demonstrates C# 14 lambda parameter modifiers: ref, in, and out
/// These examples showcase low-level memory manipulation scenarios
/// </summary>
public static class LambdaParameterModifierExamples
{
    /// <summary>
    /// Example 1: Lambda with 'ref' parameter modifier for direct memory modification
    /// Useful for scenarios where you need to modify large structures without copying
    /// </summary>
    public static void DemonstrateRefModifier()
    {
        Console.WriteLine("🔄 Lambda 'ref' Parameter Modifier Examples");
        Console.WriteLine("============================================");

        // Example 1a: Direct vector modification using ref
        Vector3D[] vectors =
        [
            new(1.0f, 2.0f, 3.0f),
            new(4.0f, 5.0f, 6.0f),
            new(7.0f, 8.0f, 9.0f)
        ];

        Console.WriteLine("\n📐 Vector Normalization with 'ref' modifier:");
        Console.WriteLine("Before normalization:");
        foreach (var v in vectors)
            Console.WriteLine($"  {v} (magnitude: {v.Magnitude:F2})");

        // C# 14: Lambda with ref parameter - modifies vectors in-place without copying
        var normalizeVector = (ref Vector3D vector) =>
        {
            var magnitude = vector.Magnitude;
            if (magnitude > 0)
            {
                vector.X /= magnitude;
                vector.Y /= magnitude;
                vector.Z /= magnitude;
            }
        };

        // Apply normalization in-place
        for (int i = 0; i < vectors.Length; i++)
        {
            normalizeVector(ref vectors[i]);
        }

        Console.WriteLine("\nAfter normalization (in-place modification):");
        foreach (var v in vectors)
            Console.WriteLine($"  {v} (magnitude: {v.Magnitude:F2})");

        // Example 1b: Matrix transformation with ref
        Console.WriteLine("\n🔢 Matrix Scaling with 'ref' modifier:");
        var matrix = Matrix4x4.Identity;
        Console.WriteLine($"Original matrix:\n{matrix}");

        var scaleMatrix = (ref Matrix4x4 m, float scale) =>
        {
            Span<float> span = m.AsSpan();
            for (int i = 0; i < span.Length; i++)
            {
                span[i] *= scale;
            }
        };

        scaleMatrix(ref matrix, 2.0f);
        Console.WriteLine($"\nScaled matrix (scale: 2.0):\n{matrix}");
    }

    /// <summary>
    /// Example 2: Lambda with 'in' parameter modifier for read-only large struct access
    /// Avoids copying large structures while ensuring they remain immutable
    /// </summary>
    public static void DemonstrateInModifier()
    {
        Console.WriteLine("\n📖 Lambda 'in' Parameter Modifier Examples");
        Console.WriteLine("==========================================");

        // Example 2a: Processing large data blocks without copying
        var dataBlocks = new LargeDataBlock[3];
        for (int i = 0; i < dataBlocks.Length; i++)
        {
            dataBlocks[i].Fill((byte)(i * 50 + 10));
        }

        Console.WriteLine("\n💾 Large Data Block Analysis with 'in' modifier:");

        // C# 14: Lambda with in parameter - reads large struct without copying
        var analyzeDataBlock = (in LargeDataBlock block, int index) =>
        {
            var span = block.AsSpan();
            var sum = 0L;
            var min = byte.MaxValue;
            var max = byte.MinValue;

            foreach (var b in span)
            {
                sum += b;
                if (b < min) min = b;
                if (b > max) max = b;
            }

            var average = sum / (double)span.Length;
            Console.WriteLine($"  Block {index}: Size={span.Length}B, Avg={average:F1}, Min={min}, Max={max}");
            
            return new { Sum = sum, Average = average, Min = min, Max = max };
        };

        for (int i = 0; i < dataBlocks.Length; i++)
        {
            analyzeDataBlock(in dataBlocks[i], i);
        }

        // Example 2b: Vector distance calculation without copying
        Console.WriteLine("\n📏 Vector Distance Calculation with 'in' modifier:");
        Vector3D[] points =
        [
            new(0, 0, 0),
            new(3, 4, 0),
            new(1, 1, 1)
        ];

        var calculateDistance = (in Vector3D p1, in Vector3D p2) =>
        {
            var dx = p1.X - p2.X;
            var dy = p1.Y - p2.Y;
            var dz = p1.Z - p2.Z;
            return MathF.Sqrt(dx * dx + dy * dy + dz * dz);
        };

        for (int i = 0; i < points.Length; i++)
        {
            for (int j = i + 1; j < points.Length; j++)
            {
                var distance = calculateDistance(in points[i], in points[j]);
                Console.WriteLine($"  Distance {points[i]} -> {points[j]}: {distance:F2}");
            }
        }
    }

    /// <summary>
    /// Example 3: Lambda with 'out' parameter modifier for efficient result generation
    /// Useful for operations that need to return multiple values or large structures
    /// </summary>
    public static void DemonstrateOutModifier()
    {
        Console.WriteLine("\n📤 Lambda 'out' Parameter Modifier Examples");
        Console.WriteLine("===========================================");

        // Example 3a: Vector decomposition with out parameters
        Console.WriteLine("\n🔍 Vector Decomposition with 'out' modifier:");
        Vector3D[] vectors =
        [
            new(3.0f, 4.0f, 5.0f),
            new(-2.0f, 1.0f, -3.0f),
            new(0.0f, 0.0f, 0.0f)
        ];

        // C# 14: Lambda with out parameters - efficiently returns multiple results
        var decomposeVector = (Vector3D input, out float magnitude, out Vector3D normalized, out bool isZero) =>
        {
            magnitude = input.Magnitude;
            isZero = magnitude < float.Epsilon;
            
            if (isZero)
            {
                normalized = new Vector3D(0, 0, 0);
            }
            else
            {
                normalized = new Vector3D(
                    input.X / magnitude,
                    input.Y / magnitude,
                    input.Z / magnitude
                );
            }
        };

        foreach (var vector in vectors)
        {
            decomposeVector(vector, out var mag, out var norm, out var zero);
            Console.WriteLine($"  Vector: {vector}");
            Console.WriteLine($"    Magnitude: {mag:F2}");
            Console.WriteLine($"    Normalized: {norm}");
            Console.WriteLine($"    Is Zero: {zero}");
            Console.WriteLine();
        }

        // Example 3b: Matrix operations with multiple outputs
        Console.WriteLine("🔢 Matrix Operations with 'out' modifier:");
        var matrices = new Matrix4x4[] { Matrix4x4.Identity };

        var analyzeMatrix = (in Matrix4x4 matrix, out float determinant, out float trace, out bool isIdentity) =>
        {
            var span = matrix.AsSpan();
            
            // Calculate trace (sum of diagonal elements)
            trace = matrix.M11 + matrix.M22 + matrix.M33 + matrix.M44;
            
            // Simplified determinant for demonstration (not full 4x4 calculation)
            determinant = matrix.M11 * matrix.M22 * matrix.M33 * matrix.M44;
            
            // Check if identity matrix
            isIdentity = Math.Abs(matrix.M11 - 1.0f) < float.Epsilon &&
                        Math.Abs(matrix.M22 - 1.0f) < float.Epsilon &&
                        Math.Abs(matrix.M33 - 1.0f) < float.Epsilon &&
                        Math.Abs(matrix.M44 - 1.0f) < float.Epsilon &&
                        Math.Abs(matrix.M12) < float.Epsilon &&
                        Math.Abs(matrix.M13) < float.Epsilon;
        };

        foreach (var matrix in matrices)
        {
            analyzeMatrix(in matrix, out var det, out var tr, out var isId);
            Console.WriteLine($"  Matrix Analysis:");
            Console.WriteLine($"    Determinant: {det:F2}");
            Console.WriteLine($"    Trace: {tr:F2}");
            Console.WriteLine($"    Is Identity: {isId}");
        }
    }

    /// <summary>
    /// Example 4: Advanced scenario combining all modifiers
    /// Demonstrates complex memory manipulation with optimal performance
    /// </summary>
    public static unsafe void DemonstrateAdvancedScenarios()
    {
        Console.WriteLine("\n⚡ Advanced Lambda Parameter Modifier Scenarios");
        Console.WriteLine("==============================================");

        Console.WriteLine("\n🧮 High-Performance Matrix-Vector Operations:");

        // Setup test data
        var matrices = new Matrix4x4[1000];
        var vectors = new Vector3D[1000];
        var results = new Vector3D[1000];

        // Initialize with random-ish data
        for (int i = 0; i < 1000; i++)
        {
            matrices[i] = Matrix4x4.Identity;
            matrices[i].M11 = 1.0f + i * 0.001f;
            matrices[i].M22 = 1.0f + i * 0.002f;
            matrices[i].M33 = 1.0f + i * 0.003f;
            
            vectors[i] = new Vector3D(i * 0.1f, i * 0.2f, i * 0.3f);
        }

        // C# 14: Lambda combining ref (for result), in (for readonly inputs)
        var transformVector = (in Matrix4x4 matrix, in Vector3D vector, ref Vector3D result) =>
        {
            result.X = matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Z;
            result.Y = matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Z;
            result.Z = matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Z;
        };

        var startTime = DateTime.UtcNow;
        
        // Perform transformations
        for (int i = 0; i < 1000; i++)
        {
            transformVector(in matrices[i], in vectors[i], ref results[i]);
        }
        
        var elapsed = DateTime.UtcNow - startTime;
        Console.WriteLine($"  Transformed 1000 vectors in {elapsed.TotalMicroseconds:F1} μs");
        Console.WriteLine($"  Average per operation: {elapsed.TotalNanoseconds / 1000:F1} ns");

        // Show some sample results
        Console.WriteLine("\n  Sample transformations:");
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine($"    Input: {vectors[i]} -> Output: {results[i]}");
        }

        // Example with span operations and lambda parameter modifiers
        Console.WriteLine("\n🎯 Span-based Memory Operations:");
        
        var sourceData = new float[10000];
        var destData = new float[10000];
        
        // Fill source with test data
        for (int i = 0; i < sourceData.Length; i++)
        {
            sourceData[i] = MathF.Sin(i * 0.001f) * 100.0f;
        }

        // C# 14: Lambda with spans for zero-copy operations
        var processSpan = (ReadOnlySpan<float> source, Span<float> destination, float multiplier) =>
        {
            for (int i = 0; i < source.Length; i++)
            {
                destination[i] = source[i] * multiplier + MathF.Cos(i * 0.001f);
            }
        };

        var sourceSpan = sourceData.AsSpan();
        var destSpan = destData.AsSpan();
        
        startTime = DateTime.UtcNow;
        processSpan(sourceSpan, destSpan, 1.5f);
        elapsed = DateTime.UtcNow - startTime;
        
        Console.WriteLine($"  Processed {sourceData.Length} elements in {elapsed.TotalMicroseconds:F1} μs");
        Console.WriteLine($"  Throughput: {sourceData.Length / elapsed.TotalMilliseconds:F0} elements/ms");
        
        // Show sample results
        Console.WriteLine("\n  Sample processed values:");
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine($"    Input: {sourceData[i]:F2} -> Output: {destData[i]:F2}");
        }
    }
}