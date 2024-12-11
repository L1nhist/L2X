using System.Collections.Concurrent;

namespace L2X.Core.Caching;

public class ConcurentCache(TimeSpan period)
{
    private class CacheEntry(DateTime timestamp, object? data)
    {
        public object? Data { get; } = data;

        public DateTime Timestamp { get; } = timestamp;
    }

    private readonly TimeSpan _period = period;
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = [];

    /// <summary>
    /// A cache value by key
    /// </summary>
    /// <param name="key">The key identifier</param>
    /// <returns>Cached value if it was in cache</returns>
    public object? this[string key]
    {
        get => Get(key);
        set => Add(key, value);
    }

    /// <summary>
    /// Add a new cache entry. Will override an existing entry if it already exists
    /// </summary>
    /// <param name="key">The key identifier</param>
    /// <param name="value">Cache value</param>
    public void Add(string key, object? data)
    {
        var entry = new CacheEntry(DateTime.UtcNow, data);
        _cache.AddOrUpdate(key, entry, (k, v) => entry);
    }

    /// <summary>
    /// Clear all over time cache entry
    /// </summary>
    public void Compact()
    {
        var now = DateTime.UtcNow;
        var keys = _cache.Keys.ToArray();
        foreach (var key in keys)
        {
            if (now - _cache[key].Timestamp > _period)
                _cache.TryRemove(key, out _);
        }
    }

    /// <summary>
    /// Get a cached value
    /// </summary>
    /// <param name="key">The key identifier</param>
    /// <returns>Cached value if it was in cache</returns>
    public object? Get(string key)
    {
        _cache.TryGetValue(key, out CacheEntry? entry);
        if (entry == null) return null;
        if (DateTime.UtcNow - entry.Timestamp > _period)
        {
            _cache.TryRemove(key, out _);
            return null;
        }

        return entry.Data;
    }
}