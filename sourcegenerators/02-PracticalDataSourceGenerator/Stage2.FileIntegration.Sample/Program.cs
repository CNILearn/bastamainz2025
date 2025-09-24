using Stage2.FileIntegration.Sample;

Console.WriteLine("=== Stage 2: File Integration Data Source Generator Demo ===");
Console.WriteLine();

// Display generator information
Console.WriteLine("Generator Info:");
Console.WriteLine(UserDataFactory.GetGeneratorInfo());
Console.WriteLine();

Console.WriteLine("Generated Users (with external configuration):");
Console.WriteLine("==============================================");

var users = UserDataFactory.CreateSamples();
foreach (var user in users)
{
    Console.WriteLine($"Name: {user.Name}, Email: {user.Email}, Age: {user.Age}, Role: {user.Role}, Active: {user.IsActive}");
}

Console.WriteLine();
Console.WriteLine("User Configuration Info:");
Console.WriteLine(UserDataFactory.GetConfigurationInfo());
Console.WriteLine();

Console.WriteLine("Generated Products (with external configuration):");
Console.WriteLine("===============================================");

var products = ProductDataFactory.CreateSamples();
foreach (var product in products)
{
    Console.WriteLine($"Product: {product.Name}");
    Console.WriteLine($"  Description: {product.Description}");
    Console.WriteLine($"  Price: ${product.Price:F2}, Stock: {product.StockQuantity}, Category: {product.Category}");
    Console.WriteLine();
}

Console.WriteLine("Product Configuration Info:");
Console.WriteLine(ProductDataFactory.GetConfigurationInfo());
Console.WriteLine();

Console.WriteLine("Generated Orders (fallback to basic generation):");
Console.WriteLine("===============================================");

var orders = OrderDataFactory.CreateSamples();
foreach (var order in orders)
{
    Console.WriteLine($"Order: {order.OrderNumber}, Total: ${order.Total:F2}, Date: {order.OrderDate:yyyy-MM-dd}, Processed: {order.IsProcessed}");
}

Console.WriteLine();
Console.WriteLine("Order Configuration Info:");
Console.WriteLine(OrderDataFactory.GetConfigurationInfo());
Console.WriteLine();

Console.WriteLine("Stage 2 Characteristics:");
Console.WriteLine("- Multiple data sources: Attributes + JSON configuration files");
Console.WriteLine("- External file integration: JSON templates for realistic data");
Console.WriteLine("- Graceful fallback: Basic generation when no external config found");
Console.WriteLine("- Still no caching: File operations performed every build");

Console.WriteLine();
Console.WriteLine("Performance Note: Stage 2 reads and parses JSON files during every");
Console.WriteLine("compilation. Stage 3 will introduce caching to improve build performance.");
