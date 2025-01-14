using L2X.Core.Caching;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace L2X.Services.Caching;

public class RedisCacheService : ICacheService, ICacheService<string>
{
	private readonly IDatabase _dbRedis;

	public RedisCacheService(IConfiguration config)
	{
		var lazy = new Lazy<ConnectionMultiplexer>(() =>
							{
								var conn = config.GetConnectionString("RedisDC") ?? throw new NullReferenceException("Connection string for Redis can not be found");
								return ConnectionMultiplexer.Connect(conn);
							});
		_dbRedis = lazy.Value.GetDatabase();
	}

	private string? Serialize<T>(T? value)
		=> value == null ? null : JsonSerializer.Serialize<T>(value);

	private T? Deserialize<T>(string? value)
		=> Util.IsEmpty(value) ? default : JsonSerializer.Deserialize<T>(value);

	private Task Set(string key, string? value, TimeSpan period)
		=> value == null ? Task.CompletedTask : _dbRedis.StringSetAsync(key, value, period);

	public async Task<string?> Get(string key)
		=> (await _dbRedis.StringGetAsync(key)).ToString();

	public async Task Set(string key, string? value, int msecs)
		=> await _dbRedis.StringSetAsync(key, value, TimeSpan.FromMilliseconds(msecs));

	public async Task Set(string key, string? value, DateTime expire)
		=> await Set(key, value, expire.Subtract(DateTime.Now));

	public async Task<T?> Get<T>(string key)
		=> Deserialize<T>(await Get(key));

	public async Task Set<T>(string key, T? value, int msecs)
		=> await Set(key, Serialize(value), msecs);

	public async Task Set<T>(string key, T? value, DateTime expire)
		=> await Set(key, Serialize(value), expire);

	public async Task Remove(string key)
	{
		if (await _dbRedis.KeyExistsAsync(key))
			await _dbRedis.KeyDeleteAsync(key);
	}
}