using System;
using System.Collections.Generic;

namespace Stage4.AdvancedCaching;

/// <summary>
/// Represents a structured cache key with hierarchical components for multi-level caching
/// </summary>
internal class CacheKey : IEquatable<CacheKey>
{
    public string EntityType { get; }
    public string? ConfigurationPath { get; }
    public int ContentHash { get; }
    public long LastModified { get; }
    public string GeneratorVersion { get; }
    public Dictionary<string, object> Dependencies { get; }

    public CacheKey(string entityType, string? configurationPath, int contentHash, long lastModified, string generatorVersion)
    {
        EntityType = entityType;
        ConfigurationPath = configurationPath;
        ContentHash = contentHash;
        LastModified = lastModified;
        GeneratorVersion = generatorVersion;
        Dependencies = new Dictionary<string, object>();
    }

    public string GetHierarchicalKey()
    {
        return $"{GeneratorVersion}:{EntityType}:{ContentHash}:{LastModified}";
    }

    public string GetPersistenceKey()
    {
        var dependencyHash = Dependencies.Count > 0 ? 
            GetDependencyHash() : 0;
        return $"{GetHierarchicalKey()}:{dependencyHash}";
    }

    private int GetDependencyHash()
    {
        var hash = 17;
        foreach (var kvp in Dependencies)
        {
            var key = kvp.Key;
            var value = kvp.Value;
            hash = hash * 31 + key.GetHashCode();
            hash = hash * 31 + (value?.GetHashCode() ?? 0);
        }
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