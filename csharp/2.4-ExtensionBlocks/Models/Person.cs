namespace ExtensionBlocks.Models;

/// <summary>
/// Represents a person demonstrating C# 14 extension properties.
/// Extension properties allow adding computed properties to existing types.
/// 
/// NOTE: Extension properties are a C# 14/.NET 10 feature.
/// This sample shows the concept - extension properties will work in .NET 10 with C# 14.
/// </summary>
public class Person
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public double HeightInMeters { get; init; }
    public double WeightInKg { get; init; }
}
