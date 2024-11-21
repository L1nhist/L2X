using System.Net;
using L2X.Data.Entities;
using L2X.Services.Results;

namespace L2X.Services.Extensions;

public static class Result
{
    #region For New IResult
    public static IResult New()
        => new BaseResult();

    public static IResult<T> New<T>()
        => new BaseResult<T>();

    public static IResult<IEnumerable<T>> NewList<T>()
        => new BaseResult<IEnumerable<T>>();

    public static PagedResult<T> NewPage<T>()
        => new();

    public static IResult Ok(string message = "")
        => new BaseResult().Ok(message);

    public static IResult<T> Ok<T>(T? data, string message = "")
        => new BaseResult<T>().Ok(data, message);

    public static PagedResult<T> Ok<T>(IEnumerable<T>? data, int page, int size, string message = "")
        => new PagedResult<T>().Ok(new Pagination<T>(page, size, data?.Count() ?? 0, data), message);

    public static PagedResult<T> Ok<T>(Pagination<T>? data, string message = "")
        => new PagedResult<T>().Ok(data, message);

    public static IResult Error(string message = "", HttpStatusCode code = HttpStatusCode.InternalServerError)
        => new BaseResult().Error(message, code);

    public static IResult<T> Error<T>(string message = "", HttpStatusCode code = HttpStatusCode.InternalServerError)
        => new BaseResult<T>().Error(message, code);
    #endregion

    #region For IResult Only
    public static IResult BadRequest(string message = "")
        => Error(message, HttpStatusCode.BadRequest);

    public static IResult Conflict(string message = "")
        => Error(message, HttpStatusCode.Conflict);

    public static IResult Forbidden(string message = "")
        => Error(message, HttpStatusCode.Forbidden);

    public static IResult NotFound(string message = "")
        => Error(message, HttpStatusCode.NotFound);

    public static IResult Unauthorized(string message = "")
        => Error(message, HttpStatusCode.Unauthorized);

    public static IResult BadRequest(this IResult result, string message = "")
        => result.Error(message, HttpStatusCode.BadRequest);

    public static IResult Conflict(this IResult result, string message = "")
        => result.Error(message, HttpStatusCode.Conflict);

    public static IResult Forbidden(this IResult result, string message = "")
        => result.Error(message, HttpStatusCode.Forbidden);

    public static IResult NotFound(this IResult result, string message = "")
        => result.Error(message, HttpStatusCode.NotFound);

    public static IResult Unauthorized(this IResult result, string message = "")
        => result.Error(message, HttpStatusCode.Unauthorized);
    #endregion

    #region For Generic IResult
    public static IResult<T> BadRequest<T>(string message = "")
        => Error<T>(message, HttpStatusCode.BadRequest);

    public static IResult<T> Conflict<T>(string message = "")
        => Error<T>(message, HttpStatusCode.Conflict);

    public static IResult<T> Forbidden<T>(string message = "")
        => Error<T>(message, HttpStatusCode.Forbidden);

    public static IResult<T> NotFound<T>(string message = "")
        => Error<T>(message, HttpStatusCode.NotFound);

    public static IResult<T> Unauthorized<T>(string message = "")
        => Error<T>(message, HttpStatusCode.Unauthorized);

    public static IResult<T> BadRequest<T>(this IResult<T> result, string message = "")
        => result.Error(message, HttpStatusCode.BadRequest);

    public static IResult<T> Conflict<T>(this IResult<T> result, string message = "")
        => result.Error(message, HttpStatusCode.Conflict);

    public static IResult<T> Forbidden<T>(this IResult<T> result, string message = "")
        => result.Error(message, HttpStatusCode.Forbidden);

    public static IResult<T> NotFound<T>(this IResult<T> result, string message = "")
        => result.Error(message, HttpStatusCode.NotFound);

    public static IResult<T> Unauthorized<T>(this IResult<T> result, string message = "")
        => result.Error(message, HttpStatusCode.Unauthorized);
    #endregion

    #region For PagedResult
    public static PagedResult<T> BadRequest<T>(this PagedResult<T> result, string message = "")
        => result.Error(message, HttpStatusCode.BadRequest);

    public static PagedResult<T> Conflict<T>(this PagedResult<T> result, string message = "")
        => result.Error(message, HttpStatusCode.Conflict);

    public static PagedResult<T> Forbidden<T>(this PagedResult<T> result, string message = "")
        => result.Error(message, HttpStatusCode.Forbidden);

    public static PagedResult<T> NotFound<T>(this PagedResult<T> result, string message = "")
        => result.Error(message, HttpStatusCode.NotFound);

    public static PagedResult<T> Unauthorized<T>(this PagedResult<T> result, string message = "")
        => result.Error(message, HttpStatusCode.Unauthorized);
    #endregion
}