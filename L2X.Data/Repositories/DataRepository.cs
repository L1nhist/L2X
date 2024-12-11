using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using L2X.Core.Extensions;

namespace L2X.Data.Repositories;

public class DataRepository<TEnt>(DbContext context, IMapper mapper) : IRepository<TEnt>
    where TEnt : class, IEntity
{
    #region Properties
    private readonly DbContext _context = context ?? throw new NullReferenceException("DbContext can not be null");

    private readonly IMapper _mapper = mapper;

    private IQueryable<TEnt>? _query = null;
    #endregion

    #region Privates
    private static Expression<Func<TEnt, object>>? BuildSelector(string field, object value)
    {
        try
        {
            var prop = typeof(TEnt).GetProperty(field);
            if (prop == null) return null;

            var param = Expression.Parameter(typeof(TEnt));
            var cast = Expression.Convert(Expression.Property(param, prop), typeof(object));
            return Expression.Lambda<Func<TEnt, object>>(cast, param);
        }
        catch { }

        return null;
    }

    private IQueryable<TEnt> GetQuery()
        => _query ??= _context.Set<TEnt>().AsQueryable();

    private T EndQuery<T>(T value)
    {
        _query = null;
        return value;
    }
    #endregion

    #region Overridens
    public void Dispose()
    {
        EndQuery(0);
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }

    public IRepository<TEnt> IgnoreFilter()
    {
        _query = GetQuery().IgnoreQueryFilters();
        return this;
    }

    /// <inheritdoc/>
    public IRepository<TEnt> Query(Expression<Func<TEnt, bool>>? where, bool splitQuery = true, bool tracking = false)
    {
        _query = GetQuery();
        _query = splitQuery ? _query.AsSplitQuery() : _query.AsSingleQuery();
        _query = tracking ? _query.AsTracking() : _query.AsNoTracking();

        if (where != null)
            _query = _query.Where(where);

        return this;
    }

    /// <inheritdoc/>
    public IRepository<TEnt> SortBy(params string[] flds)
    {
		var props = typeof(TEnt).GetProps(flds);
        if (Util.IsEmpty(props)) return this;

        _query = GetQuery();
        foreach (var p in props)
        {
            var selector = p.BuildGetter<TEnt>();
            if (selector == null) continue;

            if (flds?.Any(f => $"-{p.Name}".Equals(f?.Trim(), StringComparison.OrdinalIgnoreCase)) == true)
            {
                _query = _query.OrderByDescending(selector);
            }
            else
            {
                _query = _query.OrderBy(selector);
            }
        }

		return this;
    }

    /// <inheritdoc/>
    public IRepository<TEnt> SortBy<TFld>(Expression<Func<TEnt, TFld>> selection, bool ascending)
    {
        _query = ascending ? GetQuery().OrderBy(selection) : GetQuery().OrderByDescending(selection);
        return this;
    }

    /// <inheritdoc/>
    public IRepository<TEnt> JoinBy(params string[] flds)
    {
        _query = GetQuery();
        foreach (var fld in flds)
        {
            if (!Util.IsEmpty(fld))
                _query = _query.Include(fld);
        }

        return this;
    }

    /// <inheritdoc/>
    public IRepository<TEnt> JoinBy<TFld>(Expression<Func<TEnt, TFld>> selection)
    {
        _query = GetQuery().Include(selection);
        return this;
    }

    /// <inheritdoc/>
    public async Task<int> Commit()
        => EndQuery(await _context.SaveChangesAsync());

    /// <inheritdoc/>
    public async Task<int> Count()
		=> EndQuery(await GetQuery().CountAsync());

    /// <inheritdoc/>
    public async Task<int> Min(Expression<Func<TEnt, int>> selector)
		=> EndQuery(await GetQuery().MinAsync(selector));

	/// <inheritdoc/>
	public async Task<long> Min(Expression<Func<TEnt, long>> selector)
		=> EndQuery(await GetQuery().MinAsync(selector));

	/// <inheritdoc/>
	public async Task<double> Min(Expression<Func<TEnt, double>> selector)
		=> EndQuery(await GetQuery().MinAsync(selector));

	/// <inheritdoc/>
	public async Task<decimal> Min(Expression<Func<TEnt, decimal>> selector)
		=> EndQuery(await GetQuery().MinAsync(selector));

	/// <inheritdoc/>
	public async Task<DateTime> Min(Expression<Func<TEnt, DateTime>> selector)
		=> EndQuery(await GetQuery().MinAsync(selector));

	/// <inheritdoc/>
	public async Task<int> Max(Expression<Func<TEnt, int>> selector)
		=> EndQuery(await GetQuery().MaxAsync(selector));

	/// <inheritdoc/>
	public async Task<long> Max(Expression<Func<TEnt, long>> selector)
		=> EndQuery(await GetQuery().MaxAsync(selector));

	/// <inheritdoc/>
	public async Task<double> Max(Expression<Func<TEnt, double>> selector)
		=> EndQuery(await GetQuery().MaxAsync(selector));

	/// <inheritdoc/>
	public async Task<decimal> Max(Expression<Func<TEnt, decimal>> selector)
		=> EndQuery(await GetQuery().MaxAsync(selector));

	/// <inheritdoc/>
	public async Task<DateTime> Max(Expression<Func<TEnt, DateTime>> selector)
		=> EndQuery(await GetQuery().MaxAsync(selector));

	/// <inheritdoc/>
	public async Task<int> Sum(Expression<Func<TEnt, int>> selector)
		=> EndQuery(await GetQuery().SumAsync(selector));

	/// <inheritdoc/>
	public async Task<long> Sum(Expression<Func<TEnt, long>> selector)
		=> EndQuery(await GetQuery().SumAsync(selector));

	/// <inheritdoc/>
	public async Task<double> Sum(Expression<Func<TEnt, double>> selector)
		=> EndQuery(await GetQuery().SumAsync(selector));

	/// <inheritdoc/>
	public async Task<decimal> Sum(Expression<Func<TEnt, decimal>> selector)
		=> EndQuery(await GetQuery().SumAsync(selector));

	/// <inheritdoc/>
	public async Task<int> Insert(TEnt? ent)
    {
        if (ent == null) return 0;

        await _context.Set<TEnt>().AddAsync(ent);
        return await Commit();
    }

    /// <inheritdoc/>
    public async Task<int> Insert(IEnumerable<TEnt>? ents)
    {
        if (Util.IsEmpty(ents)) return 0;

        await _context.Set<TEnt>().AddRangeAsync(ents);
        return await Commit();
    }

    /// <inheritdoc/>
    public async Task<int> Update(TEnt? ent)
    {
        if (ent == null) return 0;

        _context.Set<TEnt>().Update(ent);
        return await Commit();
    }

    /// <inheritdoc/>
    public async Task<int> Update(IEnumerable<TEnt>? ents)
    {
        if (Util.IsEmpty(ents)) return 0;

        _context.Set<TEnt>().UpdateRange(ents);
        return await Commit();
    }

    /// <inheritdoc/>
    public async Task<int> UpdateBy(string field, object value)
        => await UpdateBy(BuildSelector(field, value), value);

    /// <inheritdoc/>
    public async Task<int> UpdateBy<TFld>(Expression<Func<TEnt, TFld>>? selector, TFld value)
        => EndQuery(selector == null ? 0 : await GetQuery().ExecuteUpdateAsync(s => s.SetProperty(selector.Compile(), value)));

    /// <inheritdoc/>
    public async Task<int> Delete(bool firstOnly = false)
    {
        _query = GetQuery();

        if (firstOnly)
            return EndQuery(await Delete(await _query.FirstOrDefaultAsync()));

        if (typeof(TEnt).IsInstanceOfType(typeof(IRemovable)))
            return await UpdateBy(e => ((IRemovable)e).IsDeleted, true);

        return EndQuery(await Delete(await _query.ToListAsync()));
    }

    /// <inheritdoc/>
    public async Task<int> Delete(TEnt? ent)
    {
        if (ent == null) return 0;
        if (ent is IRemovable rem)
        {
            rem.IsDeleted = true;
            _context.Update(ent);
        }
        else
        {
            _context.Remove(ent);
        }

        return await Commit();
    }

    /// <inheritdoc/>
    public async Task<int> Delete(IEnumerable<TEnt>? ents)
    {
        if (Util.IsEmpty(ents)) return EndQuery(0);
        if (typeof(TEnt).IsInstanceOfType(typeof(IRemovable)))
        {
            foreach (var e in ents)
            {
                ((IRemovable)e).IsDeleted = true;
            }
            _context.Update(ents);
        }
        else
		{
			_context.RemoveRange(ents);
		}

        return await Commit();
    }

    /// <inheritdoc/>
    public async Task<int> DeleteAll()
        => EndQuery(await GetQuery().ExecuteDeleteAsync());

    /// <inheritdoc/>
    public async Task<TEnt?> GetByKeys(params object[] keys)
        => Util.IsEmpty(keys) || !keys.Any(k => k != null) ? default : await _context.FindAsync<TEnt>(keys);

    /// <inheritdoc/>
    public async Task<TEnt?> GetFirst()
        => EndQuery(await GetQuery().FirstOrDefaultAsync());

    /// <inheritdoc/>
    public async Task<TMap?> GetFirst<TMap>()
        => EndQuery(await GetQuery().ProjectTo<TMap>(_mapper.ConfigurationProvider).FirstOrDefaultAsync());

    /// <inheritdoc/>
    public async Task<List<TEnt>> GetList(int top = 0)
    {
        var query = GetQuery();
        if (top > 0)
            query = query.Take(top);

        return EndQuery(await query.ToListAsync());
    }

    /// <inheritdoc/>
    public async Task<List<TMap>> GetList<TMap>(int top = 0)
    {
        var query = GetQuery();
        if (top > 0)
            query = query.Take(top);

        return EndQuery(await query.ProjectTo<TMap>(_mapper.ConfigurationProvider).ToListAsync());
    }

    /// <inheritdoc/>
    public async Task<Pagination<TEnt>> GetPaging(int page = 0, int size = 15)
    {
        page = page < 1 ? 0 : page;
        size = size < 1 ? 0 : size;
        var query = GetQuery();
        if (size > 0)
            query = query.Skip(page * size).Take(size);

        var result = new Pagination<TEnt>(page, size, size > 0 ? await query.CountAsync() : 0, await query.ToArrayAsync());
        return EndQuery(result);
    }

    /// <inheritdoc/>
    public async Task<Pagination<TMap>> GetPaging<TMap>(int page = 0, int size = 15)
    {
        page = page < 1 ? 0 : page;
        size = size < 1 ? 0 : size;
        var query = GetQuery();
        if (size > 0)
            query = query.Skip(page * size).Take(size);

        var result = new Pagination<TMap>(page, size, size > 0 ? await query.CountAsync() : 0, await query.ProjectTo<TMap>(_mapper.ConfigurationProvider).ToArrayAsync());
        return EndQuery(result);
    }
    #endregion
}