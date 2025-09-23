namespace Stage5.Optimized;

/// <summary>
/// Basic data source configuration structure for JSON deserialization
/// </summary>
internal class DataSourceConfiguration
{
    public string EntityType { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Templates { get; set; }
    public int? DefaultCount { get; set; }
    public Dictionary<string, object>? ExternalDependencies { get; set; }
    public string[]? Dependencies { get; set; }
}

/// <summary>
/// Enhanced cached configuration with advanced metadata for multi-level caching
/// </summary>
internal class AdvancedCachedDataSourceConfiguration
{
    public string EntityType { get; }
    public Dictionary<string, string[]> Templates { get; }
    public int? DefaultCount { get; }
    public string SourceFile { get; }
    public long LastWriteTime { get; }
    public int ContentHash { get; }
    public Dictionary<string, object> ExternalDependencies { get; }
    public HashSet<string> Dependencies { get; }
    public string SchemaVersion { get; }
    public DateTime CreatedAt { get; }

    public AdvancedCachedDataSourceConfiguration(
        string entityType,
        Dictionary<string, string[]> templates,
        int? defaultCount,
        string sourceFile,
        long lastWriteTime,
        int contentHash,
        Dictionary<string, object>? externalDependencies = null,
        string[]? dependencies = null,
        string schemaVersion = "1.0")
    {
        EntityType = entityType;
        Templates = templates;
        DefaultCount = defaultCount;
        SourceFile = sourceFile;
        LastWriteTime = lastWriteTime;
        ContentHash = contentHash;
        ExternalDependencies = externalDependencies ?? new Dictionary<string, object>();
        Dependencies = dependencies != null ? new HashSet<string>(dependencies) : new HashSet<string>();
        SchemaVersion = schemaVersion;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a cache key for this configuration
    /// </summary>
    public CacheKey CreateCacheKey(string generatorVersion)
    {
        var key = new CacheKey(EntityType, SourceFile, ContentHash, LastWriteTime, generatorVersion);
        
        // Add external dependencies to the cache key
        foreach (var kvp in ExternalDependencies)
        {
            var depKey = kvp.Key;
            var depValue = kvp.Value;
            key.AddDependency(depKey, depValue);
        }
        
        return key;
    }

    /// <summary>
    /// Determines if this configuration is compatible with another (for cache reuse)
    /// </summary>
    public bool IsCompatibleWith(AdvancedCachedDataSourceConfiguration other)
    {
        return EntityType == other.EntityType &&
               SchemaVersion == other.SchemaVersion &&
               ContentHash == other.ContentHash &&
               ExternalDependenciesEqual(other.ExternalDependencies);
    }

    private bool ExternalDependenciesEqual(Dictionary<string, object> otherDependencies)
    {
        if (ExternalDependencies.Count != otherDependencies.Count)
            return false;

        foreach (var kvp in ExternalDependencies)
        {
            var key = kvp.Key;
            var value = kvp.Value;
            if (!otherDependencies.TryGetValue(key, out var otherValue) || !Equals(value, otherValue))
                return false;
        }

        return true;
    }

    public override string ToString()
    {
        return $"{EntityType} (Hash: {ContentHash}, Dependencies: {Dependencies.Count}, External: {ExternalDependencies.Count})";
    }
}