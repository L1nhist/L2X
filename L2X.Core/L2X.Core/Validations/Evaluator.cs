namespace L2X.Core.Validations;

public class Evaluator<TVal, TFld> : IEvaluator<TVal, TFld>, IEquatable<bool>
{
    #region Operators
    public static bool operator true(Evaluator<TVal, TFld> validate)
        => validate._error;

    public static bool operator false(Evaluator<TVal, TFld> validate)
        => !validate._error;

    public static bool operator !(Evaluator<TVal, TFld> validate)
        => !validate._error;

    public static bool operator |(Evaluator<TVal, TFld> lft, Evaluator<TVal, TFld> rgt)
        => lft._error || rgt._error;

    public static bool operator &(Evaluator<TVal, TFld> lft, Evaluator<TVal, TFld> rgt)
        => lft._error && rgt._error;

    public static bool operator ==(Evaluator<TVal, TFld> lft, Evaluator<TVal, TFld> rgt)
        => lft._error == rgt._error;

    public static bool operator !=(Evaluator<TVal, TFld> lft, Evaluator<TVal, TFld> rgt)
        => lft._error != rgt._error;

    public static bool operator ==(Evaluator<TVal, TFld> lft, bool rgt)
        => lft._error == rgt;

    public static bool operator !=(Evaluator<TVal, TFld> lft, bool rgt)
        => lft._error != rgt;
    #endregion

    #region Properties
    private bool _empty;

    private bool _error;

    private bool _raise;

    private readonly Func<TVal, TFld> _selector;

    public string Name { get; }

    public TFld? Value { get; private set; }

    public IValidator<TVal> Validator { get; }
    #endregion

    internal Evaluator(IValidator<TVal> validator, Func<TVal, TFld> selector, TVal? value, string name)
    {
        _selector = selector;
        Validator = validator;
        Name = name;
        Restart(value);
    }

    #region Overridens
    /// <inheritdoc/>
    public IEvaluator<TVal, TFld> Eval(Expression<Func<TFld, bool>> expression, bool allowEmpty = false)
    {
        if ((allowEmpty || !_empty) && !_error)
            _error = expression.Compile().Invoke(Value!);

        return this;
    }

    /// <inheritdoc/>
    public IEvaluator<TVal, TFld> Refresh()
    {
        _error = false;
        _raise = false;
        return this;
    }

    /// <inheritdoc/>
    public IEvaluator<TVal, TFld> Restart(TVal? value)
    {
        Value = Util.IsEmpty(value) ? default : _selector.Invoke(value);
        _empty = Util.IsEmpty(value);
        return Refresh();
    }

    /// <inheritdoc/>
    public IEvaluator<TVal, TFld> Then(string message)
    {
        if (_error && !_raise)
        {
            _raise = true;
            if (!Util.IsEmpty(message))
                Validator?.AddError(Name, message);
        }

        return this;
    }

    /// <inheritdoc/>
    public IEvaluator<TVal, TFld> Else(string message)
    {
        if (!_error && !_raise)
        {
            _raise = true;
            if (!Util.IsEmpty(message))
                Validator?.AddError(Name, message);
        }

        return this;
    }

    /// <inheritdoc/>
    public IEvaluator<TVal, TFld> IsEmpty()
    {
        if (!_error)
            _error = _empty;

        return this;
    }

    /// <inheritdoc/>
    public IEvaluator<TVal, TFld> NotEmpty()
    {
        if (!_error)
            _error = !_empty;

        return this;
    }

    /// <inheritdoc/>
    public IEvaluator<TVal, TFld> EqualsAny(params TFld[] values)
        => Eval(e => values.Any(v => e == null ? v == null : e.Equals(v)));

    /// <inheritdoc/>
    public IEvaluator<TVal, TFld> EqualsNone(params TFld[] values)
        => Eval(e => values.Any(v => e == null ? v != null : !e.Equals(v)));

    /// <inheritdoc/>
    public IEvaluator<TVal, TFld> ContainsAny(params string[] values)
        => Eval(e => values.Any(v => Util.IsContains(e, v, false, false)));

    /// <inheritdoc/>
    public IEvaluator<TVal, TFld> ContainsNone(params string[] values)
        => Eval(e => values.Any(v => Util.IsContains(e, v, false, false)));

    /// <inheritdoc/>
    public bool Equals(bool value)
        => _error.Equals(value);

    public override bool Equals(object value)
        => value != null && _error.Equals(value);

    public override int GetHashCode()
        => _error.GetHashCode();
    #endregion
}