using System.Net;

namespace L2X.Services.Results;

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
    public T? Data { get; private set; } = default;

    public IResult<T> Ok(T? data, string message = "")
    {
        Data = data;
        return (IResult<T>)Ok(message);
    }

    public new IResult<T> Error(string message = "", HttpStatusCode code = HttpStatusCode.InternalServerError)
        => (IResult<T>)base.Error(message, code);

    public new IResult<T> Error(Exception exception, HttpStatusCode code = HttpStatusCode.InternalServerError)
        => (IResult<T>)base.Error(exception.Message, code);
}