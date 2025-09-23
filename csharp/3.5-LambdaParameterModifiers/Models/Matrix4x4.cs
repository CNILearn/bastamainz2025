using System.Runtime.InteropServices;

namespace LambdaParameterModifiers.Models;

/// <summary>
/// Matrix for advanced linear algebra operations with span-based memory access
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Matrix4x4
{
    public float M11, M12, M13, M14;
    public float M21, M22, M23, M24;
    public float M31, M32, M33, M34;
    public float M41, M42, M43, M44;

    public static Matrix4x4 Identity => new()
    {
        M11 = 1, M22 = 1, M33 = 1, M44 = 1
    };

    /// <summary>
    /// Get a span representing the matrix data for direct memory access
    /// </summary>
    public unsafe Span<float> AsSpan()
    {
        fixed (float* ptr = &M11)
        {
            return new Span<float>(ptr, 16);
        }
    }

    public override readonly string ToString() => 
        $"[{M11:F2}, {M12:F2}, {M13:F2}, {M14:F2}]\n" +
        $"[{M21:F2}, {M22:F2}, {M23:F2}, {M24:F2}]\n" +
        $"[{M31:F2}, {M32:F2}, {M33:F2}, {M34:F2}]\n" +
        $"[{M41:F2}, {M42:F2}, {M43:F2}, {M44:F2}]";
}
