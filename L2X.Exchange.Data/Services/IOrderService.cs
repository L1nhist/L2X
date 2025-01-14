using L2X.Data.Results;
using L2X.Data.Services;
using L2X.Exchange.Data.Entities;

namespace L2X.Exchange.Data.Services;

public interface IOrderService : IDataManagementService<Order, Guid>
{
    Task<IEnumerable<PreOrder>> GetPreOrders(int top);

    Task<IResult<OrderResponse>> GetByIdOrIndex(string value);

    Task<PagedResult<OrderResponse>> PaginateByState(bool side, PagingRequest request);

	Task<PagedResult<OrderResponse>> PaginateByFail(PagingRequest request);

	Task<PagedResult<PreOrderResponse>> PaginateAllPre(bool side, PagingRequest request);

	Task<PagedResult<MatchFillResponse>> PaginateAllMatch(PagingRequest request);
}