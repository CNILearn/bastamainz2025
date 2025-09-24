using Stage6.ForAttributeWithMetadataName.Sample;

Console.WriteLine("=== Stage 6: ForAttributeWithMetadataName Data Source Generator Demo ===");
Console.WriteLine();

// Display generator information
Console.WriteLine("Generator Info:");
Console.WriteLine(UserDataFactory.GetGeneratorInfo());
Console.WriteLine();

Console.WriteLine("ForAttributeWithMetadataName Performance Benefits:");
Console.WriteLine("================================================");
Console.WriteLine(UserDataFactory.GetPerformanceStats());
Console.WriteLine();

Console.WriteLine("Generated Users (with cached configuration):");
Console.WriteLine("===========================================");

var users = UserDataFactory.CreateSamples();
foreach (var user in users)
{
    Console.WriteLine($"Name: {user.Name}, Email: {user.Email}, Age: {user.Age}, Role: {user.Role}, Active: {user.IsActive}");
}

Console.WriteLine();
Console.WriteLine("User Configuration Info:");
Console.WriteLine(UserDataFactory.GetConfigurationInfo());
Console.WriteLine();

Console.WriteLine("Generated Products (with cached configuration):");
Console.WriteLine("==============================================");

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

Console.WriteLine("Stage 6 Characteristics:");
Console.WriteLine("- ForAttributeWithMetadataName: Enhanced attribute detection performance");
Console.WriteLine("- Cached configurations: Reuses parsed configurations across builds");
Console.WriteLine("- Optimized filtering: More efficient attribute-based source generation");
Console.WriteLine("- Build performance: Measurable improvements over manual attribute detection");

Console.WriteLine();
Console.WriteLine("Performance Improvement: Stage 6 uses ForAttributeWithMetadataName for");
Console.WriteLine("more efficient attribute detection, reducing compilation time and");
Console.WriteLine("improving overall source generation performance.");

Console.WriteLine();
Console.WriteLine("Compare attribute detection methods:");
Console.WriteLine("- Stage 3: Manual GetAttributes().FirstOrDefault() filtering");
Console.WriteLine("- Stage 6: ForAttributeWithMetadataName optimized detection (THIS STAGE)");
Console.WriteLine("- Result: Faster builds and more efficient attribute processing");
