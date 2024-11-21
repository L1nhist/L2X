using System.Net;
using L2X.Core.Models;
using L2X.Data.Entities;

namespace L2X.Services.Results;

public class PagedResult<T> : BaseResult, IResult<IEnumerable<T>>, IPagination
{
    private Pagination<T>? _items = default;

    public IEnumerable<T>? Data => _items;

    public int PageIndex => _items?.PageIndex ?? 0;

    public int PageSize => _items?.PageSize ?? 1;

    public int PageCount => _items?.PageCount ?? 0;

    public int TotalCount => _items?.TotalCount ?? 0;

    #region Overridens
    IResult<IEnumerable<T>> IResult<IEnumerable<T>>.Ok(IEnumerable<T>? data, string message = "")
        => Ok(data, message);

    IResult<IEnumerable<T>> IResult<IEnumerable<T>>.Error(string message = "", HttpStatusCode code = HttpStatusCode.InternalServerError)
        => Error(message, code);

    IResult<IEnumerable<T>> IResult<IEnumerable<T>>.Error(Exception exception, HttpStatusCode code = HttpStatusCode.InternalServerError)
        => Error(exception.Message, code);
    #endregion

    public PagedResult<T> Ok(IEnumerable<T>? data, string message = "")
    {
        if (data is not Pagination<T> pagin)
            pagin = new Pagination<T>(0, 1, data?.Count() ?? 0, data);

        _items = pagin;
        return (PagedResult<T>)Ok(message);
    }

    public new PagedResult<T> Error(string message = "", HttpStatusCode code = HttpStatusCode.InternalServerError)
        => (PagedResult<T>)base.Error(message, code);

    public new PagedResult<T> Error(Exception exception, HttpStatusCode code = HttpStatusCode.InternalServerError)
        => (PagedResult<T>)base.Error(exception.Message, code);
}