using L2X.Data.Entities;
using L2X.Exchange.Data;
using L2X.Exchange.Data.Services;
using L2X.Exchange.Models;
using L2X.Services.Caching;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IResult = L2X.Data.Results.IResult;

namespace L2X.Exchange.Site.ApiControllers;

/// <summary>
/// 
/// </summary>
/// <param name="service"></param>
[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService service, RedisCacheService redis) : ControllerBase()
{
    private readonly IOrderService _service = service;
	private readonly RedisCacheService _redis = redis;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("all-buy")]
	[HttpGet("all-buy/{page}")]
	public async Task<PagedResult<OrderResponse>> GetAllBuy(int? page)
    {
        try
		{
			var request = new PagingRequest
			{
                FilterBy = OrderCommonState.WAITING,
				PageIndex = page > 0 ? page : 0,
				PageSize = 50
			};

			return await _service.PaginateByState(true, request);
        }
        catch (Exception ex)
        {
            return Result.PagedError<OrderResponse>(ex);
        }
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	[HttpGet("all-sell")]
	[HttpGet("all-sell/{page}")]
	public async Task<PagedResult<OrderResponse>> GetAllSell(int? page)
	{
		try
		{
			var request = new PagingRequest
			{
				FilterBy = OrderCommonState.WAITING,
				PageIndex = page > 0 ? page : 0,
				PageSize = 50
			};

			return await _service.PaginateByState(false, request);
		}
		catch (Exception ex)
		{
			return Result.PagedError<OrderResponse>(ex);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	[HttpGet("cancel")]
	[HttpGet("cancel/{page}")]
	public async Task<PagedResult<OrderResponse>> GetCancel(int? page)
	{
		try
		{
			var request = new PagingRequest
			{
				PageIndex = page > 0 ? page : 0,
				PageSize = 50
			};

			return await _service.PaginateByFail(request);
		}
		catch (Exception ex)
		{
			return Result.PagedError<OrderResponse>(ex);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	[HttpGet("pre-buy")]
	[HttpGet("pre-buy/{page}")]
	public async Task<PagedResult<PreOrderResponse>> GetPreBuyOrder(int? page)
	{
		try
		{
            var request = new PagingRequest
            {
                PageIndex = page > 0 ? page : 0,
                PageSize = 50
            };

			return await _service.PaginateAllPre(true, request);
		}
		catch (Exception ex)
		{
			return Result.PagedError<PreOrderResponse>(ex);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	[HttpGet("pre-sell")]
	[HttpGet("pre-sell/{page}")]
	public async Task<PagedResult<PreOrderResponse>> GetPreSellOrder(int? page)
	{
		try
		{
			var request = new PagingRequest
			{
				PageIndex = page > 0 ? page : 0,
				PageSize = 50
			};

			return await _service.PaginateAllPre(false, request);
		}
		catch (Exception ex)
		{
			return Result.PagedError<PreOrderResponse>(ex);
		}
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("book-buy")]
    [HttpGet("book-buy/{page}")]
    public async Task<PagedResult<OBInfo>> GetBookBuy(int? page)
    {
        var result = Result.NewPage<OBInfo>();
        if (page < 0) return result.BadRequest();

        try
        {
            var request = await _redis.Get<IEnumerable<OBInfo>>("BID_OB");
            var cnt = request?.Count();
            if (request == null || cnt < page * 50) return result.NotFound();

            var paged = new Pagination<OBInfo>(page ?? 0, 50, request?.Count() ?? 0, request?.Skip((page ?? 0) * 50)?.Take(50));
            return result.Ok(paged);
        }
        catch (Exception ex)
        {
            return result.Error(ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("book-sell")]
    [HttpGet("book-sell/{page}")]
    public async Task<PagedResult<OBInfo>> GetBookSell(int? page)
    {
        var result = Result.NewPage<OBInfo>();
        if (page < 0) return result.BadRequest();

        try
        {
            var request = await _redis.Get<IEnumerable<OBInfo>>("ASK_OB");
            var cnt = request?.Count();
            if (request == null || cnt < page * 50) return result.NotFound();

            var paged = new Pagination<OBInfo>(page ?? 0, 50, request?.Count() ?? 0, request?.Skip((page ?? 0) * 50)?.Take(50));
            return result.Ok(paged);
        }
        catch (Exception ex)
        {
            return result.Error(ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("match")]
	[HttpGet("match/{page}")]
	public async Task<PagedResult<MatchFillResponse>> GetMatchOrder(int? page)
	{
		try
		{
			var request = new PagingRequest
			{
				PageIndex = page > 0 ? page : 0,
				PageSize = 50
			};

			return await _service.PaginateAllMatch(request);
		}
		catch (Exception ex)
		{
			return Result.PagedError<MatchFillResponse>(ex);
		}
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("market-info")]
    public async Task<IResult<MarketInfo>> GetMarketInfo()
    {
        var result = Result.New<MarketInfo>();

        try
        {
            var info = await _redis.Get<MarketInfo>("MKT_INFO");
            return info == null ? result.NotFound() : result.Ok(info);
        }
        catch (Exception ex)
        {
            return result.Error(ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IResult<OrderResponse>> GetOne(string id)
    {
        try
        {
            return await _service.GetByIdOrIndex(id);
        }
        catch (Exception ex)
        {
            return Result.Error<OrderResponse>(ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("new")]
    public async Task<IResult> Create(OrderRequest request)
    {
        try
        {
            return await _service.Create(request);
        }
        catch (Exception ex)
        {
            return Result.Error<OrderResponse>(ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("{id}")]
    public async Task<IResult> Update(string id, OrderRequest request)
    {
        var result = Result.New();

        try
        {
            var uid = new Uuid(id);
            if (uid.IsEmpty) return result.BadRequest("Requested is not OrderId");

            return await _service.Update(uid, request);
        }
        catch (Exception ex)
        {
            return result.Error(ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IResult> Delete(string id)
    {
        var result = Result.New();

        try
        {
            var uid = new Uuid(id);
            if (uid.IsEmpty) return result.BadRequest("Requested is not OrderId");
            return await _service.Delete(uid);
        }
        catch (Exception ex)
        {
            return result.Error(ex);
        }
    }
}