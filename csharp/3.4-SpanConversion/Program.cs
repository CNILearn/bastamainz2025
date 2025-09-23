int[] data = [1, 2, 3, 4, 5, 6, 7, 8];

ChangeUsingSpan(data[2..^1]);  // range creates a copy, not a span
ChangeUsingSpan(data.AsSpan()[2..^1]); // range access on a span creates a span
Console.WriteLine();

ReadOnlySpan<int> dataSpan = data;

int[] arr = [1, 2, 3];

Console.WriteLine(
    arr.StartsWith(1) // CS8773 - requires a receiver of ReadOnlySpan<T> in C# 13, permitted with C# 14
    );


void ChangeUsingSpan(Span<int> data)
{
    for (int i = 0; i < data.Length; i++)
    {
        data[i] += 2;
    }
}

public static class MemoryExtensions
{
    //public static bool StartsWith<T>(this T[] arr, T value) where T : IEquatable<T>
    // => arr.Length != 0 && EqualityComparer<T>.Default.Equals(arr[0], value);

    public static bool StartsWith<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T> 
        => span.Length != 0 && EqualityComparer<T>.Default.Equals(span[0], value);
}