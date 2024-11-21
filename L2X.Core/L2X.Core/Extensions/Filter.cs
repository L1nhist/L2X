using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace L2X.Core.Extensions;

/// <summary>
/// Evaluate{T} which eliminates the default 1=0 or 1=1 stub expessions
/// </summary>
/// <typeparam name="T">Type of parameter</typeparam>
public class Filter<T>
{
    #region Operators
    public static implicit operator Expression<Func<T, bool>>([NotNull] Filter<T> filter)
        => filter._exp ?? TrueExp;

    public static implicit operator Func<T, bool>([NotNull] Filter<T> filter)
        => filter.Compile();

    public static implicit operator Filter<T>([NotNull] Expression<Func<T, bool>> expession)
        => new(expession);
    #endregion

    public static readonly Expression<Func<T, bool>> TrueExp = x => true;

    public static readonly Expression<Func<T, bool>> FalseExp = x => false;

    public static Filter<T> New(Expression<Func<T, bool>>? expession = null)
        => new(expession);

    public static Filter<T> New(IEnumerable<T> enumerable, Expression<Func<T, bool>>? expession = null)
        => new(expession);

    #region Properties
    private Expression<Func<T, bool>>? _exp;

    [MemberNotNullWhen(false, nameof(_exp))]
    public bool IsEmpty => _exp == null;

    /// <summary></summary>
    public Expression? Body => _exp?.Body;

    /// <summary></summary>
    public ExpressionType NodeType => _exp?.NodeType ?? ExpressionType.New;

    /// <summary></summary>
    public ReadOnlyCollection<ParameterExpression> Parameters => _exp?.Parameters ?? TrueExp.Parameters;

    /// <summary></summary>
    public Type Type => _exp?.Type ?? TrueExp.Type;

    /// <summary></summary>
    public string Name => _exp?.Name ?? "";

    /// <summary></summary>
    public Type ReturnType => _exp?.ReturnType ?? TrueExp.ReturnType;

    /// <summary></summary>
    public bool TailCall => _exp?.TailCall == true;

    /// <summary></summary>
    public bool CanReduce => _exp?.CanReduce == true;
    #endregion

    internal Filter(Expression<Func<T, bool>>? expession = null)
        => _exp = expession;

    #region Features
    /// <summary>Set the Expression predicate</summary>
    /// <param name="expession">The first expession</param>
    public Expression<Func<T, bool>>? Start(Expression<Func<T, bool>>? exp)
        => _exp = IsEmpty ? exp : throw new Exception("Predicate cannot be started again.");

    /// <summary>Or</summary>
    [Pure]
    public Expression<Func<T, bool>>? Or(Expression<Func<T, bool>>? exp)
        => IsEmpty ? Start(exp) : _exp = _exp.Or(exp);

    /// <summary>And</summary>
    [Pure]
    public Expression<Func<T, bool>>? And(Expression<Func<T, bool>>? exp)
        => IsEmpty ? Start(exp) : _exp = _exp.And(exp);

    /// <summary>Not</summary>
    [Pure]
    public Expression<Func<T, bool>>? Not()
        => IsEmpty ? Start(FalseExp) : _exp.Not();

    /// <summary>
    /// Compile to executed delegate
    /// </summary>
    [Pure]
    public Func<T, bool> Compile()
        => (_exp ?? TrueExp).Compile();

    /// <summary> Show predicate string </summary>
    public override string ToString()
        => (_exp ?? TrueExp).ToString() ?? "";
    #endregion
}