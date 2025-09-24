# TestDataGenerator

A C# source generator that automatically creates factory methods for populating classes and records with test data. Perfect for unit testing, demos, and development scenarios where you need sample data.

## Features

- **Automatic Factory Generation**: Classes marked with `[GenerateTestData]` get factory methods
- **Support for Basic Types**: int, string, bool, DateTime, enums, Guid, decimal, nullable types
- **Custom Value Configuration**: Set fixed values, ranges, and custom generation options
- **External Type Support**: Configure test data for types you can't modify
- **Random Data Generation**: Generates varied, realistic test data
- **Collection Support**: Create single instances or collections with `CreateMany()`
- **Zero Runtime Dependencies**: Pure compile-time generation with no runtime overhead
- **Modern C# Support**: Uses latest .NET and C# language features

## Usage

### Basic Example

```csharp
using TestDataGenerator.Attributes;

[GenerateTestData]
public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
    public DateTime RegisteredOn { get; set; }
    public bool IsActive { get; set; }
}

// Usage
var user = UserTestDataFactory.CreateSample();
var users = UserTestDataFactory.CreateMany(10);
```

### Custom Configuration

```csharp
[GenerateTestData(StringValue = "Alice", IntRangeMin = 18, IntRangeMax = 99)]
public class CustomUser
{
    public string Name { get; set; }    // Will be "Alice"
    public int Age { get; set; }        // Will be between 18-99
    public DateTime CreatedAt { get; set; }
}
```

### External Type Configuration

Configure types you can't modify (e.g., from external libraries):

```csharp
// Configure types you can't modify (e.g., from external libraries)
TestDataRegistry.Register(options =>
{
    options.ForType<ExternalLibrary.Person>()
           .WithProperty(p => p.Name, "Generated Name")
           .WithProperty(p => p.Age, (20, 40));
});
```

## Generated Code

The generator creates factory classes with static methods:

```csharp
// Auto-generated
public static class UserTestDataFactory
{
    private static readonly Random _random = new();

    public static User CreateSample()
    {
        return new User
        {
            Name = "SampleName",
            Age = _random.Next(1, 100),
            RegisteredOn = DateTime.Now.AddDays(_random.Next(-365, 365)),
            IsActive = _random.Next(2) == 0
        };
    }

    public static List<User> CreateMany(int count = 10)
    {
        var items = new List<User>();
        for (int i = 0; i < count; i++)
        {
            items.Add(CreateSample());
        }
        return items;
    }
}
```

## Attribute Configuration

### GenerateTestDataAttribute Properties

- **StringValue**: Fixed string value for string properties
- **IntRangeMin**: Minimum value for integer range generation
- **IntRangeMax**: Maximum value for integer range generation

### Example with All Options

```csharp
[GenerateTestData(
    StringValue = "DefaultName",
    IntRangeMin = 1,
    IntRangeMax = 100
)]
public class ConfiguredUser
{
    public string Name { get; set; }
    public int Score { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

## Supported Types

The generator supports the following types out of the box:

- **Primitive Types**: `int`, `string`, `bool`, `decimal`, `float`, `double`
- **Date/Time**: `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly`
- **Identifiers**: `Guid`
- **Nullable Types**: `int?`, `DateTime?`, etc.
- **Enums**: All enum types
- **Collections**: Automatically creates collections when using `CreateMany()`

## Advanced Configuration

### Property-Specific Configuration

```csharp
TestDataRegistry.Register(options =>
{
    options.ForType<Product>()
           .WithProperty(p => p.Name, "Sample Product")
           .WithProperty(p => p.Price, (10.0m, 1000.0m))
           .WithProperty(p => p.IsActive, true);
});
```

### Range Configuration

```csharp
TestDataRegistry.Register(options =>
{
    options.ForType<Order>()
           .WithProperty(o => o.Quantity, (1, 50))
           .WithProperty(o => o.Total, (0.01m, 9999.99m));
});
```

## Installation

1. Add the source generator to your test project:

```xml
<ItemGroup>
  <ProjectReference Include="path/to/TestDataGenerator.csproj" 
                    OutputItemType="Analyzer" 
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

2. Mark your classes with the `[GenerateTestData]` attribute
3. Use the generated factory methods in your tests

## Usage in Unit Tests

```csharp
[Test]
public void TestUserProcessing()
{
    // Arrange
    var testUser = UserTestDataFactory.CreateSample();
    var testUsers = UserTestDataFactory.CreateMany(5);
    
    // Act
    var result = userService.ProcessUser(testUser);
    var batchResult = userService.ProcessUsers(testUsers);
    
    // Assert
    Assert.IsNotNull(result);
    Assert.AreEqual(5, batchResult.Count);
}
```

## Requirements

- **.NET 9.0** or later
- **C# 12.0** or later
- **MSBuild-based projects** (.NET SDK style)

## Benefits

- **Reduced Test Setup**: No manual test data creation
- **Consistent Data**: Standardized test data generation
- **Time Saving**: Automatic factory method generation
- **Maintainable**: Easy to update test data configurations
- **Flexible**: Support for custom ranges and values
- **External Types**: Can configure types you don't own

## Examples Repository

Check out the [TestDataGenerator.Sample](TestDataGenerator.Sample/) project for complete working examples demonstrating all features.

---

Part of the [sourcegenerators-samples](../../README.md) repository demonstrating advanced C# source generator techniques.