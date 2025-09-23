using Stage1.Basic.Sample;

Console.WriteLine("=== Stage 1: Basic Data Source Generator Demo ===");
Console.WriteLine();

// Display generator information
Console.WriteLine("Generator Info:");
Console.WriteLine(UserDataFactory.GetGeneratorInfo());
Console.WriteLine();

Console.WriteLine("Generated Users:");
Console.WriteLine("================");

var users = UserDataFactory.CreateSamples();
foreach (var user in users)
{
    Console.WriteLine($"Name: {user.Name}, Email: {user.Email}, Age: {user.Age}, Role: {user.Role}, Active: {user.IsActive}");
}

Console.WriteLine();
Console.WriteLine("Generated Products:");
Console.WriteLine("==================");

var products = ProductDataFactory.CreateSamples();
foreach (var product in products)
{
    Console.WriteLine($"Product: {product.Name}, Price: ${product.Price:F2}, Stock: {product.StockQuantity}, Category: {product.Category}");
}

Console.WriteLine();
Console.WriteLine("Stage 1 Characteristics:");
Console.WriteLine("- No caching: Data generation logic runs every build");
Console.WriteLine("- Single data source: Only uses attribute configuration");
Console.WriteLine("- Simple implementation: Baseline for performance comparison");
Console.WriteLine("- All data values are randomly generated at runtime");

Console.WriteLine();
Console.WriteLine("Performance Note: In Stage 1, all generation logic is recalculated");
Console.WriteLine("during every compilation. Future stages will show caching benefits.");
