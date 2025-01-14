namespace L2X.Core.Models;

public interface IWrapper<T>
{
	T? Origin { get; }

	void From(T? value);
}

public interface IWrapper : IWrapper<object>
{ }