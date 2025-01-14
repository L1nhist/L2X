using L2X.Core.Models;

namespace L2X.Core.Structures;

public class Wrapper<T>(T? origin = default) : IWrapper<T>
{
    public T? Origin { get; private set; } = origin;

    public void From(T? value)
        => Origin = value;
}

public class Wrapper(object? origin = null) : Wrapper<object>, IWrapper
{ }