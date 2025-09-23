namespace Stage5.Optimized;

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
/// Optimized statistics and performance metrics for cache operations with microsecond precision
/// Uses atomic operations for thread-safe, lock-free updates
/// </summary>
internal class OptimizedCacheStatistics
{
    private long _totalAccessTime;
    private long _l1Hits;
    private long _l2Hits;
    private long _l3Hits;
    private long _misses;
    private long _evictions;
    private long _totalAccesses;

    public long L1Hits => Interlocked.Read(ref _l1Hits);
    public long L2Hits => Interlocked.Read(ref _l2Hits);
    public long L3Hits => Interlocked.Read(ref _l3Hits);
    public long Misses => Interlocked.Read(ref _misses);
    public long Evictions => Interlocked.Read(ref _evictions);
    public long TotalAccesses => Interlocked.Read(ref _totalAccesses);
    public double HitRatio
    {
        get
        {
            var total = TotalAccesses;
            return total == 0 ? 0.0 : (double)(L1Hits + L2Hits + L3Hits) / total;
        }
    }
    public double AverageAccessTime
    {
        get
        {
            var total = TotalAccesses;
            return total == 0 ? 0.0 : (double)Interlocked.Read(ref _totalAccessTime) / total;
        }
    }

    public void RecordL1Hit(long accessTimeMs = 0)
    {
        Interlocked.Increment(ref _l1Hits);
        RecordAccess(accessTimeMs);
    }

    public void RecordL2Hit(long accessTimeMs = 0)
    {
        Interlocked.Increment(ref _l2Hits);
        RecordAccess(accessTimeMs);
    }

    public void RecordL3Hit(long accessTimeMs = 0)
    {
        Interlocked.Increment(ref _l3Hits);
        RecordAccess(accessTimeMs);
    }

    public void RecordMiss(long accessTimeMs = 0)
    {
        Interlocked.Increment(ref _misses);
        RecordAccess(accessTimeMs);
    }

    public void RecordEviction()
    {
        Interlocked.Increment(ref _evictions);
    }

    private void RecordAccess(long accessTimeMs)
    {
        Interlocked.Increment(ref _totalAccesses);
        Interlocked.Add(ref _totalAccessTime, accessTimeMs);
    }

    public void Reset()
    {
        Interlocked.Exchange(ref _l1Hits, 0);
        Interlocked.Exchange(ref _l2Hits, 0);
        Interlocked.Exchange(ref _l3Hits, 0);
        Interlocked.Exchange(ref _misses, 0);
        Interlocked.Exchange(ref _evictions, 0);
        Interlocked.Exchange(ref _totalAccessTime, 0);
        Interlocked.Exchange(ref _totalAccesses, 0);
    }

    public string GetSummary()
    {
        return $"Cache Stats - L1: {L1Hits}, L2: {L2Hits}, L3: {L3Hits}, Misses: {Misses}, Hit Ratio: {HitRatio:P2}, Avg Access: {AverageAccessTime:F2}ms";
    }
}