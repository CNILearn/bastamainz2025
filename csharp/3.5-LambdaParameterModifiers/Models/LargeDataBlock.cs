using System.Runtime.InteropServices;

namespace LambdaParameterModifiers.Models;

/// <summary>
/// Large data structure for demonstrating memory copy performance
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct LargeDataBlock
{
    public const int Size = 1024; // 1KB of data
    
    public unsafe fixed byte Data[Size];

    public unsafe Span<byte> AsSpan()
    {
        fixed (byte* ptr = Data)
        {
            return new Span<byte>(ptr, Size);
        }
    }

    public unsafe void Fill(byte value)
    {
        AsSpan().Fill(value);
    }
}