namespace L2X.Core.Structures;

[JsonConverter(typeof(EpochJsonConverter))]
public readonly struct Epoch : IComparable, IComparable<long>, IEquatable<long>, IComparable<DateTime>, IEquatable<DateTime>, IFormattable
{
    #region Operators
    public static bool operator ==(Epoch lft, Epoch rgt)
        => lft._date == rgt._date;

    public static bool operator !=(Epoch lft, Epoch rgt)
        => lft._date != rgt._date;

    public static bool operator ==(Epoch lft, long rgt)
        => lft._value == rgt;

    public static bool operator !=(Epoch lft, long rgt)
        => lft._value != rgt;

    public static bool operator ==(Epoch lft, DateTime rgt)
        => lft._date == new Epoch(rgt)._date;

    public static bool operator !=(Epoch lft, DateTime rgt)
        => lft._date != new Epoch(rgt)._date;

    public static bool operator <(Epoch lft, Epoch rgt)
        => lft._value < rgt._value;

    public static bool operator >(Epoch lft, Epoch rgt)
        => rgt._value > lft._value;

    public static bool operator <=(Epoch lft, Epoch rgt)
        => lft._value <= rgt._value;

    public static bool operator >=(Epoch lft, Epoch rgt)
        => lft._value >= rgt._value;

    public static Epoch operator +(Epoch epoch, TimeSpan ts)
        => new(epoch._date.Add(ts));

    public static Epoch operator -(Epoch epoch, TimeSpan ts)
        => new(epoch._date.Subtract(ts));

    public static TimeSpan operator -(Epoch lft, Epoch rgt)
        => lft._date.Subtract(rgt._date);

    public static TimeSpan operator -(Epoch lft, DateTime rgt)
        => lft._date.Subtract(rgt);

    public static implicit operator long(Epoch stamp)
        => stamp._value;

    public static implicit operator DateTime(Epoch stamp)
        => stamp._date;

    public static implicit operator Epoch(long value)
    => new(value);

    public static explicit operator Epoch(DateTime date)
        => new(date);
    #endregion

    public static long GetTimeStamp()
        => new Epoch().Timestamp;

    public static long GetTimeStamp(DateTime date)
        => new Epoch(date).Timestamp;

    public static DateTime GetDateTime(long timestamp)
        => new Epoch(timestamp).Datetime;

    #region Properties
    private static readonly DateTime _zero = new(1970, 1, 1);

    public readonly static Epoch Zero = new(_zero);

    public static Epoch Now => new();

    private readonly DateTime _date;

    private readonly long _value;

    public DateTime Datetime => _date;

    public long Timestamp => _value;
    #endregion

    #region Constructions
    public Epoch() : this(DateTime.UtcNow)
    { }

    public Epoch(DateTime date)
    {
        _date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
        _value = (long)date.ToUniversalTime().Subtract(_zero).TotalMilliseconds;
    }

    public Epoch(long value)
    {
        _date = _zero.AddSeconds(value).ToLocalTime();
        _value = value;
    }

    public Epoch(string? value)
    {
        if (long.TryParse(value, out long stamp))
        {
            _date = _zero.AddSeconds(stamp).ToLocalTime();
            _value = stamp;
        }
        else
        {
            _date = DateTime.MinValue;
            _value = 0;
        }
    }
    #endregion

    #region DateTime Reflection
    public int Year => _date.Year;

    public int Month => _date.Month;

    public int Day => _date.Day;

    public int Hour => _date.Hour;

    public int Minute => _date.Minute;

    public int Second => _date.Second;

    public int Millisecond => _date.Millisecond;

    public long Ticks => _date.Ticks;

    public Epoch Add(TimeSpan value)
        => new(_date.Add(value));

    public Epoch AddYears(int years)
        => new(_date.AddYears(years));

    public Epoch AddMonths(int months)
        => new(_date.AddMonths(months));

    public Epoch AddDays(int days)
        => new(_date.AddDays(days));

    public Epoch AddHours(int hours)
        => new(_date.AddHours(hours));

    public Epoch AddMinutes(int minutes)
        => new(_date.AddMinutes(minutes));

    public Epoch AddSeconds(int seconds)
        => new(_date.AddSeconds(seconds));

    public Epoch AddMilliseconds(int millis)
        => new(_date.AddMilliseconds(millis));

    public Epoch Subtract(TimeSpan value)
        => new(_date.Subtract(value));

    public TimeSpan Subtract(DateTime date)
        => _date.Subtract(date);

    public TimeSpan Subtract(Epoch epoch)
        => _date.Subtract(epoch._date);
    #endregion

    #region Overridens
    /// <inheritdoc/>
    public int CompareTo(object? value)
        => _date.CompareTo(value);

    /// <inheritdoc/>
    public int CompareTo(long value)
        => _value.CompareTo(value);

    /// <inheritdoc/>
    public int CompareTo(DateTime date)
        => _date.CompareTo(date);

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? o)
        => _date.Equals(o);

    /// <inheritdoc/>
    public bool Equals(long value)
        => _value.Equals(value);

    /// <inheritdoc/>
    public bool Equals(DateTime date)
        => _date.Equals(date);

    /// <inheritdoc/>
    public override int GetHashCode()
        => _date.GetHashCode();

    /// <inheritdoc/>
    public override string ToString()
        => _value.ToString();

    /// <inheritdoc/>
    public string ToString([StringSyntax("DateTimeFormat")] string? format)
        => _date.ToString(format);

    /// <inheritdoc/>
    public string ToString([StringSyntax("DateTimeFormat")] string? format, IFormatProvider? provider)
        => _date.ToString(format, provider);
    #endregion
}
