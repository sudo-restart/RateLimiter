using Microsoft.AspNetCore.Mvc;
using ApiRateLimiter.Cache;


[Route("rateLimiter")]
[ApiController]
public class ApiRateLimiterController(ILogger<ApiRateLimiterController> logger, ICache cacheImpl) : ControllerBase
{
    ICache _cache = cacheImpl;
    private readonly ILogger<ApiRateLimiterController> _logger = logger;

    [HttpGet("{id}")]
    public async Task<ActionResult<string>> GetProduct(int id)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "NoIp";
        Guid myuuid = Guid.NewGuid();

        TimeSpan t = DateTime.UtcNow - new DateTime(2000, 1, 1);
        int secondsSinceEpoch = (int)t.TotalSeconds;

        await _cache.RemoveByRange(ipAddress, 0, secondsSinceEpoch - 10);
        long count = await _cache.GetCount(ipAddress);

        if(count >= 5) return "Exceeded permitted 10 values per 10 seconds, try again in a while.";
        else {
            await _cache.AddEntry(ipAddress, "IP" + myuuid.ToString(), secondsSinceEpoch);
            await _cache.SetKeyExpiry(ipAddress, TimeSpan.FromSeconds(5));
            return new RedirectResult(url: "https://www.google.com/", permanent: false,
                             preserveMethod: true);
        }

    }
}
