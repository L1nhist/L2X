using AutoMapper;
using L2X.Core.Validations;
using L2X.Data.Extensions;
using L2X.Data.Repositories;
using L2X.Data.Results;
using L2X.Data.Services;
using L2X.Exchange.Data.Entities;
using Microsoft.Extensions.Logging;

namespace L2X.Exchange.Data.Services;

public class OrderService(
        ILoggerFactory loggerFactory,
        IMapper mapper,
		IRepository<Match> mchRepo,
        IRepository<Order> repository,
        IRepository<PreOrder> preRepo,
		IValidator<Order> validator
    )
    : DataManagementService<Order, Guid>(loggerFactory, mapper, repository), IOrderService
{
    private readonly IRepository<Match> _mchRepo = mchRepo;
	private readonly IRepository<PreOrder> _preRepo = preRepo;
	private readonly IValidator<Order> _validator = validator;

    public override async Task<ValidResult<Order>> Validate(IRequest? request)
    {
        var valid = await base.Validate(request);
        if (!valid.Success) return valid;

        return valid.IsValid(_validator);
    }

    public async Task<IEnumerable<PreOrder>> GetPreOrders(int top)
        => await _preRepo.SortBy(o => o.CreatedAt).GetList(top);

    public async Task<IResult<OrderResponse>> GetByIdOrIndex(string value)
    {
        var result = Result.New<OrderResponse>();

        try
        {
            var uid = new Uuid(value);
            var num = new Sequence(value);

            Order? order = null;
            if (!uid.IsEmpty)
            {
                order = await Repository.GetById<Order, Guid>(uid);
            }
            else if (!num.IsEmpty)
            {
                order = await Repository.Query(o => o.OrderNo == num).GetFirst();
            }

            if (order == null) return result.BadRequest("Request value must be OrderId or OrderNo");

            var data = Mapper.Map<Order, OrderResponse>(order);
            if (data == null) return result.BadRequest("Result data can not be mapped as usual");
            return result.Ok(data);
        }
        catch (Exception ex)
        {
            return result.Error(ex);
        }
	}

	public async Task<PagedResult<OrderResponse>> PaginateByState(bool side, PagingRequest request)
	{
		var result = Result.NewPage<OrderResponse>();

		try
		{
			var state = request?.FilterBy?.Trim()?.ToLower();
			if (!OrderCommonState.Contains(state)) return result.BadRequest("Requested state is empty or not exist");

			var data = await Repository.Query(o => o.Side == side && o.State == state).JoinBy(o => o.Owner).JoinBy(o => o.Market).GetPaging<OrderResponse>(request.PageIndex, request.PageSize);
			if (data == null) return result.BadRequest("Can not find result");
			return result.Ok(data);
		}
		catch (Exception ex)
		{
			return result.Error(ex);
		}
	}

	public async Task<PagedResult<OrderResponse>> PaginateByFail(PagingRequest request)
    {
        var result = Result.NewPage<OrderResponse>();

        try
        {
            var data = await Repository.Query(o => o.State.Contains("invalid") || o.State.Contains("cancel")).JoinBy(o => o.Owner).JoinBy(o => o.Market).GetPaging<OrderResponse>(request.PageIndex, request.PageSize);
            if (data == null) return result.BadRequest("Can not find result");
            return result.Ok(data);
        }
        catch (Exception ex)
        {
            return result.Error(ex);
        }
	}

    public async Task<PagedResult<PreOrderResponse>> PaginateAllPre(bool side, PagingRequest request)
	{
		var result = Result.NewPage<PreOrderResponse>();

		try
		{
			var data = await _preRepo.Query(o => o.Side == side).SortBy(o => o.CreatedAt).JoinBy(o => o.Owner).JoinBy(o => o.Market).GetPaging<PreOrderResponse>(request.PageIndex, request.PageSize);
			if (data == null) return result.BadRequest("Can not find result");
			return result.Ok(data);
		}
		catch (Exception ex)
		{
			return result.Error(ex);
		}
	}

	public async Task<PagedResult<MatchFillResponse>> PaginateAllMatch(PagingRequest request)
	{
		var result = Result.NewPage<MatchFillResponse>();

		try
		{
			var mchs = await _mchRepo.SortBy(o => o.CreatedAt).JoinBy(o => o.Market).GetPaging(request.PageIndex, request.PageSize);
			if (mchs == null) return result.BadRequest("Can not find result");

            var oids = mchs.Select(m => m.MkrOrdId).Union(mchs.Select(m => m.TkrOrdId)).Distinct().ToList();
            var ords = await Repository.Query(o => oids.Contains(o.Id)).JoinBy(o => o.Owner).GetList();
			if (ords == null) return result.BadRequest("Can not find result");

            foreach (var m in mchs)
            {
                m.MakerOrder = ords.FirstOrDefault(o => o.Id == m.MkrOrdId);
                m.Maker = m.MakerOrder?.Owner;
                m.TakerOrder = ords.FirstOrDefault(o => o.Id == m.TkrOrdId);
                m.Taker = m.TakerOrder?.Owner;
            }

            var data = Mapper.MapPagine<Match, MatchFillResponse>(mchs);
			return result.Ok(data);
		}
		catch (Exception ex)
		{
			return result.Error(ex);
		}
	}
}