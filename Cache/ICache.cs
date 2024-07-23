namespace ApiRateLimiter.Cache;

public interface ICache
{
    Task<long> RemoveByRange(string key, double minVal, double maxVal);
    Task<long> AddEntry(string key, string type, double value);
    Task<long> GetCount(string key);
    Task<bool> SetKeyExpiry(string key, TimeSpan timeout);
}