using System.Runtime.InteropServices;

namespace LambdaParameterModifiers.Models;

/// <summary>
/// Represents a high-performance vector operation for memory-intensive calculations
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Vector3D(float x, float y, float z)
{
    public float X = x;
    public float Y = y;
    public float Z = z;

    public readonly float Magnitude => MathF.Sqrt(X * X + Y * Y + Z * Z);

    public override readonly string ToString() => $"({X:F2}, {Y:F2}, {Z:F2})";
}
