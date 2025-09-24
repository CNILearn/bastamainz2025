namespace Stage6.ForAttributeWithMetadataName;

internal class DataSourceConfiguration
{
    public string EntityType { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Templates { get; set; }
    public int? DefaultCount { get; set; }
}
