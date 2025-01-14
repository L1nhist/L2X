namespace L2X.Core.Caching;

public interface ICacheService
{
	Task<T?> Get<T>(string key);

	Task Set<T>(string key, T? value, int msecs = 0);

	Task Set<T>(string key, T? value, DateTime expire);

	Task Remove(string key);
}

public interface ICacheService<T>
{
	Task<T?> Get(string key);

	Task Set(string key, T? value, int msecs = 0);

	Task Set(string key, T? value, DateTime expire);

	Task Remove(string key);
}