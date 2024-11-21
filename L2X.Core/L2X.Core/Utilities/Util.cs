using System.Collections;

namespace L2X.Core.Utilities;

public sealed class Util
{
    /// <summary>
    /// Clone a new instance of object with same
    /// properties as value
    /// </summary>
    /// <typeparam name="T">type of value to clone</typeparam>
    /// <param name="value">Value to clone</param>
    /// <returns>New instance as o</returns>
    public static T? Clone<T>(T value)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value));
        }
        catch { }

        return default;
    }

    /// <summary>
    /// All empty value of specific primitive types
    /// </summary>
    private static readonly object[] _empties =
    [
        byte.MinValue, short.MinValue, int.MinValue, long.MinValue,
        sbyte.MinValue, ushort.MinValue, uint.MinValue, ulong.MinValue,
        float.NaN, double.NaN, decimal.MinValue, Half.MinValue,
        false, char.MinValue, string.Empty, Guid.Empty,
        DateOnly.MinValue, DateTime.MinValue, TimeOnly.MinValue, TimeSpan.MinValue,
        EventArgs.Empty,
    ];

    /// <summary>
    /// Convert an object of type TSrc to TDes or else
    /// create new instance of TDes and map with TSrc value
    /// </summary>
    /// <typeparam name="TDes">Type of result object</typeparam>
    /// <typeparam name="TSrc">Type of object to convert</typeparam>
    /// <param name="value">Value to convert</param>
    /// <returns>Converted result</returns>
    public static TDes? Convert<TSrc, TDes>(TSrc? value)
    {
        if (value == null) return default;

        try
        {
            if (value is IConvertible cvt) return (TDes)cvt.ToType(typeof(TDes), null);

            return Map<TSrc, TDes>(value);
        }
        catch { }

        return default;
    }

    /// <summary>
    /// Convert an object of type TSrc to TDes or else
    /// create new instance of TDes and map with TSrc value
    /// </summary>
    /// <typeparam name="TDes">Type of result object</typeparam>
    /// <typeparam name="TSrc">Type of object to convert</typeparam>
    /// <param name="value">Value to convert</param>
    /// <returns>Converted result</returns>
    public static TDes? Map<TSrc, TDes>(TSrc? value)
    {
        if (value == null) return default;

        try
        {
            var result = Activator.CreateInstance<TDes>();
            var p1s = typeof(TSrc).GetProperties();
            var p2s = typeof(TDes).GetProperties();

            foreach (var p1 in p1s)
                p2s.FirstOrDefault(p => p.Name == p1.Name && p.PropertyType.IsAssignableFrom(p1.PropertyType))?.SetValue(result, p1.GetValue(value));
        }
        catch { }

        return default;
    }

    /// <summary>
    /// Check whether value is equal with comparer or not
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <param name="comparer">Value to compare</param>
    /// <param name="caseSense">Use case sensitive or not</param>
    /// <param name="trimSpace">Trim any outer space character or not</param>
    /// <returns>true is equal, else false</returns>
    public static bool IsEquals(string? value, string? comparer, bool caseSense = false, bool trimSpace = false)
    {
        if (value == comparer) return true;
        if (trimSpace) return IsEquals(value?.Trim(), comparer?.Trim(), caseSense);

        return !(value == null || comparer == null) && value.Equals(comparer, caseSense ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Check whether string content of an object of type T
    /// is equal with comparer or not
    /// </summary>
    /// <typeparam name="T">Type of value to check</typeparam>
    /// <param name="value">Value to check</param>
    /// <param name="comparer">Value to compare</param>
    /// <param name="caseSense">Use case sensitive or not</param>
    /// <param name="trimSpace">Trim any outer space character or not</param>
    /// <returns>true is equal, else false</returns>
    public static bool IsEquals<T>(T? value, string? comparer, bool caseSense = false, bool trimSpace = false)
        => IsEquals(value?.ToString(), comparer, caseSense, trimSpace);

    /// <summary>
    /// Check whether string content of an object of type T
    /// is equal with comparer or not
    /// </summary>
    /// <typeparam name="T">Type of value to check</typeparam>
    /// <param name="value">Value to check</param>
    /// <param name="comparer">Value to compare</param>
    /// <param name="caseSense">Use case sensitive or not</param>
    /// <param name="trimSpace">Trim any outer space character or not</param>
    /// <returns>true is equal, else false</returns>
    public static bool IsContains<T>(T? value, string? comparer, bool caseSense = false, bool trimSpace = false)
    {
        if (value == null || IsEmpty(comparer)) return false;

        var text = value.ToString() ?? "";
        if (trimSpace) return IsContains(text.Trim(), comparer.Trim(), caseSense);
        return text.Contains(comparer, caseSense ?StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Check whether value is null or empty string or not
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <param name="trimSpace">Trim any outer space character or not</param>
    /// <returns>true is empty, else false</returns>
    public static bool IsEmpty([NotNullWhen(false)] string? value, bool trimSpace = false)
        => string.IsNullOrEmpty(trimSpace ? value?.Trim() : value);

    /// <summary>
    /// Check whether value is null or empty value
    /// (specified in _empties array above) or not
    /// </summary>
    /// <typeparam name="T">Type of value to check</typeparam>
    /// <param name="value">Value to check</param>
    /// <returns>true is empty, else false</returns>
    public static bool IsEmpty<T>([NotNullWhen(false)] T? value)
    {
        if (value == null) return true;
        if (value is IEnumerable ens) return !ens.GetEnumerator().MoveNext();
        
        var v = Empty<T>();
        return v != null && v.Equals(value);
    }

    /// <summary>
    /// Get empty value of type T
    /// </summary>
    /// <typeparam name="T">Type of value to get</typeparam>
    /// <returns>null for all non primitive type,
    /// empty value of type T existed in _empties array above
    /// or default value if not</returns>
    public static T? Empty<T>()
    {
        var t = typeof(T);
        return (T?)(_empties.FirstOrDefault(v => v.GetType() == t) ?? default);
    }
}