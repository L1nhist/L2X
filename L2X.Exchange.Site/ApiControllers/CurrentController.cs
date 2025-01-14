using L2X.Exchange.Site.Servicves;
using L2X.Services.Caching;

namespace L2X.Exchange.Site.ApiControllers;

/// <summary>
/// 
/// </summary>
/// <param name="service"></param>
[ApiController]
[Route("api/[controller]")]
public class CurrentController(RedisCacheService cache, DataGenerationService service) : ControllerBase()
{
    private readonly RedisCacheService _cache = cache;
    private readonly DataGenerationService _service = service;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("key")]
    public string NewId()
        => new Uuid().ToString();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("time")]
    public long Timestamp()
        => new Epoch().Timestamp;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [HttpGet("cadd")]
    public async Task SetCache(string key, string value)
        => await _cache.Set(key, value, 30000);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [HttpGet("cead")]
    public async Task<string?> GetCache(string key)
    {
        var value = await _cache.Get(key);
        return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
	[HttpGet("acc")]
    public async Task<IResult<int>> Account()
        => await _service.NewAccount();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("mkt")]
    public async Task<IResult<int>> Market()
        => await _service.NewMarket();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("mem")]
    public async Task<IResult<int>> Member()
        => await _service.NewMember();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="market"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    [HttpGet("pre")]
    public async Task<IResult<int>> PreOrder(string market, int number)
        => await _service.NewPreOrder(market, number);
}