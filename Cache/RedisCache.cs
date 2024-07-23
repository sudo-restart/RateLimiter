
using StackExchange.Redis;

namespace ApiRateLimiter.Cache;

public class RedisCache(IConnectionMultiplexer connectionMultiplexer) : ICache
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();

    public async Task<long> RemoveByRange(string key, double minVal, double maxVal)
    {
       return await _database.SortedSetRemoveRangeByScoreAsync(key, 0, maxVal);
    }

    public async Task<long> AddEntry(string key, string type, double value)
    {
        return await _database.SortedSetAddAsync(key, [new SortedSetEntry(type, value)]);
    }

    public async Task<long> GetCount(string key)
    {
        return await _database.SortedSetLengthAsync(key);
    }

    public async Task<bool> SetKeyExpiry(string key, TimeSpan timeout)
    {
        return await _database.KeyExpireAsync(key, timeout);
    }

    ~RedisCache()
    {
        connectionMultiplexer.Close();
    }
}