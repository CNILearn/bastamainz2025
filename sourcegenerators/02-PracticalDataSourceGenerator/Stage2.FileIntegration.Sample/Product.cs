using Stage2.FileIntegration.Attributes;

namespace Stage2.FileIntegration.Sample;

/// <summary>
/// Sample product entity with external configuration for realistic data
/// </summary>
[DataSource(EntityName = "Product", Count = 8, ConfigurationFile = "Product.datasource.json")]
public class Product
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public Guid Id { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime? LastUpdated { get; set; }
    public ProductCategory Category { get; set; }
    public double Weight { get; set; }
    public float Rating { get; set; }
}
