namespace L2X.Data.Entities;

public class Pagination<T> : IPagination, IEnumerable<T>, IEnumerable
{
    private readonly IEnumerable<T> _items;

    public int PageIndex { get; }

    public int PageSize { get; }

    public int PageCount { get; }

    public int TotalCount { get; }

    public Pagination(int page, int size, int total, IEnumerable<T>? items = null)
    {
        _items = items ?? [];
        PageIndex = page < 0 || items == null ? 0 : page;
        PageSize = size < 1 ? 0 : size;
        TotalCount = items == null ? 0 : Math.Max(total, items.Count());
        PageCount = PageSize == 0 ? 1 : TotalCount / PageSize + TotalCount % PageSize > 0 ? 1 : 0;
    }

    #region Overridens
    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => _items.GetEnumerator();
    
    /// <inheritdoc/>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
        => _items.GetEnumerator();
    #endregion
}