namespace System.Runtime.CompilerServices;

// TODO: switch to https://github.com/dotnet/roslyn/issues/72133 with a source generator
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
sealed class InterceptsLocationAttribute(string filePath, int line, int column) : Attribute
{
}
