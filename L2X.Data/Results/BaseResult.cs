using System.Diagnostics.CodeAnalysis;

namespace L2X.Data.Results;

public class BaseResult : IResult
{
    public bool Success => Code == HttpStatusCode.OK;

    public HttpStatusCode Code { get; private set; } = HttpStatusCode.OK;

    public string Message { get; private set; } = "";

    public IResult Ok(string message = "")
    {
        Code = HttpStatusCode.OK;
        Message = message;
        return this;
    }

    public IResult Error(string message = "", HttpStatusCode code = HttpStatusCode.InternalServerError)
    {
        Code = code;
        Message = message;
        return this;
    }

    public IResult Error(Exception exception, HttpStatusCode code = HttpStatusCode.InternalServerError)
        => Error(exception.Message, code);
}

public class BaseResult<T> : BaseResult, IResult<T>
{
    [MemberNotNullWhen(true, nameof(Data))]
    public new bool Success => Code == HttpStatusCode.OK;

    public T? Data { get; private set; } = default;

    public IResult<T> Ok(T? data, string message = "")
    {
        Data = data;
        return (IResult<T>)Ok(message);
    }

    public new IResult<T> Error(string message = "", HttpStatusCode code = HttpStatusCode.InternalServerError)
    {
        Data = default;
        return (IResult<T>)base.Error(message, code);
    }

    public new IResult<T> Error(Exception exception, HttpStatusCode code = HttpStatusCode.InternalServerError)
        => Error(exception.Message, code);
}