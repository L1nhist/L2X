namespace L2X.Core.Structures;

/// <summary>
/// Strong type struct as a replacement for int or long structure
/// using as quantity amount property of class
/// </summary>
[JsonConverter(typeof(QuantityJsonConverter))]
public readonly struct Sequence : IComparable, IComparable<uint>, IEquatable<uint>, IComparable<ulong>, IEquatable<ulong>, IFormattable
{
    #region Operators
    public static bool operator ==(Sequence lft, Sequence rgt)
        => lft._quantity == rgt._quantity;

    public static bool operator !=(Sequence lft, Sequence rgt)
        => lft._quantity != rgt._quantity;

    public static bool operator ==(Sequence lft, ulong rgt)
        => lft._quantity == rgt;

    public static bool operator !=(Sequence lft, ulong rgt)
        => lft._quantity != rgt;

    public static bool operator ==(Sequence lft, uint rgt)
        => lft._quantity == rgt;

    public static bool operator !=(Sequence lft, uint rgt)
        => lft._quantity != rgt;

    public static bool operator <(Sequence lft, Sequence rgt)
        => lft._quantity < rgt._quantity;

    public static bool operator >(Sequence lft, Sequence rgt)
        => lft._quantity > rgt._quantity;

    public static bool operator <=(Sequence lft, Sequence rgt)
        => lft._quantity <= rgt._quantity;

    public static bool operator >=(Sequence lft, Sequence rgt)
        => lft._quantity >= rgt._quantity;

    public static bool operator <(Sequence lft, uint rgt)
        => lft._quantity < rgt;

    public static bool operator >(Sequence lft, uint rgt)
        => lft._quantity > rgt;

    public static bool operator <=(Sequence lft, uint rgt)
        => lft._quantity <= rgt;

    public static bool operator >=(Sequence lft, uint rgt)
        => lft._quantity >= rgt;

    public static bool operator <(Sequence lft, ulong rgt)
        => lft._quantity < rgt;

    public static bool operator >(Sequence lft, ulong rgt)
        => lft._quantity > rgt;

    public static bool operator <=(Sequence lft, ulong rgt)
        => lft._quantity <= rgt;

    public static bool operator >=(Sequence lft, ulong rgt)
        => lft._quantity >= rgt;

    public static implicit operator uint(Sequence value)
        => value.IsEmpty || value._quantity > uint.MaxValue ? uint.MaxValue : (uint)value._quantity;

    public static implicit operator Sequence(uint quantity)
        => new(quantity);

    public static implicit operator ulong(Sequence value)
        => value._quantity;

    public static implicit operator Sequence(ulong quantity)
        => new(quantity);
    #endregion

    #region Properties
    /// <summary>
    /// Represent as an empty value
    /// </summary>
    public static readonly Sequence Empty = new(0);

    /// <summary>
    /// Initial value of this instance
    /// </summary>
    private readonly ulong _quantity;

    /// <summary>
    /// Check whether value is empty or not
    /// </summary>
    public bool IsEmpty => _quantity == 0;
    #endregion

    #region Constructions
    public Sequence(uint quantity)
        => _quantity = quantity;

    public Sequence(ulong quantity)
        => _quantity = quantity;

    public Sequence(string? value)
        => _quantity = !Util.IsEmpty(value) && ulong.TryParse(value, out ulong quantity) ? quantity : ulong.MinValue;
    #endregion

    #region Overridens
    /// <inheritdoc/>
    public int CompareTo(object? value)
        => _quantity.CompareTo(value);

    /// <inheritdoc/>
    public int CompareTo(uint value)
        => _quantity.CompareTo(value);

    /// <inheritdoc/>
    public int CompareTo(ulong value)
        => _quantity.CompareTo(value);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? o)
        => _quantity.Equals(o);

    /// <inheritdoc/>
    public bool Equals(uint value)
        => _quantity.Equals(value);

    /// <inheritdoc/>
    public bool Equals(ulong value)
        => _quantity.Equals(value);

    /// <inheritdoc/>
    public override int GetHashCode()
        => _quantity.GetHashCode();

    /// <inheritdoc/>
    public override string ToString()
        => _quantity.ToString();

    /// <inheritdoc/>
    public string ToString(string? format)
        => _quantity.ToString(format);

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? provider)
        => _quantity.ToString(format, provider);
    #endregion
}
