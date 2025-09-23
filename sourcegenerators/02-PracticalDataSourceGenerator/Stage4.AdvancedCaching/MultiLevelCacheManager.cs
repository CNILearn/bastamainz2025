using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Stage4.AdvancedCaching;

/// <summary>
/// Multi-level cache manager implementing L1 (in-memory), L2 (persistent), and L3 (distributed simulation) caching hierarchies
/// </summary>
internal class MultiLevelCacheManager(
    int l1MaxSize = 1000,
    TimeSpan? l1Ttl = null,
    TimeSpan? l2Ttl = null,
    TimeSpan? l3Ttl = null,
    string? persistentCacheDirectory = null)
{
    private readonly ConcurrentDictionary<string, object> _l1Cache = []; // In-memory cache
    private readonly PersistentCacheStorage _l2Cache = new(persistentCacheDirectory); // Persistent disk cache
    private readonly ConcurrentDictionary<string, object> _l3Cache = []; // Simulated distributed cache
    private readonly int _l1MaxSize = l1MaxSize;
    private readonly TimeSpan _l1Ttl = l1Ttl ?? TimeSpan.FromMinutes(30);
    private readonly TimeSpan _l2Ttl = l2Ttl ?? TimeSpan.FromHours(24);
    private readonly TimeSpan _l3Ttl = l3Ttl ?? TimeSpan.FromDays(7);

    public CacheStatistics Statistics { get; } = new();
    public ChangeDetectionEngine ChangeDetection { get; } = new();

    /// <summary>
    /// Retrieves a value from the cache hierarchy (L1 -> L2 -> L3)
    /// </summary>
    public async Task<T?> GetAsync<T>(CacheKey key) where T : class
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var keyString = key.GetHierarchicalKey();

            // L1 Cache (In-Memory) - Fastest access
            if (_l1Cache.TryGetValue(keyString, out var l1Value) && l1Value is CacheEntry<T> l1Entry)
            {
                if (!l1Entry.IsExpired)
                {
                    l1Entry.RecordAccess();
                    Statistics.RecordL1Hit(stopwatch.ElapsedMilliseconds);
                    return l1Entry.Value;
                }
                else
                {
                    // Expired, remove from L1
                    _l1Cache.TryRemove(keyString, out _);
                }
            }

            // L2 Cache (Persistent) - Medium access speed
            var l2Entry = await _l2Cache.LoadAsync<T>(key);
            if (l2Entry != null && !l2Entry.IsExpired)
            {
                l2Entry.RecordAccess();
                
                // Promote to L1 for faster future access
                await PromoteToL1Async(l2Entry);
                
                Statistics.RecordL2Hit(stopwatch.ElapsedMilliseconds);
                return l2Entry.Value;
            }

            // L3 Cache (Distributed Simulation) - Slowest access
            var l3Key = key.GetPersistenceKey();
            if (_l3Cache.TryGetValue(l3Key, out var l3Value) && l3Value is CacheEntry<T> l3Entry)
            {
                if (!l3Entry.IsExpired)
                {
                    l3Entry.RecordAccess();
                    
                    // Promote to L2 and L1 for faster future access
                    await PromoteToL2Async(l3Entry);
                    await PromoteToL1Async(l3Entry);
                    
                    Statistics.RecordL3Hit(stopwatch.ElapsedMilliseconds);
                    return l3Entry.Value;
                }
                else
                {
                    // Expired, remove from L3
                    _l3Cache.TryRemove(l3Key, out _);
                }
            }

            // Cache miss at all levels
            Statistics.RecordMiss(stopwatch.ElapsedMilliseconds);
            return null;
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    /// <summary>
    /// Stores a value in all cache levels
    /// </summary>
    public async Task SetAsync<T>(CacheKey key, T value) where T : class
    {
        var l1Entry = new CacheEntry<T>(key, value, _l1Ttl);
        var l2Entry = new CacheEntry<T>(key, value, _l2Ttl);
        var l3Entry = new CacheEntry<T>(key, value, _l3Ttl);

        // Store in all levels
        await SetL1Async(l1Entry);
        await SetL2Async(l2Entry);
        await SetL3Async(l3Entry);
    }

    /// <summary>
    /// Invalidates a cache entry across all levels
    /// </summary>
    public async Task InvalidateAsync(CacheKey key)
    {
        var keyString = key.GetHierarchicalKey();
        var persistenceKey = key.GetPersistenceKey();

        // Remove from all levels
        _l1Cache.TryRemove(keyString, out _);
        await _l2Cache.InvalidateAsync(key);
        _l3Cache.TryRemove(persistenceKey, out _);
    }

    /// <summary>
    /// Invalidates all cache entries for entities affected by changes
    /// </summary>
    public async Task InvalidateAffectedAsync(HashSet<string> affectedEntities)
    {
        List<string> keysToInvalidate = [];

        // Find all cache keys that match affected entities
        foreach (var key in _l1Cache.Keys)
        {
            if (affectedEntities.Any(entity => key.Contains(entity)))
            {
                keysToInvalidate.Add(key);
            }
        }

        // Invalidate found keys
        var tasks = keysToInvalidate.Select(async keyString =>
        {
            _l1Cache.TryRemove(keyString, out _);
            
            // For L2 and L3, we need to construct a cache key - this is a simplified approach
            // In a real implementation, you'd want to maintain a reverse lookup
            foreach (var entity in affectedEntities)
            {
                var tempKey = new CacheKey(entity, null, 0, 0, "");
                await _l2Cache.InvalidateAsync(tempKey);
                _l3Cache.TryRemove(tempKey.GetPersistenceKey(), out _);
            }
        });

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Performs cache maintenance operations (cleanup expired entries, evict LRU items)
    /// </summary>
    public async Task PerformMaintenanceAsync()
    {
        // Cleanup expired L1 entries
        List<string> expiredL1Keys = [];
        foreach (var kvp in _l1Cache)
        {
            var key = kvp.Key;
            var value = kvp.Value;
            if (value is CacheEntry<object> entry && entry.IsExpired)
            {
                expiredL1Keys.Add(key);
            }
        }

        foreach (var key in expiredL1Keys)
        {
            if (_l1Cache.TryRemove(key, out _))
            {
                Statistics.RecordEviction();
            }
        }

        // Evict LRU items if L1 cache is over capacity
        if (_l1Cache.Count > _l1MaxSize)
        {
            var entriesToEvict = _l1Cache
                .Select(kvp => new { Key = kvp.Key, Entry = kvp.Value as CacheEntry<object> })
                .Where(x => x.Entry != null)
                .OrderBy(x => x.Entry!.LastAccessedAt)
                .Take(_l1Cache.Count - _l1MaxSize)
                .ToList();

            foreach (var item in entriesToEvict)
            {
                if (_l1Cache.TryRemove(item.Key, out _))
                {
                    Statistics.RecordEviction();
                }
            }
        }

        // Cleanup expired L2 entries
        await _l2Cache.CleanupExpiredEntriesAsync();

        // Cleanup expired L3 entries
        var expiredL3Keys = new List<string>();
        foreach (var kvp in _l3Cache)
        {
            var key = kvp.Key;
            var value = kvp.Value;
            if (value is CacheEntry<object> entry && entry.IsExpired)
            {
                expiredL3Keys.Add(key);
            }
        }

        foreach (var key in expiredL3Keys)
        {
            _l3Cache.TryRemove(key, out _);
        }
    }

    /// <summary>
    /// Gets cache status and statistics
    /// </summary>
    public CacheStatus GetStatus()
    {
        return new CacheStatus
        {
            L1Count = _l1Cache.Count,
            L3Count = _l3Cache.Count,
            Statistics = Statistics.GetSummary(),
            ChangeDetectionSummary = ChangeDetection.GetTrackingSummary()
        };
    }

    private async Task SetL1Async<T>(CacheEntry<T> entry) where T : class
    {
        var keyString = entry.Key.GetHierarchicalKey();
        _l1Cache.AddOrUpdate(keyString, entry, (_, _) => entry);
        
        // Trigger maintenance if cache is getting full
        if (_l1Cache.Count > _l1MaxSize * 1.2) // 20% over capacity
        {
            _ = Task.Run(PerformMaintenanceAsync); // Fire and forget
        }
    }

    private async Task SetL2Async<T>(CacheEntry<T> entry) where T : class
    {
        await _l2Cache.SaveAsync(entry);
    }

    private Task SetL3Async<T>(CacheEntry<T> entry) where T : class
    {
        var keyString = entry.Key.GetPersistenceKey();
        _l3Cache.AddOrUpdate(keyString, entry, (_, _) => entry);
        return Task.CompletedTask; // Simulate async distributed cache operation
    }

    private async Task PromoteToL1Async<T>(CacheEntry<T> entry) where T : class
    {
        var l1Entry = new CacheEntry<T>(entry.Key, entry.Value, _l1Ttl);
        await SetL1Async(l1Entry);
    }

    private async Task PromoteToL2Async<T>(CacheEntry<T> entry) where T : class
    {
        var l2Entry = new CacheEntry<T>(entry.Key, entry.Value, _l2Ttl);
        await SetL2Async(l2Entry);
    }
}

/// <summary>
/// Represents the current status of the multi-level cache
/// </summary>
internal class CacheStatus
{
    public int L1Count { get; set; }
    public int L3Count { get; set; }
    public string Statistics { get; set; } = string.Empty;
    public string ChangeDetectionSummary { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Cache Status - L1: {L1Count} entries, L3: {L3Count} entries | {Statistics} | {ChangeDetectionSummary}";
    }
}