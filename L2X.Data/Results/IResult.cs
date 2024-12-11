namespace L2X.Data.Results;

public interface IResult
{
    bool Success { get; }

    HttpStatusCode Code { get; }

    string Message { get; }

    IResult Ok(string message = "");

    IResult Error(string message = "", HttpStatusCode code = HttpStatusCode.InternalServerError);

    IResult Error(Exception exception, HttpStatusCode code = HttpStatusCode.InternalServerError);
}

public interface IResult<T> : IResult
{
    T? Data { get; }

    IResult<T> Ok(T? data, string message = "");

    new IResult<T> Error(string message = "", HttpStatusCode code = HttpStatusCode.InternalServerError);

    new IResult<T> Error(Exception exception, HttpStatusCode code = HttpStatusCode.InternalServerError);
}