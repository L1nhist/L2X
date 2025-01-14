using L2X.Core.Validations;

namespace L2X.Core.Extensions;

public static class EvaluatorExtensions
{
    #region String Only
    public static IEvaluator<T, string> ContainsAny<T>(this IEvaluator<T, string> eval, bool caseSense = false, bool trimSpace = false, params string[] values)
        => eval.Eval(e => values.Any(v => Util.IsContains(e, v, caseSense, trimSpace)));

    public static IEvaluator<T, string> ContainsNone<T>(this IEvaluator<T, string> eval, params string[] values)
        => eval.Eval(e => values.Any(v => !Util.IsContains(e, v, false, false)));

    public static IEvaluator<T, string> PresentsAny<T>(this IEvaluator<T, string> eval, bool caseSense = false, bool trimSpace = false, params string[] values)
        => eval.Eval(e => values.Any(v => Util.IsContains(v, e, caseSense, trimSpace)));

    public static IEvaluator<T, string> PresentsNone<T>(this IEvaluator<T, string> eval, params string[] values)
        => eval.Eval(e => values.Any(v => !Util.IsContains(v, e, false, false)));
    #endregion

    #region Char Only
    public static IEvaluator<T, char> PresentsAny<T>(this IEvaluator<T, char> eval, bool caseSense = false, bool trimSpace = false, params string[] values)
        => eval.Eval(e => values.Any(v => Util.IsContains(v, e.ToString(), caseSense, trimSpace)));

    public static IEvaluator<T, char> PresentsNone<T>(this IEvaluator<T, char> eval, params string[] values)
        => eval.Eval(e => values.Any(v => !Util.IsContains(v, e.ToString(), false, false)));
    #endregion

    #region Byte Only
    public static IEvaluator<T, byte> IsBetween<T>(this IEvaluator<T, byte> eval, byte min, byte max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, byte> NotBetween<T>(this IEvaluator<T, byte> eval, byte min, byte max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, byte> OverAny<T>(this IEvaluator<T, byte> eval, params byte[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, byte> OverOrEqualAny<T>(this IEvaluator<T, byte> eval, params byte[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, byte> UnderAny<T>(this IEvaluator<T, byte> eval, params byte[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, byte> UnderOrEqualAny<T>(this IEvaluator<T, byte> eval, params byte[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion

    #region Sbyte Only
    public static IEvaluator<T, sbyte> IsBetween<T>(this IEvaluator<T, sbyte> eval, sbyte min, sbyte max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, sbyte> NotBetween<T>(this IEvaluator<T, sbyte> eval, sbyte min, sbyte max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, sbyte> OverAny<T>(this IEvaluator<T, sbyte> eval, params sbyte[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, sbyte> OverOrEqualAny<T>(this IEvaluator<T, sbyte> eval, params sbyte[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, sbyte> UnderAny<T>(this IEvaluator<T, sbyte> eval, params sbyte[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, sbyte> UnderOrEqualAny<T>(this IEvaluator<T, sbyte> eval, params sbyte[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion

    #region Short Only
    public static IEvaluator<T, short> IsBetween<T>(this IEvaluator<T, short> eval, short min, short max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, short> NotBetween<T>(this IEvaluator<T, short> eval, short min, short max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, short> OverAny<T>(this IEvaluator<T, short> eval, params short[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, short> OverOrEqualAny<T>(this IEvaluator<T, short> eval, params short[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, short> UnderAny<T>(this IEvaluator<T, short> eval, params short[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, short> UnderOrEqualAny<T>(this IEvaluator<T, short> eval, params short[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion

    #region Ushort Only
    public static IEvaluator<T, ushort> IsBetween<T>(this IEvaluator<T, ushort> eval, ushort min, ushort max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, ushort> NotBetween<T>(this IEvaluator<T, ushort> eval, ushort min, ushort max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, ushort> OverAny<T>(this IEvaluator<T, ushort> eval, params ushort[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, ushort> OverOrEqualAny<T>(this IEvaluator<T, ushort> eval, params ushort[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, ushort> UnderAny<T>(this IEvaluator<T, ushort> eval, params ushort[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, ushort> UnderOrEqualAny<T>(this IEvaluator<T, ushort> eval, params ushort[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion

    #region Int Only
    public static IEvaluator<T, int> IsBetween<T>(this IEvaluator<T, int> eval, int min, int max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, int> NotBetween<T>(this IEvaluator<T, int> eval, int min, int max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, int> OverAny<T>(this IEvaluator<T, int> eval, params int[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, int> OverOrEqualAny<T>(this IEvaluator<T, int> eval, params int[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, int> UnderAny<T>(this IEvaluator<T, int> eval, params int[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, int> UnderOrEqualAny<T>(this IEvaluator<T, int> eval, params int[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion

    #region Uint Only
    public static IEvaluator<T, uint> IsBetween<T>(this IEvaluator<T, uint> eval, uint min, uint max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, uint> NotBetween<T>(this IEvaluator<T, uint> eval, uint min, uint max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, uint> OverAny<T>(this IEvaluator<T, uint> eval, params uint[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, uint> OverOrEqualAny<T>(this IEvaluator<T, uint> eval, params uint[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, uint> UnderAny<T>(this IEvaluator<T, uint> eval, params uint[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, uint> UnderOrEqualAny<T>(this IEvaluator<T, uint> eval, params uint[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion

    #region Long Only
    public static IEvaluator<T, long> IsBetween<T>(this IEvaluator<T, long> eval, long min, long max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, long> NotBetween<T>(this IEvaluator<T, long> eval, long min, long max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, long> OverAny<T>(this IEvaluator<T, long> eval, params long[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, long> OverOrEqualAny<T>(this IEvaluator<T, long> eval, params long[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, long> UnderAny<T>(this IEvaluator<T, long> eval, params long[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, long> UnderOrEqualAny<T>(this IEvaluator<T, long> eval, params long[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion

    #region Ulong Only
    public static IEvaluator<T, ulong> IsBetween<T>(this IEvaluator<T, ulong> eval, ulong min, ulong max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, ulong> NotBetween<T>(this IEvaluator<T, ulong> eval, ulong min, ulong max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, ulong> OverAny<T>(this IEvaluator<T, ulong> eval, params ulong[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, ulong> OverOrEqualAny<T>(this IEvaluator<T, ulong> eval, params ulong[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, ulong> UnderAny<T>(this IEvaluator<T, ulong> eval, params ulong[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, ulong> UnderOrEqualAny<T>(this IEvaluator<T, ulong> eval, params ulong[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion

    #region Quantity Only
    public static IEvaluator<T, Sequence> IsBetween<T>(this IEvaluator<T, Sequence> eval, Sequence min, Sequence max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, Sequence> NotBetween<T>(this IEvaluator<T, Sequence> eval, Sequence min, Sequence max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, Sequence> OverAny<T>(this IEvaluator<T, Sequence> eval, params Sequence[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, Sequence> OverOrEqualAny<T>(this IEvaluator<T, Sequence> eval, params Sequence[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, Sequence> UnderAny<T>(this IEvaluator<T, Sequence> eval, params Sequence[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, Sequence> UnderOrEqualAny<T>(this IEvaluator<T, Sequence> eval, params Sequence[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion

    #region Float Only
    public static IEvaluator<T, float> IsBetween<T>(this IEvaluator<T, float> eval, float min, float max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, float> NotBetween<T>(this IEvaluator<T, float> eval, float min, float max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, float> OverAny<T>(this IEvaluator<T, float> eval, params float[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, float> OverOrEqualAny<T>(this IEvaluator<T, float> eval, params float[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, float> UnderAny<T>(this IEvaluator<T, float> eval, params float[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, float> UnderOrEqualAny<T>(this IEvaluator<T, float> eval, params float[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion

    #region Double Only
    public static IEvaluator<T, double> IsBetween<T>(this IEvaluator<T, double> eval, double min, double max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, double> NotBetween<T>(this IEvaluator<T, double> eval, double min, double max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, double> OverAny<T>(this IEvaluator<T, double> eval, params double[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, double> OverOrEqualAny<T>(this IEvaluator<T, double> eval, params double[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, double> UnderAny<T>(this IEvaluator<T, double> eval, params double[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, double> UnderOrEqualAny<T>(this IEvaluator<T, double> eval, params double[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion

    #region Decimal Only
    public static IEvaluator<T, decimal> IsBetween<T>(this IEvaluator<T, decimal> eval, decimal min, decimal max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, decimal> NotBetween<T>(this IEvaluator<T, decimal> eval, decimal min, decimal max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, decimal> OverAny<T>(this IEvaluator<T, decimal> eval, params decimal[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, decimal> OverOrEqualAny<T>(this IEvaluator<T, decimal> eval, params decimal[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, decimal> UnderAny<T>(this IEvaluator<T, decimal> eval, params decimal[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, decimal> UnderOrEqualAny<T>(this IEvaluator<T, decimal> eval, params decimal[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion

    #region Price Only
    public static IEvaluator<T, Price> IsBetween<T>(this IEvaluator<T, Price> eval, Price min, Price max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, Price> NotBetween<T>(this IEvaluator<T, Price> eval, Price min, Price max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, Price> OverAny<T>(this IEvaluator<T, Price> eval, params Price[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, Price> OverOrEqualAny<T>(this IEvaluator<T, Price> eval, params Price[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, Price> UnderAny<T>(this IEvaluator<T, Price> eval, params Price[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, Price> UnderOrEqualAny<T>(this IEvaluator<T, Price> eval, params Price[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion

    #region Volume Only
    public static IEvaluator<T, Volume> IsBetween<T>(this IEvaluator<T, Volume> eval, Volume min, Volume max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, Volume> NotBetween<T>(this IEvaluator<T, Volume> eval, Volume min, Volume max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, Volume> OverAny<T>(this IEvaluator<T, Volume> eval, params Volume[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, Volume> OverOrEqualAny<T>(this IEvaluator<T, Volume> eval, params Volume[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, Volume> UnderAny<T>(this IEvaluator<T, Volume> eval, params Volume[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, Volume> UnderOrEqualAny<T>(this IEvaluator<T, Volume> eval, params Volume[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion

    #region DateTime Only
    public static IEvaluator<T, DateTime> IsBetween<T>(this IEvaluator<T, DateTime> eval, DateTime min, DateTime max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, DateTime> NotBetween<T>(this IEvaluator<T, DateTime> eval, DateTime min, DateTime max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, DateTime> OverAny<T>(this IEvaluator<T, DateTime> eval, params DateTime[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, DateTime> OverOrEqualAny<T>(this IEvaluator<T, DateTime> eval, params DateTime[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, DateTime> UnderAny<T>(this IEvaluator<T, DateTime> eval, params DateTime[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, DateTime> UnderOrEqualAny<T>(this IEvaluator<T, DateTime> eval, params DateTime[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion

    #region TimeSpan Only
    public static IEvaluator<T, TimeSpan> IsBetween<T>(this IEvaluator<T, TimeSpan> eval, TimeSpan min, TimeSpan max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, TimeSpan> NotBetween<T>(this IEvaluator<T, TimeSpan> eval, TimeSpan min, TimeSpan max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, TimeSpan> OverAny<T>(this IEvaluator<T, TimeSpan> eval, params TimeSpan[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, TimeSpan> OverOrEqualAny<T>(this IEvaluator<T, TimeSpan> eval, params TimeSpan[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, TimeSpan> UnderAny<T>(this IEvaluator<T, TimeSpan> eval, params TimeSpan[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, TimeSpan> UnderOrEqualAny<T>(this IEvaluator<T, TimeSpan> eval, params TimeSpan[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion

    #region Epoch Only
    public static IEvaluator<T, Epoch> IsBetween<T>(this IEvaluator<T, Epoch> eval, Epoch min, Epoch max, bool equally = false)
        => eval.Eval(e => min < e && e < max || equally && (e == min || e == max));

    public static IEvaluator<T, Epoch> NotBetween<T>(this IEvaluator<T, Epoch> eval, Epoch min, Epoch max, bool equally = false)
        => eval.Eval(e => min > e && e > max || equally && (e == min || e == max));

    public static IEvaluator<T, Epoch> OverAny<T>(this IEvaluator<T, Epoch> eval, params Epoch[] values)
        => eval.Eval(e => values.Any(v => e > v));

    public static IEvaluator<T, Epoch> OverOrEqualAny<T>(this IEvaluator<T, Epoch> eval, params Epoch[] values)
        => eval.Eval(e => values.Any(v => e >= v));

    public static IEvaluator<T, Epoch> UnderAny<T>(this IEvaluator<T, Epoch> eval, params Epoch[] values)
        => eval.Eval(e => values.Any(v => e < v));

    public static IEvaluator<T, Epoch> UnderOrEqualAny<T>(this IEvaluator<T, Epoch> eval, params Epoch[] values)
        => eval.Eval(e => values.Any(v => e <= v));
    #endregion
}