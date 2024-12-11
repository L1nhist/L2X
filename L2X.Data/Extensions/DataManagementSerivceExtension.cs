using L2X.Data.Services;
using L2X.Data.Requests;
using L2X.Data.Results;

namespace L2X.Data.Extensions;

public static class DataManagementSerivceExtension
{
    public static async Task<IResult> SetPosition<TEnt, TId>(this IDataManagementService<TEnt, TId> service, TId id, int position)
        where TEnt : class, IEntity<TId>, ISortable
    {
        var resp = new BaseResult();
        try
        {
            if (await service.Repository.Query(e => e.Id.Equals(id)).UpdateBy(e => e.Position, position) > 0) return resp.Ok();
        }
        catch (Exception ex)
        {
            service.WriteLog(ex);
            return resp.Error(ex.Message);
        }

        return resp.NotFound("Can not find specific data");
    }

    public static async Task<IResult> Sort<TEnt, TId>(this IDataManagementService<TEnt, TId> service, IEnumerable<TId> ids, params int[] positions)
        where TEnt : class, IEntity<TId>, ISortable
    {
        var resp = new BaseResult();
        try
        {
            var ents = await service.Repository.GetListByIds(ids.ToArray());
            if (Util.IsEmpty(ents)) return resp.NotFound("Can not find specific data");

            var idx = -1;
            foreach (var e in ents)
            {
                idx++;
                if (idx < positions.Length)
                {
                    e.Position = positions[idx];
                }
                else
                {
                    e.Position = positions[idx] + idx - positions.Length;
                }
            }

            if (await service.Repository.Update(ents) > 0) return resp.Ok();
        }
        catch (Exception ex)
        {
            service.WriteLog(ex);
            return resp.Error(ex.Message);
        }

        return resp.BadRequest("Request can not be execute");
    }

    public static async Task<IResult> SetActived<TEnt, TId>(this IDataManagementService<TEnt, TId> service, bool active, params TId[] ids)
        where TEnt : class, IEntity<TId>, IActivable
    {
        var resp = new BaseResult();
        try
        {
            if (Util.IsEmpty(ids)) return resp.NotFound("Can not find specific data");
            if (await service.Repository.Query(e => ids.Contains(e.Id)).UpdateBy(e => e.IsActived, active) > 0) return resp.Ok();
        }
        catch (Exception ex)
        {
            service.WriteLog(ex);
            return resp.Error(ex.Message);
        }

        return resp.NotFound("Can not find specific data");
    }

    public static async Task<IResult> SetState<TEnt, TId>(this IDataManagementService<TEnt, TId> service, string state, params TId[] ids)
        where TEnt : class, IEntity<TId>, IMarkable
    {
        var resp = new BaseResult();
        try
        {
            if (Util.IsEmpty(ids)) return resp.NotFound("Can not find specific data");
            if (await service.Repository.Query(e => ids.Contains(e.Id)).UpdateBy(e => e.State, state) > 0) return resp.Ok();
        }
        catch (Exception ex)
        {
            service.WriteLog(ex);
            return resp.Error(ex.Message);
        }
        return resp.NotFound("Can not find specific data");
    }

    public static async Task<IResult<IEnumerable<TRes>>> SearchFromTrash<TEnt, TReq, TRes>(this IDataManagementService<TEnt> service, TReq request)
        where TEnt : class, IEntity, IRemovable
        where TReq : FilterRequest
    {
        service.Repository.IgnoreFilter().Query(e => e.IsDeleted);
        return await service.Search<TReq, TRes>(request);
    }

    public static async Task<PagedResult<TRes>> PaginateFromTrash<TEnt, TReq, TRes>(this IDataManagementService<TEnt> service, TReq request)
        where TEnt : class, IEntity, IRemovable
        where TReq : PagingRequest
    {
        service.Repository.IgnoreFilter().Query(e => e.IsDeleted);
        return await service.Paginate<TReq, TRes>(request);
    }
}