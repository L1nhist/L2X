namespace L2X.Core.Validations;

public interface IValidator<T>
{
    string ErrorMessage { get; }

    void AddError(string field, string message);

    IEvaluator<T, TFld> For<TFld>(Expression<Func<T, TFld>> selector);

    bool Validate(T? value);
}