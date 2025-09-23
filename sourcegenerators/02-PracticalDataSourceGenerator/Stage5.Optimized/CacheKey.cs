namespace Stage5.Optimized;

/// <summary>
/// Represents a structured cache key with hierarchical components for multi-level caching
/// </summary>
internal class CacheKey(string entityType, string? configurationPath, int contentHash, long lastModified, string generatorVersion) : IEquatable<CacheKey>
{
    public string EntityType { get; } = entityType;
    public string? ConfigurationPath { get; } = configurationPath;
    public int ContentHash { get; } = contentHash;
    public long LastModified { get; } = lastModified;
    public string GeneratorVersion { get; } = generatorVersion;
    public Dictionary<string, object> Dependencies { get; } = [];

    // Cached key strings to reduce allocations
    private string? _hierarchicalKey;
    private string? _persistenceKey;
    private int? _dependencyHash;

    public string GetHierarchicalKey()
    {
        return _hierarchicalKey ??= $"{GeneratorVersion}:{EntityType}:{ContentHash}:{LastModified}";
    }

    public string GetPersistenceKey()
    {
        if (_persistenceKey == null)
        {
            var dependencyHash = Dependencies.Count > 0 ? GetDependencyHash() : 0;
            _persistenceKey = $"{GetHierarchicalKey()}:{dependencyHash}";
        }
        return _persistenceKey;
    }

    private int GetDependencyHash()
    {
        if (_dependencyHash.HasValue)
            return _dependencyHash.Value;
            
        var hash = 17;
        foreach (var kvp in Dependencies)
        {
            var key = kvp.Key;
            var value = kvp.Value;
            hash = hash * 31 + key.GetHashCode();
            hash = hash * 31 + (value?.GetHashCode() ?? 0);
        }
        _dependencyHash = hash;
        return hash;
    }

    public void AddDependency(string key, object value)
    {
        Dependencies[key] = value;
    }

    public bool Equals(CacheKey? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return EntityType == other.EntityType &&
               ConfigurationPath == other.ConfigurationPath &&
               ContentHash == other.ContentHash &&
               LastModified == other.LastModified &&
               GeneratorVersion == other.GeneratorVersion &&
               DependenciesEqual(other.Dependencies);
    }

    private bool DependenciesEqual(Dictionary<string, object> otherDependencies)
    {
        if (Dependencies.Count != otherDependencies.Count) return false;
        
        foreach (var kvp in Dependencies)
        {
            var key = kvp.Key;
            var value = kvp.Value;
            if (!otherDependencies.TryGetValue(key, out var otherValue))
                return false;
            if (!Equals(value, otherValue))
                return false;
        }
        return true;
    }

    public override bool Equals(object? obj) => Equals(obj as CacheKey);

    public override int GetHashCode()
    {
        var hash = (EntityType?.GetHashCode() ?? 0) ^
                  (ConfigurationPath?.GetHashCode() ?? 0) ^
                  ContentHash ^
                  LastModified.GetHashCode() ^
                  (GeneratorVersion?.GetHashCode() ?? 0);
        return hash ^ GetDependencyHash();
    }

    public static bool operator ==(CacheKey? left, CacheKey? right) => Equals(left, right);
    public static bool operator !=(CacheKey? left, CacheKey? right) => !Equals(left, right);
}