using Stage1.Basic.Attributes;

namespace Stage1.Basic.Sample;

/// <summary>
/// Sample product entity with more complex properties
/// </summary>
[DataSource(EntityName = "Product", Count = 8)]
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
