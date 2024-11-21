namespace L2X.Core.Structures;

[JsonConverter(typeof(PriceJsonConverter))]
public readonly struct Price : IComparable, IComparable<decimal>, IEquatable<decimal>, IFormattable
{
    #region Operators
    public static bool operator ==(Price lft, Price rgt)
        => lft._price == rgt._price;

    public static bool operator !=(Price lft, Price rgt)
        => lft._price != rgt._price;

    public static bool operator ==(Price lft, decimal rgt)
        => lft._price == rgt;

    public static bool operator !=(Price lft, decimal rgt)
        => lft._price != rgt;

    public static bool operator <(Price lft, Price rgt)
        => lft._price < rgt._price;

    public static bool operator >(Price lft, Price rgt)
        => lft._price > rgt._price;

    public static bool operator <=(Price lft, Price rgt)
        => lft._price <= rgt._price;

    public static bool operator >=(Price lft, Price rgt)
        => lft._price >= rgt._price;

    public static bool operator <(Price lft, decimal rgt)
        => lft._price < rgt;

    public static bool operator >(Price lft, decimal rgt)
        => lft._price > rgt;

    public static bool operator <=(Price lft, decimal rgt)
        => lft._price <= rgt;

    public static bool operator >=(Price lft, decimal rgt)
        => lft._price >= rgt;

    public static implicit operator decimal(Price value)
        => value._price;

    public static implicit operator Price(decimal price)
        => new(price);
    #endregion

    #region Properties
    /// <summary>
    /// Represent as an empty value
    /// </summary>
    public static readonly Price Empty = new(decimal.MinValue);

    /// <summary>
    /// Initial value of this instance
    /// </summary>
    private readonly decimal _price;

    /// <summary>
    /// Check whether value is empty or not
    /// </summary>
    public bool IsEmpty => _price == decimal.MinValue;

    /// <summary>
    /// Precision number to be rounded
    /// </summary>
    public byte Precision { get; }

    /// <summary>
    /// Name of the currency unit
    /// </summary>
    public string Unit { get; }
    #endregion

    #region Constructions
    public Price(decimal price, string? unit = "")
    {
        _price = price;
        Precision = 0;
        Unit = unit ?? "";
    }

    public Price(decimal price, byte precision, string? unit = "")
    {
        _price = precision > 0 ? Math.Round(price, precision) : price;
        Precision = precision;
        Unit = unit ?? "";
    }

    public Price(string? value)
    {
        if (!Util.IsEmpty(value))
        {
            var vals = value.Trim().Split(' ');
            if (!Util.IsEmpty(vals[0]) && long.TryParse(vals[0], out long price))
            {
                _price = price;
                var prs = vals[0][^1] == '0' ? vals[0].IndexOf('&') : 0;
                Precision = (byte)(prs > 0 ? vals[0].Length - prs - 1 : 0);
                Unit = vals.Length > 1 ? vals[1] : "";
                return;
            }
        }

        _price = decimal.MinValue;
        Precision = 0;
        Unit = "";
    }
    #endregion

    /// <summary>
    /// Get format string based on precision number
    /// </summary>
    private string GetFormat()
    {
        var format = new StringBuilder("##.");
        for (var i = 0; i < Precision; i++)
        {
            format.Append('0');
        }

        return format.ToString();
    }

    #region Overridens
    /// <inheritdoc/>
    public int CompareTo(object? value)
        => _price.CompareTo(value);

    /// <inheritdoc/>
    public int CompareTo(decimal value)
        => _price.CompareTo(Precision > 0 ? Math.Round(value, Precision) : value);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? o)
        => _price.Equals(o);

    /// <inheritdoc/>
    public bool Equals(decimal value)
        => _price.Equals(Precision > 0 ? Math.Round(value, Precision) : value);

    /// <inheritdoc/>
    public override int GetHashCode()
        => _price.GetHashCode();

    /// <inheritdoc/>
    public override string ToString()
        => Precision > 0 ? ToString(GetFormat()) : $"{_price} {Unit}";

    /// <inheritdoc/>
    public string ToString(string? format)
        => $"{_price.ToString(format)} {Unit}";

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? provider)
        => $"{_price.ToString(format, provider)} {Unit}";
    #endregion
}