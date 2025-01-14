using L2X.Exchange.Data.Services;
using IResult = L2X.Data.Results.IResult;

namespace L2X.Exchange.Site.ApiControllers;

/// <summary>
/// 
/// </summary>
/// <param name="service"></param>
[ApiController]
[Route("api/[controller]")]
public class MembersController(IMemberService service) : ControllerBase()
{
    private readonly IMemberService _service = service;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<PagedResult<MemberResponse>> GetAll(PagingRequest request)
    {
        try
        {
            return await _service.Paginate<MemberResponse>(request);
        }
        catch (Exception ex)
        {
            return Result.PagedError<MemberResponse>(ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [HttpGet("{key}")]
    public async Task<IResult<MemberResponse>> GetOne(string key)
    {
        try
        {
            return await _service.GetByIdOrName(key);
        }
        catch (Exception ex)
        {
            return Result.Error<MemberResponse>(ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("new")]
    public async Task<IResult> Create(MemberRequest request)
    {
        try
        {
            return await _service.Create(request);
        }
        catch (Exception ex)
        {
            return Result.Error<MemberResponse>(ex);
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