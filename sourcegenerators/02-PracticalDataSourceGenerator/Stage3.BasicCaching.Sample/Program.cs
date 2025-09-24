using Stage3.BasicCaching.Sample;

Console.WriteLine("=== Stage 3: Basic Caching Data Source Generator Demo ===");
Console.WriteLine();

// Display generator information
Console.WriteLine("Generator Info:");
Console.WriteLine(UserDataFactory.GetGeneratorInfo());
Console.WriteLine();

Console.WriteLine("Caching Performance Benefits:");
Console.WriteLine("============================");
Console.WriteLine(UserDataFactory.GetCachingStats());
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

Console.WriteLine("Stage 3 Characteristics:");
Console.WriteLine("- Basic caching: Configuration files cached based on content changes");
Console.WriteLine("- Incremental providers: Only regenerates when dependencies change");
Console.WriteLine("- Performance optimization: Avoids redundant file I/O and parsing");
Console.WriteLine("- Content-aware caching: Uses file timestamps and content hashes");

Console.WriteLine();
Console.WriteLine("Performance Improvement: Stage 3 caches parsed configurations,");
Console.WriteLine("avoiding redundant file operations on subsequent builds when");
Console.WriteLine("configuration files haven't changed.");

Console.WriteLine();
Console.WriteLine("Compare build times:");
Console.WriteLine("- Stage 1: Recalculates everything every build");
Console.WriteLine("- Stage 2: Reparses all files every build");
Console.WriteLine("- Stage 3: Only reparses files when content changes (THIS STAGE)");
Console.WriteLine("- Next: Stage 4 will add advanced caching strategies");
