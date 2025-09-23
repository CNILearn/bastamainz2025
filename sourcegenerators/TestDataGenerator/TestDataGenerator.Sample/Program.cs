using System;
using System.Collections.Generic;
using System.Linq;
using TestDataGenerator.Attributes;

namespace TestDataGenerator.Sample;

// Sample user class with GenerateTestDataAttribute
[GenerateTestData]
public class User
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime RegisteredOn { get; set; }
    public bool IsActive { get; set; }
    public UserRole Role { get; set; }
}

// Sample class with custom attribute values
[GenerateTestData(StringValue = "Alice", IntRangeMin = 18, IntRangeMax = 99)]
public class CustomUser
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Sample enum
public enum UserRole
{
    Guest,
    User,
    Admin
}

// Sample product class
[GenerateTestData]
public class Product
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public Guid Id { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime? LastUpdated { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== TestDataGenerator Sample Demo ===");
        Console.WriteLine("This demo shows how the source generator creates factory methods for automatic test data generation.");
        Console.WriteLine();

        // Example 1: Basic User test data
        Console.WriteLine("1. Basic User test data:");
        var sampleUser = UserTestDataFactory.CreateSample();
        Console.WriteLine($"   Name: {sampleUser.Name}");
        Console.WriteLine($"   Age: {sampleUser.Age}");
        Console.WriteLine($"   RegisteredOn: {sampleUser.RegisteredOn:yyyy-MM-dd}");
        Console.WriteLine($"   IsActive: {sampleUser.IsActive}");
        Console.WriteLine($"   Role: {sampleUser.Role}");
        Console.WriteLine();

        // Example 2: Multiple users
        Console.WriteLine("2. Multiple Users (first 3 of 5):");
        var users = UserTestDataFactory.CreateMany(5);
        foreach (var user in users.Take(3))
        {
            Console.WriteLine($"   {user.Name} (Age: {user.Age}, Role: {user.Role})");
        }
        Console.WriteLine($"   ... and {users.Count - 3} more");
        Console.WriteLine();

        // Example 3: Custom User with attribute configuration
        Console.WriteLine("3. Custom User with fixed string value and age range:");
        var customUser = CustomUserTestDataFactory.CreateSample();
        Console.WriteLine($"   Name: {customUser.Name} (should be 'Alice')");
        Console.WriteLine($"   Age: {customUser.Age} (should be between 18-99)");
        Console.WriteLine($"   CreatedAt: {customUser.CreatedAt:yyyy-MM-dd}");
        Console.WriteLine();

        // Example 4: Product test data
        Console.WriteLine("4. Product test data:");
        var product = ProductTestDataFactory.CreateSample();
        Console.WriteLine($"   Name: {product.Name}");
        Console.WriteLine($"   Price: ${product.Price:F2}");
        Console.WriteLine($"   Stock: {product.StockQuantity}");
        Console.WriteLine($"   ID: {product.Id}");
        Console.WriteLine($"   Available: {product.IsAvailable}");
        Console.WriteLine($"   Last Updated: {product.LastUpdated?.ToString("yyyy-MM-dd") ?? "null"}");
        Console.WriteLine();

        // Example 5: Multiple products
        Console.WriteLine("5. Multiple Products:");
        var products = ProductTestDataFactory.CreateMany(3);
        foreach (var p in products)
        {
            Console.WriteLine($"   {p.Name}: ${p.Price:F2} (Stock: {p.StockQuantity})");
        }
        Console.WriteLine();

        Console.WriteLine("Demo completed! The source generator created factory methods that automatically");
        Console.WriteLine("populate test data for your classes and records.");
        Console.WriteLine();
        Console.WriteLine("Generated factory methods available:");
        Console.WriteLine("- UserTestDataFactory.CreateSample()");
        Console.WriteLine("- UserTestDataFactory.CreateMany(count)");
        Console.WriteLine("- CustomUserTestDataFactory.CreateSample()");
        Console.WriteLine("- CustomUserTestDataFactory.CreateMany(count)");
        Console.WriteLine("- ProductTestDataFactory.CreateSample()");
        Console.WriteLine("- ProductTestDataFactory.CreateMany(count)");
    }
}