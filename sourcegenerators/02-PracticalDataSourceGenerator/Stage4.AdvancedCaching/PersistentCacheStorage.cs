using System.Collections.Concurrent;

namespace Stage4.AdvancedCaching;

/// <summary>
/// Simulated persistent storage for source generator constraints (no actual file I/O)
/// In a real implementation, this would serialize to disk between builds
/// </summary>
internal class PersistentCacheStorage(string? cacheDirectory = null, string schemaVersion = "1.0")
{
    private readonly ConcurrentDictionary<string, string> _simulatedStorage = [];
    private readonly string _schemaVersion = schemaVersion;

    public Task<CacheEntry<T>?> LoadAsync<T>(CacheKey key) where T : class
    {
        var cacheKeyString = key.GetPersistenceKey();
        
        // Simulate cache lookup - in real implementation this would deserialize from disk
        if (_simulatedStorage.TryGetValue(cacheKeyString, out var serializedData))
        {
            // Simplified simulation - in real implementation would deserialize JSON
            if (serializedData.Contains(_schemaVersion))
            {
                // Return null to simulate cache miss for demonstration
                // Real implementation would deserialize the actual cached data
                return Task.FromResult<CacheEntry<T>?>(null);
            }
        }
        
        return Task.FromResult<CacheEntry<T>?>(null);
    }

    public Task SaveAsync<T>(CacheEntry<T> entry)
    {
        var cacheKeyString = entry.Key.GetPersistenceKey();
        
        // Simulate cache storage - in real implementation this would serialize to disk
        var serializedData = $"{_schemaVersion}:{entry.Value}:{entry.CreatedAt}";
        _simulatedStorage.AddOrUpdate(cacheKeyString, serializedData, (_, _) => serializedData);
        
        return Task.CompletedTask;
    }

    public Task InvalidateAsync(CacheKey key)
    {
        var cacheKeyString = key.GetPersistenceKey();
        _simulatedStorage.TryRemove(cacheKeyString, out _);
        return Task.CompletedTask;
    }

    public Task CleanupExpiredEntriesAsync()
    {
        // Simulate cleanup - in real implementation would check file timestamps and remove expired entries
        var keysToRemove = new List<string>();
        foreach (var kvp in _simulatedStorage)
        {
            var key = kvp.Key;
            var value = kvp.Value;
            // Simple simulation - remove entries that don't match current schema
            if (!value.StartsWith(_schemaVersion))
            {
                keysToRemove.Add(key);
            }
        }

        foreach (var key in keysToRemove)
        {
            _simulatedStorage.TryRemove(key, out _);
        }

        return Task.CompletedTask;
    }

    public int GetStorageCount() => _simulatedStorage.Count;
}