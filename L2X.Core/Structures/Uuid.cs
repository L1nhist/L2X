namespace L2X.Core.Structures;

/// <summary>
/// Strong type struct as a replacement for Guid structure
/// </summary>
[JsonConverter(typeof(UuidJsonConverter))]
public readonly struct Uuid : IComparable, IComparable<Guid>, IEquatable<Guid>, IFormattable
{
    #region Operators
    public static bool operator ==(Uuid lft, Uuid rgt)
        => lft._guid == rgt._guid;

    public static bool operator !=(Uuid lft, Uuid rgt)
        => lft._guid != rgt._guid;

    public static bool operator ==(Uuid lft, Guid rgt)
        => lft._guid == rgt;

    public static bool operator !=(Uuid lft, Guid rgt)
        => lft._guid != rgt;

    public static bool operator ==(Uuid lft, string rgt)
        => Util.IsEquals(lft, rgt);

    public static bool operator !=(Uuid lft, string rgt)
        => !Util.IsEquals(lft, rgt);

    public static bool operator <(Uuid lft, Uuid rgt)
        => lft._guid < rgt._guid;

    public static bool operator >(Uuid lft, Uuid rgt)
        => lft._guid > rgt._guid;

    public static bool operator <=(Uuid lft, Uuid rgt)
        => lft._guid <= rgt._guid;

    public static bool operator >=(Uuid lft, Uuid rgt)
        => lft._guid >= rgt._guid;

    public static implicit operator Guid(Uuid value)
        => value._guid;

    public static implicit operator Uuid(Guid guid)
        => new(guid);
    #endregion

    #region Static
    /// <summary>
    /// Represent as an empty value
    /// </summary>
    public static readonly Uuid Empty = Guid.Empty;

    /// <summary>
    /// Initiate a new instant of Guid value
    /// </summary>
    public static Uuid New => new();

    /// <summary>
    /// Initiate a new instant of Guid value that can be converted to
    /// a BASE64 string without any special characters (+,/)
    /// </summary>
    public static Uuid NewCode => new(Crypto.Random.NewGuid());

    /// <summary>
    /// Try parse a string to Guid with or without format or culture provider
    /// if explicit or else try parse a BASE64 string to Guid 
    /// </summary>
    /// <param name="value">Guid text content</param>
    /// <param name="format"> One of the following specifiers that indicates
    /// the exact format to use when interpreting input: "N", "D", "B", "P",
    /// or "X"</param>
    /// <param name="provider">An object that provides culture-specific
    /// formatting information</param>
    /// <returns>Guid value that was parsed or Guid empty if failed</returns>
    private static Guid ToGuid(string? value, string? format = null, IFormatProvider? provider = null)
    {
        if (Util.IsEmpty(value)) return Guid.Empty;

        try
        {
            if (!Util.IsEmpty(format))
                return Guid.ParseExact(value, format);
            else if (provider != null)
                return Guid.Parse(value, provider);
        }
        catch { }

        try
        {
            if (!Guid.TryParse(value, out Guid guid))
                return new(Convert.FromBase64String((value.EndsWith("==") ? value : value + "==").Replace('@', '+').Replace('$', '/')));
        }
        catch { }

        return Guid.Empty;
    }

    /// <summary>
    /// Try parse a characters span to Guid with or without format or culture provider
    /// if explicit or else try parse a BASE64 characters span to Guid 
    /// </summary>
    /// <param name="span">Characters span content</param>
    /// <param name="format"> One of the following specifiers that indicates
    /// the exact format to use when interpreting input: "N", "D", "B", "P",
    /// or "X"</param>
    /// <param name="provider">An object that provides culture-specific
    /// formatting information</param>
    /// <returns>Guid value that was parsed or Guid empty if failed</returns>
    private static Guid ToGuid(ReadOnlySpan<char> span, ReadOnlySpan<char> format, IFormatProvider? provider = null)
    {
        try
        {
            if (format != null && format != [])
                return Guid.ParseExact(span, format);
            else if (provider != null)
                return Guid.Parse(span, provider);
        }
        catch { }

        try
        {
            if (!Guid.TryParse(span, out Guid guid))
                return ToGuid(new string(span));
        }
        catch { }

        return Guid.Empty;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Initial value of this instance
    /// </summary>
    private readonly Guid _guid;

    /// <summary>
    /// Check whether value is empty or not
    /// </summary>
    public readonly bool IsEmpty => _guid == Guid.Empty;
    #endregion

    #region Constructions
    public Uuid() => _guid = Guid.NewGuid();

    public Uuid(Guid guid) => _guid = guid;

    public Uuid(string? value)
        => _guid = ToGuid(value);

    public Uuid(string value, IFormatProvider? provider = null)
        => _guid = ToGuid(value, null, provider);

    public Uuid(string value, string format = "")
        => _guid = ToGuid(value, format);

    public Uuid(ReadOnlySpan<char> span, IFormatProvider? provider = null)
        => _guid = ToGuid(span, [], provider);

    public Uuid(ReadOnlySpan<char> span, [StringSyntax("GuidFormat")] ReadOnlySpan<char> format)
        => _guid = ToGuid(span, format);
    #endregion

    #region Overridens
    /// <inheritdoc/>
    public int CompareTo(object? value)
        => _guid.CompareTo(value);

    /// <inheritdoc/>
    public int CompareTo(Guid guid)
        => _guid.CompareTo(guid);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? o)
        => _guid.Equals(o);

    /// <inheritdoc/>
    public bool Equals(Guid guid)
        => _guid.Equals(guid);

    /// <inheritdoc/>
    public override int GetHashCode()
        => _guid.GetHashCode();

    /// <inheritdoc/>
    public byte[] ToByteArray(bool bigEndian)
        => _guid.ToByteArray(bigEndian);

    /// <inheritdoc/>
    public byte[] ToByteArray()
        => _guid.ToByteArray();

    /// <inheritdoc/>
    public override string ToString()
        => IsEmpty ? "" : Convert.ToBase64String(_guid.ToByteArray())[..^2].Replace('+', '@').Replace('/', '$');

    /// <inheritdoc/>
    public string ToString([StringSyntax("GuidFormat")] string? format)
        => IsEmpty ? "" : _guid.ToString(format);

    /// <inheritdoc/>
    public string ToString([StringSyntax("GuidFormat")] string? format, IFormatProvider? provider)
        => IsEmpty ? "" : _guid.ToString(format, provider);
    #endregion
}