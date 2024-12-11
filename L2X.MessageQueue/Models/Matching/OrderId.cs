using System.Diagnostics.CodeAnalysis;

namespace L2X.MessageQueue.Models.Matching;

/// <summary>
/// The structure as strong type replacement for int or long identity number of an Order
/// </summary>
public readonly struct OrderId
    : IComparable, IFormattable,
      IComparable<OrderId>, IEquatable<OrderId>,
      IComparable<int>, IEquatable<int>,
      IComparable<uint>, IEquatable<uint>,
      IComparable<long>, IEquatable<long>,
      IComparable<ulong>, IEquatable<ulong>
{
    #region Operators
    public static bool operator ==(OrderId lft, OrderId rgt)
        => lft._value == rgt._value;

    public static bool operator !=(OrderId lft, OrderId rgt)
        => lft._value != rgt._value;

    public static bool operator ==(OrderId lft, int rgt)
        => rgt >= 0 && lft._value == (ulong)rgt;

    public static bool operator !=(OrderId lft, int rgt)
        => rgt < 0 || lft._value != (ulong)rgt;

    public static bool operator ==(OrderId lft, uint rgt)
        => rgt >= 0 && lft._value == rgt;

    public static bool operator !=(OrderId lft, uint rgt)
        => rgt < 0 || lft._value != rgt;

    public static bool operator ==(OrderId lft, long rgt)
        => rgt >= 0 && lft._value == (ulong)rgt;

    public static bool operator !=(OrderId lft, long rgt)
        => rgt < 0 || lft._value != (ulong)rgt;

    public static bool operator ==(OrderId lft, ulong rgt)
        => lft._value == rgt;

    public static bool operator !=(OrderId lft, ulong rgt)
        => lft._value != rgt;

    public static bool operator <(OrderId lft, OrderId rgt)
        => lft._value < rgt._value;

    public static bool operator >(OrderId lft, OrderId rgt)
        => lft._value > rgt._value;

    public static bool operator <=(OrderId lft, OrderId rgt)
        => lft._value <= rgt._value;

    public static bool operator >=(OrderId lft, OrderId rgt)
        => lft._value >= rgt._value;

    public static bool operator <(OrderId lft, int rgt)
        => rgt < 0 || lft._value < (ulong)rgt;

    public static bool operator >(OrderId lft, int rgt)
        => rgt >= 0 && lft._value > (ulong)rgt;

    public static bool operator <(OrderId lft, uint rgt)
        => lft._value < rgt;

    public static bool operator >(OrderId lft, uint rgt)
        => lft._value > rgt;

    public static bool operator <=(OrderId lft, int rgt)
        => rgt < 0 || lft._value <= (ulong)rgt;

    public static bool operator >=(OrderId lft, int rgt)
        => rgt >= 0 && lft._value >= (ulong)rgt;

    public static bool operator <=(OrderId lft, uint rgt)
        => lft._value <= rgt;

    public static bool operator >=(OrderId lft, uint rgt)
        => lft._value >= rgt;

    public static bool operator <(OrderId lft, long rgt)
        => lft._value < (ulong)rgt;

    public static bool operator >(OrderId lft, long rgt)
        => lft._value > (ulong)rgt;

    public static bool operator <=(OrderId lft, ulong rgt)
        => lft._value <= rgt;

    public static bool operator >=(OrderId lft, ulong rgt)
        => lft._value >= rgt;

    public static implicit operator int(OrderId value)
        => value.IsEmpty || value._value > int.MaxValue ? int.MaxValue : (int)value._value;

    public static implicit operator uint(OrderId value)
        => value._value > uint.MaxValue ? uint.MaxValue : (uint)value._value;

    public static implicit operator ulong(OrderId value)
        => value._value;

    public static implicit operator OrderId(int value)
        => new(value);

    public static implicit operator OrderId(long value)
        => new(value);

    public static implicit operator OrderId(uint value)
        => new(value);

    public static implicit operator OrderId(ulong value)
        => new(value);
    #endregion

    #region Properties
    /// <summary>
    /// Represent as an empty value
    /// </summary>
    public static readonly OrderId Empty = new(0);

    /// <summary>
    /// Initial value of this instance
    /// </summary>
    private readonly ulong _value;

    /// <summary>
    /// Check whether value is empty or not
    /// </summary>
    public bool IsEmpty => _value == 0;
    #endregion

    #region Constructions
    public OrderId(int value)
        => _value = value < 0 ? 0 : (ulong)value;

    public OrderId(long value)
        => _value = value < 0 ? 0 : (ulong)value;

    public OrderId(uint value)
        => _value = value < 0 ? 0 : value;

    public OrderId(ulong value)
        => _value = value < 0 ? 0 : value;

    public OrderId(string? value)
        => _value = ulong.TryParse(value, out ulong result) ? result : 0;
    #endregion

    #region Overridens
    /// <inheritdoc/>
    public int CompareTo(object? value)
        => _value.CompareTo(value);

    /// <inheritdoc/>
    public int CompareTo(OrderId value)
        => _value.CompareTo(value._value);

    /// <inheritdoc/>
    public int CompareTo(int value)
        => _value.CompareTo(value);

    /// <inheritdoc/>
    public int CompareTo(uint value)
        => _value.CompareTo(value);

    /// <inheritdoc/>
    public int CompareTo(long value)
        => _value.CompareTo(value);

    /// <inheritdoc/>
    public int CompareTo(ulong value)
        => _value.CompareTo(value);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? o)
        => _value.Equals(o);

    /// <inheritdoc/>
    public bool Equals(OrderId value)
        => _value.Equals(value._value);

    /// <inheritdoc/>
    public bool Equals(int value)
        => _value.Equals(value);

    /// <inheritdoc/>
    public bool Equals(uint value)
        => _value.Equals(value);

    /// <inheritdoc/>
    public bool Equals(long value)
        => _value.Equals(value);

    /// <inheritdoc/>
    public bool Equals(ulong value)
        => _value.Equals(value);

    /// <inheritdoc/>
    public override int GetHashCode()
        => _value.GetHashCode();

    /// <inheritdoc/>
    public override string ToString()
        => _value.ToString();

    /// <inheritdoc/>
    public string ToString(string? format)
        => _value.ToString(format);

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? provider)
        => _value.ToString(format, provider);
    #endregion
}