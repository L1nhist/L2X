using L2X.Data.Repositories;

namespace L2X.Data.Extentions;

public static class DataRepositoryExtensions
{
    public static IRepository<TEnt> Query<TEnt, TId>(this IRepository<TEnt> repository, TId id)
        where TEnt : class, IEntity<TId>
        => repository.Query(e => e.Id.Equals(id), false);

    public static IRepository<TEnt> Query<TEnt, TId>(this IRepository<TEnt> repository, params TId[] ids)
        where TEnt : class, IEntity<TId>
    {
        var all = ids?.Where(i => i != null).ToList();
        return Util.IsEmpty(all) ? repository.Query() : repository.Query(e => all.Contains(e.Id));
    }

    public static async Task<int> DeleteByIds<TEnt, TId>(this IRepository<TEnt> repository, params TId[] ids)
        where TEnt : class, IEntity<TId>
    {
        if (Util.IsEmpty(ids)) return 0;

        if (typeof(TEnt).IsInstanceOfType(typeof(IRemovable)))
            return await repository.Query(ids).UpdateBy(e => ((IRemovable)e).IsDeleted, true);
        else
            return await repository.Query(ids).DeleteAll();
    }

    public static async Task<TEnt?> GetById<TEnt, TId>(this IRepository<TEnt> repository, TId? id)
        where TEnt : class, IEntity<TId>
        => id == null ? null : await repository.GetByKeys(id);

    public static async Task<IEnumerable<TEnt>?> GetListByIds<TEnt, TId>(this IRepository<TEnt> repository, params TId[] ids)
        where TEnt : class, IEntity<TId>
        => await repository.Query(ids).GetList();

    public static async Task<int> Activate<TEnt>(this IRepository<TEnt> repository, bool state = true)
        where TEnt : class, IEntity, IActivable
        => await repository.UpdateBy(e => e.IsActived, state);

    public static async Task<int> Activate<TEnt, TId>(this IRepository<TEnt> repository, bool state, params TId[] ids)
        where TEnt : class, IEntity<TId>, IActivable
        => await repository.Query(ids).UpdateBy(e => e.IsActived, state);

    public static async Task<int> ChangePosition<TEnt, TId>(this IRepository<TEnt> repository, int pos1, int pos2)
        where TEnt : class, IEntity<TId>, ISortable
    {
        var ents = await repository.Query(e => e.Position == pos1 || e.Position == pos2, tracking: true).GetList();
        if (Util.IsEmpty(ents)) return 0;

        foreach (var e in ents)
        {
            e.Position = e.Position == pos1 ? pos2 : pos1;
        }

        return await repository.Update(ents);
    }

    public static async Task<int> SetPosition<TEnt, TId>(this IRepository<TEnt> repository, TId id, int position)
        where TEnt : class, IEntity<TId>, ISortable
        => await repository.Query(id).UpdateBy(e => e.Position, position);

    public static async Task<int> SetPosition<TEnt, TId>(this IRepository<TEnt> repository, IEnumerable<TId> ids, params int[] positions)
        where TEnt : class, IEntity<TId>, ISortable
    {
        var ents = await repository.Query(e => ids.Contains(e.Id), tracking: true).GetList();
        if (Util.IsEmpty(ents)) return 0;

        foreach (var e in ents)
        {
            var idx = -1;
            foreach (var id in ids)
            {
                idx++;
                if (e.Id.Equals(id) && positions.Length > idx)
                {
                    e.Position = positions[idx];
                    break;
                }
            }
        }

        return await repository.Update(ents);
    }
}
