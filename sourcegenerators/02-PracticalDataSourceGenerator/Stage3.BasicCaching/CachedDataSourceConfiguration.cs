namespace Stage3.BasicCaching;

/// <summary>
/// Cached configuration that includes metadata for change detection and performance optimization
/// </summary>
internal class CachedDataSourceConfiguration(
    string entityType,
    Dictionary<string, string[]> templates,
    int? defaultCount,
    string sourceFile,
    long lastWriteTime,
    int contentHash)
{
    public string EntityType { get; } = entityType;
    public Dictionary<string, string[]> Templates { get; } = templates;
    public int? DefaultCount { get; } = defaultCount;
    public string SourceFile { get; } = sourceFile;
    public long LastWriteTime { get; } = lastWriteTime;
    public int ContentHash { get; } = contentHash;
}