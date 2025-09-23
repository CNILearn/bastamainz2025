// class: created on the heap
MyClass myClass = new(3);
myClass.ShowX();

// struct: created on the stack, but can be boxed to the heap
MyStruct myStruct = new(5);  // created on the stack
myStruct.ShowX();
string? s = myStruct.ToString();  // boxing! using the heap!
Console.WriteLine(s);

// ref struct: created on the stack, cannot be boxed to the heap
MyRefStruct myRefStruct = new(7);  // created on the stack
myRefStruct.ShowX();
// string? s2 = myRefStruct.ToString();  // does not compile - no boxing!

int x = MethodUsingADelegate(() => 10);
Console.WriteLine(x);
string s1 = MethodUsingADelegate(() => "Hello");
Console.WriteLine(s1);
int[] ints = MethodUsingADelegate(() =>
{
    int[] arr = [ 1, 2, 3 ];
    return arr;
});
Console.WriteLine(string.Join(", ", ints));

ReadOnlySpan<string?> strings = MethodUsingADelegate(() =>
{
    string[] arr = ["one", "two", "three"];
    return arr; // .AsSpan();  // AsSpan not needed, implicit conversion
});
Console.WriteLine(string.Join(',', strings));

ColorResult cr = new(2, 1);
char[] data = new char[3];
Span<char> cd = data; //.AsSpan();
if (cr.TryFormat(cd, out int written))
{
    Console.WriteLine(written);
    Console.WriteLine(new string(cd));
}
else
{
    Console.WriteLine("parsing failed");
}

// static T MethodUsingADelegate<T>(Func<T> f1) => f1();

// .NET 9 - ref structs can be used as type parameters - requires "where T : allows ref struct"
static T MethodUsingADelegate<T>(Func<T> f1) where T : allows ref struct => f1();  

class MyClass(int x)
{
    private readonly int _x = x;
    public void ShowX()
    {
        Console.WriteLine(_x);
    }
}

readonly struct MyStruct(int x)
{
    public int X { get; } = x;
    public void ShowX()
    {
        Console.WriteLine(X);
    }
}

readonly ref struct MyRefStruct(int x)
{
    public int X { get; } = x;

    public void ShowX()
    {
        Console.WriteLine(X);
    }
}

// ref struct implementing an interface - requires C# 13
public readonly ref struct ColorResult(int correct, int wrongPosition) : ISpanFormattable
{
    private const char Separator = ':';
    public int Correct { get; } = correct;
    public int WrongPosition { get; } = wrongPosition;

    public override string ToString() => ToString(default, default);

    public string ToString(string? format = default, IFormatProvider? formatProvider = default)
    {
        var destination = new char[3].AsSpan();
        if (TryFormat(destination, out _, format.AsSpan(), formatProvider))
        {
            return destination.ToString();
        }
        else
        {
            throw new FormatException();
        }
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = default)
    {
        if (destination.Length < 3)
        {
            charsWritten = 0;
            return false;
        }

        destination[0] = (char)(Correct + '0');
        destination[1] = Separator;
        destination[2] = (char)(WrongPosition + '0');
        charsWritten = 3;
        return true;
    }
}
