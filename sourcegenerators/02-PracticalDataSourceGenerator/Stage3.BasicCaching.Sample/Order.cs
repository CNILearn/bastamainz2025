using Stage3.BasicCaching.Attributes;

namespace Stage3.BasicCaching.Sample;

/// <summary>
/// Entity without external configuration file to show fallback behavior
/// </summary>
[DataSource(EntityName = "Order", Count = 3)]
public class Order
{
    public string OrderNumber { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTime OrderDate { get; set; }
    public bool IsProcessed { get; set; }
}
