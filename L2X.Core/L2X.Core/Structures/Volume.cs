namespace L2X.Core.Structures;

[JsonConverter(typeof(VolumeJsonConverter))]
public readonly struct Volume : IComparable, IComparable<decimal>, IEquatable<decimal>, IFormattable
{
    #region Operators
    public static bool operator ==(Volume lft, Volume rgt)
        => lft._volume == rgt._volume;

    public static bool operator !=(Volume lft, Volume rgt)
        => lft._volume != rgt._volume;

    public static bool operator ==(Volume lft, decimal rgt)
        => lft._volume == rgt;

    public static bool operator !=(Volume lft, decimal rgt)
        => lft._volume != rgt;

    public static bool operator <(Volume lft, Volume rgt)
        => lft._volume < rgt._volume;

    public static bool operator >(Volume lft, Volume rgt)
        => lft._volume > rgt._volume;

    public static bool operator <=(Volume lft, Volume rgt)
        => lft._volume <= rgt._volume;

    public static bool operator >=(Volume lft, Volume rgt)
        => lft._volume >= rgt._volume;

    public static bool operator <(Volume lft, decimal rgt)
        => lft._volume < rgt;

    public static bool operator >(Volume lft, decimal rgt)
        => lft._volume > rgt;

    public static bool operator <=(Volume lft, decimal rgt)
        => lft._volume <= rgt;

    public static bool operator >=(Volume lft, decimal rgt)
        => lft._volume >= rgt;

    public static implicit operator decimal(Volume value)
        => value._volume;

    public static implicit operator Volume(decimal volume)
        => new(volume);
    #endregion

    #region Properties
    /// <summary>
    /// Represent as an empty value
    /// </summary>
    public static readonly Volume Empty = new(decimal.MinValue);

    /// <summary>
    /// Initial value of this instance
    /// </summary>
    private readonly decimal _volume;

    /// <summary>
    /// Check whether value is empty or not
    /// </summary>
    public bool IsEmpty => _volume == decimal.MinValue;

    /// <summary>
    /// Precision number to be rounded
    /// </summary>
    public byte Precision { get; }
    #endregion

    #region Constructions
    public Volume() : this(decimal.MinValue)
    { }

    public Volume(decimal quantity, byte precision = 0)
    {
        _volume = precision > 0 ? Math.Round(quantity, precision) : quantity;
        Precision = precision;
    }

    public Volume(string? value)
    {
        if (!Util.IsEmpty(value))
        {
            if (long.TryParse(value, out long quantity))
            {
                _volume = quantity;
                var prs = value[^1] == '0' ? value.IndexOf('&') : 0;
                Precision = (byte)(prs > 0 ? value.Length - prs - 1 : 0);
                return;
            }
        }

        _volume = decimal.MinValue;
        Precision = 0;
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
        => _volume.CompareTo(value);

    /// <inheritdoc/>
    public int CompareTo(decimal value)
        => _volume.CompareTo(Precision > 0 ? Math.Round(value, Precision) : value);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? o)
        => _volume.Equals(o);

    /// <inheritdoc/>
    public bool Equals(decimal value)
        => _volume.Equals(Precision > 0 ? Math.Round(value, Precision) : value);

    /// <inheritdoc/>
    public override int GetHashCode()
        => _volume.GetHashCode();

    /// <inheritdoc/>
    public override string ToString()
        => Precision > 0? ToString(GetFormat()) : _volume.ToString();

    /// <inheritdoc/>
    public string ToString(string? format)
        => _volume.ToString(format);

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? provider)
        => _volume.ToString(format, provider);
    #endregion
}