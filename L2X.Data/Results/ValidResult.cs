using L2X.Core.Validations;
using L2X.Data.Extensions;

namespace L2X.Data.Results;

public class ValidResult<T> : BaseResult<T>
{
    public Dictionary<string, string> Errors { get; } = [];

    public bool IsEmpty { get; private set; } = false;

    public new string Message
    {
        get
        {
            var msg = "";
            foreach (var d in Errors)
            {
                msg += $"{d.Key}: {d.Value};";
            }
            return msg;
        }
    }

    public void AddError(string field, string message)
    {
        base.Error("", HttpStatusCode.BadRequest);
        Errors.Add(field, message);
    }

    public ValidResult<T> IsValid(T? data, string message = "")
    {
        if (Util.IsEmpty(data))
        {
            IsEmpty = true;
            return this.NotFound(message);
        }
        else
        {
            IsEmpty = false;
            return (ValidResult<T>)Ok(data);
        }
    }

    public ValidResult<T> IsValid(IValidator<T> validator)
    {
        if (IsEmpty) return this;
        if (!validator.Validate(Data))
        {
            return this.BadRequest();
        }
        
        return (ValidResult<T>)Ok();
    }
}