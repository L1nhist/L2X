using L2X.Data.Entities;
using L2X.Data.Repositories;
using L2X.Services.Requests;
using L2X.Services.Results;
using System.Linq.Expressions;

namespace L2X.Services.Managements;

public interface IDataManagementService<T>
    where T : class, IEntity
{
    IRepository<T> Repository { get; }

    Task<IResult> Create<TReq>(TReq request)
        where TReq : IRequest;

    Task<IResult> Update<TReq>(TReq request)
        where TReq : IRequest;

    Task<IResult> Delete<TReq>(TReq request)
        where TReq : IRequest;

    Task<IResult<TRes>> Select<TReq, TRes>(TReq request)
        where TReq : IRequest;

    Task<IResult<IEnumerable<TRes>>> Search<TReq, TRes>(TReq request)
        where TReq : FilterRequest;

    Task<PagedResult<TRes>> Paginate<TReq, TRes>(TReq request)
        where TReq : PagingRequest;

    Task<IResult> Validate(object? request);

    Task<IResult<TRes>> Validate<TRes>(object? request);

    void WriteLog(string msg = "");

    void WriteLog(Exception ex);
}

public interface IDataManagementService<TEnt, TId> : IDataManagementService<TEnt>
    where TEnt : class, IEntity<TId>
{
    Task<IResult<TId>> Create<TReq>(TReq request)
        where TReq : IRequest;

    Task<IResult> Update<TReq>(TId id, TReq request)
        where TReq : IRequest;

    Task<IResult> Delete(params TId[] ids);

    Task<IResult<TRes>> Select<TRes>(TId id);

    Task<IResult<IEnumerable<TRes>>> Select<TRes>(params TId[] ids);
}