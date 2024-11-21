using System.Diagnostics.Contracts;
using System.Reflection;

namespace L2X.Core.Extensions;

/// <summary>
/// Build Expression tree with code
/// </summary>
public static class ExpressionExtensions
{
    private class RebindParameterVisitor(ParameterExpression oldPrm, ParameterExpression newPrm) : ExpressionVisitor
    {
        private readonly ParameterExpression _oldPrm = oldPrm;
        private readonly ParameterExpression _newPrm = newPrm;

        protected override Expression VisitParameter(ParameterExpression node)
            => node == _oldPrm ? _newPrm : base.VisitParameter(node);
    }

    #region Features
    /// <summary> OR </summary>
    [Pure]
    public static Expression<Func<T, bool>>? Or<T>(this Expression<Func<T, bool>>? exp1, Expression<Func<T, bool>>? exp2)
    {
        if (exp1 == null || exp2 == null) return exp1 ?? exp2;

        var exp2Body = new RebindParameterVisitor(exp2.Parameters[0], exp1.Parameters[0]).Visit(exp2.Body);
        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(exp1.Body, exp2Body), exp1.Parameters);
    }

    /// <summary> AND </summary>
    [Pure]
    public static Expression<Func<T, bool>>? And<T>(this Expression<Func<T, bool>>? exp1, Expression<Func<T, bool>>? exp2)
    {
        if (exp1 == null || exp2 == null) return exp1 ?? exp2;

        var exp2Body = new RebindParameterVisitor(exp2.Parameters[0], exp1.Parameters[0]).Visit(exp2.Body);
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(exp1.Body, exp2Body), exp1.Parameters);
    }

    /// <summary> NOT </summary>
    [Pure]
    public static Expression<Func<T, bool>>? Not<T>(this Expression<Func<T, bool>>? exp)
        => exp == null ? null : Expression.Lambda<Func<T, bool>>(Expression.Not(exp.Body), exp.Parameters);

    /// <summary>
    /// Extends the specified source Predicate with another Predicate and the specified PredicateOperator.
    /// </summary>
    /// <typeparam name="TInp"></typeparam>
    /// <typeparam name="TRes"></typeparam>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    [Pure]
    public static Expression<Func<TIn, TOut>>? Compose<TIn, TOut>(this Expression<Func<TIn, TOut>>? exp1, Expression<Func<TIn, TOut>>? exp2)
    {
        if (exp1 == null || exp2 == null) return exp1 ?? exp2;

        var exp2Body = new RebindParameterVisitor(exp2.Parameters[0], exp1.Parameters[0]).Visit(exp2.Body);
        return Expression.Lambda<Func<TIn, TOut>>(exp2Body, exp1.Parameters[0]);
    }

    public static Expression<Action<T>>? Compose<T>(this Expression<Action<bool>>? exp1, Expression<Func<T, bool>>? exp2)
    {
        if (exp1 == null || exp2 == null) return null;

        var exp2Body = new RebindParameterVisitor(exp2.Parameters[0], exp1.Parameters[0]).Visit(exp2.Body);
        return Expression.Lambda<Action<T>>(exp2Body, exp1.Parameters[0]);
    }

    /// <summary>
    /// Extends the specified source Predicate with another Predicate and the specified PredicateOperator.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    /// <param name="exp1">The source Predicate.</param>
    /// <param name="exp2">The second Predicate.</param>
    /// <param name="orElse">True is "Or", else is "And"</param>
    /// <returns>Expression{Func{T, bool}}</returns>
    [Pure]
    public static Expression<Func<T, bool>>? Extend<T>(this Expression<Func<T, bool>>? exp1, Expression<Func<T, bool>>? exp2, bool @orElse = true)
        => exp1 == null ? exp2 : @orElse ? exp1.Or(exp2) : exp1.And(exp2);

    /// <summary>
    /// Extends the specified source Predicate with another Predicate and the specified PredicateOperator.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    /// <param name="exp1">The source Predicate.</param>
    /// <param name="exp2">The second Predicate.</param>
    /// <param name="orElse">True is "Or", else is "And"</param>
    /// <returns>Expression{Func{T, bool}}</returns>
    [Pure]
    public static Expression<Func<T, bool>>? Extend<T>(this Filter<T>? exp1, Expression<Func<T, bool>>? exp2, bool @orElse = true)
        => exp1 == null ? exp2 : @orElse ? exp1.Or(exp2) : exp1.And(exp2);
    #endregion

    public static IEnumerable<PropertyInfo> GetProps(this object obj, params string[] fields)
    {
        try
        {
            var names = fields == null ? [] : fields.Select(f => f?.Trim()).Where(f => !string.IsNullOrEmpty(f)).ToArray();
            var props = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (Util.IsEmpty(names)) return props;

            return props.Where(p => names.Any(n => n.Equals(p.Name, StringComparison.OrdinalIgnoreCase) || n.Equals($"-{p.Name}", StringComparison.OrdinalIgnoreCase))).ToArray();
        }
        catch { }

        return [];
    }

    public static IEnumerable<PropertyInfo> GetProps<T>(params string[] fields)
    {
        try
        {
            var names = fields == null ? [] : fields.Select(f => f?.Trim()).Where(f => !string.IsNullOrEmpty(f)).ToArray();
            var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (Util.IsEmpty(names)) return props;

            return props.Where(p => names.Any(n => n.Equals(p.Name, StringComparison.OrdinalIgnoreCase) || n.Equals($"-{p.Name}", StringComparison.OrdinalIgnoreCase))).ToArray();
        }
        catch { }

        return [];
    }

    public static IEnumerable<PropertyInfo> GetProps<TOwn, TFld>(params string[] fields)
    {
        try
        {
            var props = GetProps<TOwn>(fields);
            return props.Where(p => p.PropertyType is TFld).ToArray();
        }
        catch { }

        return [];
    }

    public static string GetPropName<TOwn, TFld>(this Expression<Func<TOwn, TFld>> expession)
    {
        MemberExpression? member;
        if (expession.Body is MemberExpression)
        {
            member = (MemberExpression)expession.Body;
        }
        else if (expession.Body is UnaryExpression)
        {
            member = (MemberExpression)((UnaryExpression)expession.Body).Operand;
        }
        else
        {
            throw new NotSupportedException($"{expession.GetType()} not a proper member selector");
        }

        return member.Member.Name;
    }

    public static Expression<Func<T, object>>? BuildGetter<T>(this PropertyInfo? property)
    {
        if (property == null) return null;
        var param = Expression.Parameter(typeof(T));
        var prop = Expression.Property(param, property);
        var boxed = Expression.Convert(prop, typeof(object));
        return Expression.Lambda<Func<T, object>>(boxed, param);
    }

    public static Expression<Func<T, object>>? BuildGetter<T>(string property)
    {
        var props = GetProps<T>(property);
        return props?.FirstOrDefault().BuildGetter<T>();
    }

    public static void SetValue<TOnw, TFld>(this Expression<Func<TOnw, TFld>> predicate, TOnw target, TFld value)
    {
        if (predicate?.Body is MemberExpression member)
        {
            var property = member.Member as PropertyInfo;
            property?.SetValue(target, value, null);
        }
    }

    public static string GetMemberName(this LambdaExpression? expression)
    {
        var body = expression?.Body;
        if (body == null) return "";

        while (true)
        {
            switch (body.NodeType)
            {
                case ExpressionType.Parameter:
                    return ((ParameterExpression)body).Name ?? "";
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)body).Member.Name;
                case ExpressionType.Call:
                    return ((MethodCallExpression)body).Method.Name;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    body = ((UnaryExpression)body).Operand;
                    break;
                case ExpressionType.Invoke:
                    body = ((InvocationExpression)body).Expression;
                    break;
                case ExpressionType.ArrayLength:
                    return "Length";
                default:
                    return "";
            }
        }
    }
}