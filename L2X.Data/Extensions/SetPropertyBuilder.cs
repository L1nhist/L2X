using Microsoft.EntityFrameworkCore.Query;

namespace L2X.Data.Extensions;

public static class SetPropertyBuilder
{
	public static Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> New<T>(string? field, object? value)
		=> new SetPropertyBuilder<T>().SetProperty(field, value).Caller;

	public static Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> New<T>(Action<SetPropertyBuilder<T>>? expression)
	{
		var builder = new SetPropertyBuilder<T>();
		expression?.Invoke(builder);
		return builder.Caller;
	}

	public static Expression<Func<SetPropertyCalls<TObj>, SetPropertyCalls<TObj>>> New<TObj, TFld>(string? field, TFld? value)
		=> new SetPropertyBuilder<TObj>().SetProperty(field, value).Caller;

	public static Expression<Func<SetPropertyCalls<TObj>, SetPropertyCalls<TObj>>> New<TObj, TFld>(Expression<Func<TObj, TFld?>>? selector, TFld? value)
		=> new SetPropertyBuilder<TObj>().SetProperty(selector, value).Caller;

	public static Expression<Func<SetPropertyCalls<TObj>, SetPropertyCalls<TObj>>> New<TObj, TFld>(Expression<Func<TObj, TFld?>>? selector, Expression<Func<TObj, TFld?>> value)
		=> new SetPropertyBuilder<TObj>().SetProperty(selector, value).Caller;
}

public class SetPropertyBuilder<T>
{
	public Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> Caller { get; private set; } = b => b;

	public SetPropertyBuilder<T> SetProperty(string? field, object? value)
	{
        if (Util.IsEmpty(field)) return this;

		var prop = typeof(T).GetProperty(field);
		if (prop == null) return this;

		var param = Expression.Parameter(typeof(T));
		var cast = Expression.Convert(Expression.Property(param, prop), typeof(object));
		var selector = Expression.Lambda<Func<T, object?>>(cast, param);
		return SetProperty(selector, _ => value);
	}

	public SetPropertyBuilder<T> SetProperty<TFld>(string? field, TFld? value)
	{
		if (Util.IsEmpty(field)) return this;

		var info = typeof(T).GetProperty(field);
		if (info == null) return this;

		var param = Expression.Parameter(typeof(T));
		var prop = Expression.Property(param, info);
		var method = typeof(Func<,>).MakeGenericType(typeof(T), info.PropertyType);
		var selector = Expression.Lambda<Func<T, TFld?>>(prop, param);
		return SetProperty(selector, _ => value);
	}

    public SetPropertyBuilder<T> SetProperty<TFld>(Expression<Func<T, TFld?>>? selector, TFld? value)
        => selector == null ? this : SetProperty(selector, _ => value);

	public SetPropertyBuilder<T> SetProperty<TFld>(Expression<Func<T, TFld?>>? selector, Expression<Func<T, TFld?>> value)
    {
		Caller = Caller.Update(
            body: Expression.Call(
                instance: Caller.Body,
                methodName: nameof(SetPropertyCalls<T>.SetProperty),
                typeArguments: [typeof(TFld?)],
                arguments: [selector, value]
            ),
            parameters: Caller.Parameters
        );
        return this;
	}
}