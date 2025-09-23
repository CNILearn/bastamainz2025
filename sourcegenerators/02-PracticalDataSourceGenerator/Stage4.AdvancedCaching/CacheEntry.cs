namespace Stage4.AdvancedCaching;

/// <summary>
/// Represents a cache entry with metadata and value, supporting serialization for persistence
/// </summary>
/// <typeparam name="T">The type of the cached value</typeparam>
internal class CacheEntry<T>
{
    public CacheKey Key { get; }
    public T Value { get; }
    public DateTime CreatedAt { get; }
    public DateTime LastAccessedAt { get; private set; }
    public TimeSpan TimeToLive { get; }
    public int AccessCount { get; private set; }
    public Dictionary<string, object> Metadata { get; }

    public CacheEntry(CacheKey key, T value, TimeSpan? ttl = null)
    {
        Key = key;
        Value = value;
        CreatedAt = DateTime.UtcNow;
        LastAccessedAt = CreatedAt;
        TimeToLive = ttl ?? TimeSpan.FromHours(24); // Default 24 hour TTL
        AccessCount = 0;
        Metadata = new Dictionary<string, object>();
    }

    public bool IsExpired => DateTime.UtcNow - CreatedAt > TimeToLive;

    public bool IsStale(TimeSpan maxAge) => DateTime.UtcNow - LastAccessedAt > maxAge;

    public void RecordAccess()
    {
        LastAccessedAt = DateTime.UtcNow;
        AccessCount++;
    }

    public void AddMetadata(string key, object value)
    {
        Metadata[key] = value;
    }

    public TMetadata? GetMetadata<TMetadata>(string key)
    {
        if (Metadata.TryGetValue(key, out var value) && value is TMetadata typedValue)
        {
            return typedValue;
        }
        return default;
    }
}

/// <summary>
/// Statistics and performance metrics for cache operations
/// </summary>
internal class CacheStatistics
{
    private long _totalAccessTime;

    public int L1Hits { get; private set; }
    public int L2Hits { get; private set; }
    public int L3Hits { get; private set; }
    public int Misses { get; private set; }
    public int Evictions { get; private set; }
    public int TotalAccesses { get; private set; }
    public double HitRatio => TotalAccesses == 0 ? 0.0 : (double)(L1Hits + L2Hits + L3Hits) / TotalAccesses;
    public double AverageAccessTime => TotalAccesses == 0 ? 0.0 : (double)_totalAccessTime / TotalAccesses;

    public void RecordL1Hit(long accessTimeMs = 0)
    {
        L1Hits++;
        RecordAccess(accessTimeMs);
    }

    public void RecordL2Hit(long accessTimeMs = 0)
    {
        L2Hits++;
        RecordAccess(accessTimeMs);
    }

    public void RecordL3Hit(long accessTimeMs = 0)
    {
        L3Hits++;
        RecordAccess(accessTimeMs);
    }

    public void RecordMiss(long accessTimeMs = 0)
    {
        Misses++;
        RecordAccess(accessTimeMs);
    }

    public void RecordEviction()
    {
        Evictions++;
    }

    private void RecordAccess(long accessTimeMs)
    {
        TotalAccesses++;
        _totalAccessTime += accessTimeMs;
    }

    public void Reset()
    {
        L1Hits = 0;
        L2Hits = 0;
        L3Hits = 0;
        Misses = 0;
        Evictions = 0;
        _totalAccessTime = 0;
        TotalAccesses = 0;
    }

    public string GetSummary()
    {
        return $"Cache Stats - L1: {L1Hits}, L2: {L2Hits}, L3: {L3Hits}, Misses: {Misses}, Hit Ratio: {HitRatio:P2}, Avg Access: {AverageAccessTime:F2}ms";
    }
}