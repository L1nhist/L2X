using AutoMapper.QueryableExtensions;
using L2X.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

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
 //   private static Expression<Func<TEnt, object>>? BuildSelector(string field, object value)
 //   {
 //       try
 //       {
 //           var prop = typeof(TEnt).GetProperty(field);
 //           if (prop == null) return null;

 //           var param = Expression.Parameter(typeof(TEnt));
 //           var cast = Expression.Convert(Expression.Property(param, prop), typeof(object));
 //           return Expression.Lambda<Func<TEnt, object>>(cast, param);
 //       }
 //       catch { }

 //       return null;
	//}

	//private static Expression<Func<SetPropertyCalls<TEnt>, SetPropertyCalls<TEnt>>> CreateSetProp<TFld>(Expression<Func<TEnt, TFld>> selector, TFld value)
	//{
	//	var param = Expression.Parameter(typeof(SetPropertyCalls<TEnt>));
	//	var method = typeof(SetPropertyCalls<TEnt>).GetMethods()
	//				.Where(info => info.Name == nameof(SetPropertyCalls<TEnt>.SetProperty))
	//				.Where(info => info.GetParameters() is [_, { ParameterType.IsConstructedGenericType: false }])
	//				.Single();

	//	// construct appropriately typed generic SetProperty
	//	var generic = method.MakeGenericMethod(selector.Type.GetGenericArguments()[1]);
	//	var caller = Expression.Call(param, generic, selector, Expression.Constant(value));
	//	return Expression.Lambda<Func<SetPropertyCalls<TEnt>, SetPropertyCalls<TEnt>>>(caller, param);
	//}

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
    {
        var result = await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
        return EndQuery(result);
    }

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
	public async Task<int> UpdateBy(Expression<Func<SetPropertyCalls<TEnt>, SetPropertyCalls<TEnt>>>? setter)
        => setter == null ? 0 : EndQuery(await GetQuery().ExecuteUpdateAsync(setter));

	/// <inheritdoc/>
	public async Task<int> UpdateBy(Action<SetPropertyBuilder<TEnt>> builder)
		=> builder == null ? 0 : await UpdateBy(SetPropertyBuilder.New(builder));

	/// <inheritdoc/>
	public async Task<int> UpdateBy(string? field, object? value)
        => Util.IsEmpty(field) ? 0 : await UpdateBy(SetPropertyBuilder.New<TEnt>(field, value));

    /// <inheritdoc/>
    public async Task<int> UpdateBy<TFld>(Expression<Func<TEnt, TFld?>>? selector, TFld? value)
        => selector == null ? 0 : await UpdateBy(SetPropertyBuilder.New(selector, value));

	/// <inheritdoc/>
	public async Task<int> Delete(int top = 0)
    {
        var query = GetQuery();

        if (top > 0)
            query = query.Take(top);

        if (typeof(TEnt).IsInstanceOfType(typeof(IRemovable)))
            return await UpdateBy(e => ((IRemovable)e).IsDeleted, true);

        return EndQuery(await Delete(await query.ToListAsync()));
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
    public async Task<Pagination<TEnt>> GetPaging(int? page = 0, int? size = 15)
    {
        var pg = page == null || page < 1 ? 0 : page.Value;
        var sz = size == null || size < 0 ? 15 : size.Value;
        var query = GetQuery();
        var cnt = sz > 0 ? await query.CountAsync() : 0;
        if (size > 0)
            query = query.Skip(pg * sz).Take(sz);

        var result = new Pagination<TEnt>(pg, sz, cnt, await query.ToArrayAsync());
        return EndQuery(result);
    }

    /// <inheritdoc/>
    public async Task<Pagination<TMap>> GetPaging<TMap>(int? page = 0, int? size = 15)
    {
        var pg = page == null || page < 1 ? 0 : page.Value;
        var sz = size == null || size < 0 ? 15 : size.Value;
        var query = GetQuery();
        var cnt = sz > 0 ? await query.CountAsync() : 0;
        if (size > 0)
            query = query.Skip(pg * sz).Take(sz);

        var result = new Pagination<TMap>(pg, sz, cnt, await query.ProjectTo<TMap>(_mapper.ConfigurationProvider).ToArrayAsync());
        return EndQuery(result);
    }
    #endregion
}