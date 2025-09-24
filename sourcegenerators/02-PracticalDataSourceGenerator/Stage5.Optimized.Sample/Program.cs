using Stage5.Optimized.Attributes;

namespace Stage5.Optimized.Sample;

[DataSource(EntityName = "User", Count = 10)]
public class User
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

[DataSource(EntityName = "Product", Count = 15)]
public class Product
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
}

[DataSource(EntityName = "Order", Count = 20)]
public class Order
{
    public string OrderId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Stage 5: Optimized Data Source Generator Demo ===");
        Console.WriteLine();

        // Display generator information
        Console.WriteLine($"Generator Info: {UserDataGenerator.GetGeneratorInfo()}");
        Console.WriteLine($"Cache Performance: {UserDataGenerator.GetCachePerformanceMetrics()}");
        Console.WriteLine();

        // Generate and display Users
        Console.WriteLine("=== Generated Users ===");
        var users = UserDataGenerator.GenerateData(5);
        foreach (var user in users)
        {
            Console.WriteLine($"User: {user.Name} ({user.Email}) - Age: {user.Age}, Active: {user.IsActive}");
        }
        Console.WriteLine($"Configuration: {UserDataGenerator.GetConfigurationInfo()}");
        Console.WriteLine();

        // Generate and display Products
        Console.WriteLine("=== Generated Products ===");
        var products = ProductDataGenerator.GenerateData(5);
        foreach (var product in products)
        {
            Console.WriteLine($"Product: {product.Name} - ${product.Price:F2} ({product.Category}) - Stock: {product.StockQuantity}");
        }
        Console.WriteLine($"Configuration: {ProductDataGenerator.GetConfigurationInfo()}");
        Console.WriteLine();

        // Generate and display Orders
        Console.WriteLine("=== Generated Orders ===");
        var orders = OrderDataGenerator.GenerateData(5);
        foreach (var order in orders)
        {
            Console.WriteLine($"Order: {order.OrderId} - Status: {order.Status}, Total: ${order.TotalAmount:F2}");
        }
        Console.WriteLine($"Configuration: {OrderDataGenerator.GetConfigurationInfo()}");
        Console.WriteLine();

        // Display cache statistics
        Console.WriteLine("=== Optimized Cache Statistics ===");
        Console.WriteLine($"User Cache Stats: {UserDataGenerator.GetAdvancedCachingStats()}");
        Console.WriteLine($"Product Cache Stats: {ProductDataGenerator.GetAdvancedCachingStats()}");
        Console.WriteLine($"Order Cache Stats: {OrderDataGenerator.GetAdvancedCachingStats()}");
        Console.WriteLine();

        Console.WriteLine("Demo completed. Press any key to exit...");
        Console.ReadKey();
    }
}