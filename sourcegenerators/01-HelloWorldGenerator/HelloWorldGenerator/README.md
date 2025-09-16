# HelloWorld Source Generator

A simple C# source generator that automatically generates HelloWorld and type introspection methods in your projects.

## Features

- **HelloWorld Method**: Generates a static method that returns "Hello, World!"
- **Type Introspection**: Generates a method that displays all accessible types and methods in the compilation context

## Usage

1. Add the source generator to your project:

```xml
<ItemGroup>
  <PackageReference Include="HelloWorldGenerator" Version="1.0.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
  </PackageReference>
</ItemGroup>
```

2. The generator will automatically create the methods:
   - `GeneratedHelloWorld.SayHello()` - Returns "Hello, World!"
   - `GeneratedHelloWorld.GetAvailableTypesAndMethods()` - Returns compilation information

## Generated Code Example

```csharp
public static class GeneratedHelloWorld
{
    public static string SayHello()
    {
        return "Hello, World!";
    }
    
    public static string GetAvailableTypesAndMethods()
    {
        // Returns information about available types and methods
    }
}
```