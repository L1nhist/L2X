using System.Linq.Expressions;

namespace L2X.Core.Validations;

public interface IEvaluator<TVal, TFld>
{
    IValidator<TVal> Validator { get; }

    IEvaluator<TVal, TFld> Refresh();

    IEvaluator<TVal, TFld> Restart(TVal? value);

    IEvaluator<TVal, TFld> Eval(Expression<Func<TFld, bool>> expression, bool allowEmpty = false);

    IEvaluator<TVal, TFld> Then(string message = "");

    IEvaluator<TVal, TFld> Else(string message = "");

    IEvaluator<TVal, TFld> IsEmpty();

    IEvaluator<TVal, TFld> NotEmpty();

    IEvaluator<TVal, TFld> EqualsAny(params TFld[] values);

    IEvaluator<TVal, TFld> EqualsNone(params TFld[] values);

    IEvaluator<TVal, TFld> ContainsAny(params string[] values);

    IEvaluator<TVal, TFld> ContainsNone(params string[] values);
}