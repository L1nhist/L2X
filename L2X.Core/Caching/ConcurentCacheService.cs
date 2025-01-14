using System.Collections.Concurrent;

namespace L2X.Core.Caching;

public class ConcurentCacheService(TimeSpan period) : ICacheService
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
        get
        {
            _cache.TryGetValue(key, out CacheEntry? entry);
            if (entry == null) return null;
            if (DateTime.UtcNow > entry.Timestamp)
            {
                _cache.TryRemove(key, out _);
                return null;
            }
            return entry.Data;
        }
        set => Set(key, value, DateTime.UtcNow.Add(_period));
    }

	/// <summary>
	/// Get a cached value
	/// </summary>
	/// <param name="key">The key identifier</param>
	/// <returns>Cached value if it was in cache</returns>
	public Task<T?> Get<T>(string key)
	{
        var obj = this[key];
        return Task.FromResult(obj == null ? default : Util.Convert<object, T>(obj));
	}

    /// <summary>
    /// Add a new cache entry. Will override an existing entry if it already exists
    /// </summary>
    /// <param name="key">The key identifier</param>
    /// <param name="value">Cache value</param>
    public Task Set<T>(string key, T? data, int msecs = 0)
        => Set(key, data, DateTime.UtcNow.AddMilliseconds(msecs));

	/// <summary>
	/// Add a new cache entry. Will override an existing entry if it already exists
	/// </summary>
	/// <param name="key">The key identifier</param>
	/// <param name="value">Cache value</param>
	public Task Set<T>(string key, T? data, DateTime expire)
	{
        if (data == null) return Task.CompletedTask;

		var entry = new CacheEntry(expire, data);
		_cache.AddOrUpdate(key, entry, (k, v) => entry);
        return Task.CompletedTask;
	}

	/// <summary>
	/// Clear all over time cache entry
	/// </summary>
	public Task Compact()
    {
        var now = DateTime.UtcNow;
        var keys = _cache.Keys.ToArray();
        foreach (var key in keys)
        {
            if (_cache[key].Timestamp < now)
                _cache.TryRemove(key, out _);
        }

        return Task.CompletedTask;
    }

    public Task Remove(string key)
    {
        _cache.Remove(key, out var entry);
        return Task.CompletedTask;

	}
}