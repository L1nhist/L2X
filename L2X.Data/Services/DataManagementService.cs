using Microsoft.Extensions.Logging;
using L2X.Data.Extensions;
using L2X.Data.Repositories;
using L2X.Data.Requests;
using L2X.Data.Results;

namespace L2X.Data.Services;

public class DataManagementService<T> : IDataManagementService<T>
    where T : class, IEntity
{
    protected ILogger Logger { get; }

    protected IMapper Mapper { get; }

    public IRepository<T> Repository { get; }

    public DataManagementService(ILoggerFactory logFactory, IMapper mapper, IRepository<T> repository)
    {
        Logger = logFactory.CreateLogger(GetType());
        Mapper = mapper;
        Repository = repository;
    }

    protected virtual void BuildQuery<TFilt>(TFilt? filter)
        where TFilt : FilterRequest
    {
        if (!Util.IsEmpty(filter?.OrderBy))
            Repository.SortBy(filter.OrderBy.ToArray());
    }

    public async Task<IResult> Create<TReq>(TReq request)
        where TReq : IRequest
    {
        var result = Result.New();
        try
        {
            var valid = await Validate(request);
            if (!valid.Success) return valid;

            if (await Repository.Insert(valid.Data) > 0) return result.Ok();
        }
        catch (Exception ex)
        {
            WriteLog(ex);
            return result.Error(ex.Message);
        }

        return result.BadRequest("Request can not be execute");
    }

    public async Task<IResult> Update<TReq>(TReq request)
        where TReq : IRequest
    {
        var result = new BaseResult();
        try
        {
            var valid = await Validate(request);
            if (!valid.Success) return valid;

            var upd = await Repository.GetByKeys(valid.Data.Keys);
            if (upd == null) return result.NotFound("Can not find specific data");

            Mapper.Map(valid.Data, upd);
            if (await Repository.Update(upd) > 0) return result.Ok();
        }
        catch (Exception ex)
        {
            WriteLog(ex);
            return result.Error(ex.Message);
        }

        return result.BadRequest("Request can not be execute");
    }

    public async Task<IResult> Delete<TReq>(TReq request)
        where TReq : IRequest
    {
        var result = new BaseResult();
        try
        {
            var valid = await Validate(request);
            if (!valid.Success) return valid;

            var del = await Repository.GetByKeys(valid.Data.Keys);
            if (del == null) return result.NotFound("Can not find specific data");
            if (await Repository.Delete(del) > 0) return result.Ok();
        }
        catch (Exception ex)
        {
            WriteLog(ex);
            return result.Error(ex);
        }

        return result.BadRequest("Request can not be execute");
    }

    public async Task<IResult<TRes>> Select<TReq, TRes>(TReq request)
        where TReq : IRequest
    {
        var result = Result.New<TRes>();
        try
        {
            var valid = await Validate(request);
            if (!valid.Success) return result.Error(valid.Message, valid.Code);

            var ent = await Repository.GetByKeys(valid.Data.Keys);
            if (ent == null) return result.NotFound("Can not find specific data");

            var data = Mapper.Map<TRes>(ent);
            if (data != null) return result.Ok(data);
        }
        catch (Exception ex)
        {
            WriteLog(ex);
            return result.Error(ex.Message);
        }
        
        return result.BadRequest("Result data can not be mapped as usual");
    }

    public async Task<IResult<IEnumerable<TRes>>> Search<TReq, TRes>(TReq request)
        where TReq : FilterRequest
    {
        var result = Result.NewList<TRes>();
        try
        {
            if (request != null) BuildQuery(request);

            var ents = await Repository.GetList<TRes>();
            if (Util.IsEmpty(ents)) return result.NotFound("Can not find specific data");
            return result.Ok(ents);
        }
        catch (Exception ex)
        {
            WriteLog(ex);
            return result.Error(ex.Message);
        }
    }

    public async Task<PagedResult<TRes>> Paginate<TReq, TRes>(TReq request)
        where TReq : PagingRequest
    {
        var result = Result.NewPage<TRes>();
        try
        {
            if (request != null) BuildQuery(request);

            var ents = await Repository.GetPaging<TRes>(request?.PageIndex ?? 0, request?.PageSize ?? 0);
            if (Util.IsEmpty(ents)) return result.NotFound("Can not find specific data");
            return result.Ok(ents);
        }
        catch (Exception ex)
        {
            WriteLog(ex);
            return result.Error(ex.Message);
        }
    }

    public virtual Task<ValidResult<T>> Validate<TReq>(TReq? request)
    {
        var valid = new ValidResult<T>();
        if (Util.IsEmpty(request)) return Task.FromResult(valid.BadRequest("Requested data can not be mapped as usual"));

        var ent = Mapper.Map<T>(request);
        return Task.FromResult(valid.IsValid(ent, "Can not find specific data"));
    }

    public void WriteLog(string? msg = "")
    {
        if (!Util.IsEmpty(msg, true))
            Logger.LogWarning(msg);
    }

    public void WriteLog(Exception ex)
    {
        if (ex != null)
            Logger.LogError(ex, "");
    }
}

public class DataManagementService<TEnt, TId>(ILoggerFactory logFactory, IMapper mapper, IRepository<TEnt> repository)
    : DataManagementService<TEnt>(logFactory, mapper, repository), IDataManagementService<TEnt, TId>
    where TEnt : class, IEntity<TId>
{
    public new async Task<IResult<TId>> Create<TReq>(TReq request)
        where TReq : IRequest
    {
        var result = Result.New<TId>();
        try
        {
            var valid = await Validate(request);
            if (!valid.Success) return result.Error(valid.Message, valid.Code);

            if (await Repository.Insert(valid.Data) > 0) return result.Ok(valid.Data.Id);
        }
        catch (Exception ex)
        {
            WriteLog(ex);
            return result.Error(ex.Message);
        }

        return result.BadRequest("Request can not be execute");
    }

    public async Task<IResult> Update<TReq>(TId id, TReq request)
        where TReq : IRequest
    {
        var result = new BaseResult();
        try
        {
            var valid = await Validate(request);
            if (!valid.Success) return result.Error(valid.Message, valid.Code);

            var ent = await Repository.GetById(id);
            if (ent == null) return result.NotFound("Can not find specific data");

            ent = Mapper.Map(request, ent);
            if (await Repository.Update(ent) > 0) return result.Ok();
        }
        catch (Exception ex)
        {
            WriteLog(ex);
            return result.Error(ex.Message);
        }

        return result.BadRequest("Request can not be execute");
    }

    public async Task<IResult> Delete(params TId[] ids)
    {
        var result = new BaseResult();
        try
        {
            if (await Repository.DeleteByIds(ids) > 0) return result.Ok();
        }
        catch (Exception ex)
        {
            WriteLog(ex);
            return result.Error(ex.Message);
        }

        return result.BadRequest("Request can not be execute");
    }

    public async Task<IResult<TRes>> Select<TRes>(TId id)
    {
        var result = Result.New<TRes>();
        try
        {
            var ent = await Repository.GetById(id);
            if (ent == null) return result.NotFound("Can not find specific data");

            var data = Mapper.Map<TRes>(ent);
            if (data != null) return result.Ok(data);
        }
        catch (Exception ex)
        {
            WriteLog(ex);
            return result.Error(ex.Message);
        }
        
        return result.BadRequest("Result data can not be mapped as usual");
    }

    public async Task<IResult<IEnumerable<TRes>>> Select<TRes>(params TId[] ids)
    {
        var result = Result.NewList<TRes>();
        try
        {
            var ents = await Repository.GetListByIds(ids);
            if (Util.IsEmpty(ents)) return result.NotFound("Can not find specific data");

            var data = Mapper.Map<IEnumerable<TRes>>(ents);
            if (data != null) return result.Ok(data);
        }
        catch (Exception ex)
        {
            WriteLog(ex);
            return result.Error(ex.Message);
        }

        return result.BadRequest("Result data can not be mapped as usual");
    }
}