using System.Reflection;

namespace L2X.Core.Serialize;

public class BinarySerializer<T> : ISerializer<T, byte[]>
{
    private static readonly byte[] NulSeparator = [byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue];
    private static readonly byte[] StrSeparator = [ byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue ];

    public T? Deserialize(byte[] bytes)
    {
        try
        {
            var result = ReadType(new ReadOnlySpan<byte>(bytes), typeof(T), out int len);
            return result == null ? default : (T)result;
        }
        catch { }

        return default;
    }

    public byte[]? Serialize(T value)
    {
        if (value == null) return null;

        try
        {
            using var strm = new MemoryStream();
            WriteType(strm, value);

            strm.Flush();
            return strm.ToArray();
        }
        catch { }

        return null;
    }

    #region Read from byte array
    public static bool ReadBool(ReadOnlySpan<byte> span)
        => BitConverter.ToBoolean(span);

    public static char ReadChar(ReadOnlySpan<byte> span)
        => BitConverter.ToChar(span);

    public static short ReadShort(ReadOnlySpan<byte> span)
        => BitConverter.ToInt16(span);

    public static ushort ReadUShort(ReadOnlySpan<byte> span)
        => BitConverter.ToUInt16(span);

    public static int ReadInt(ReadOnlySpan<byte> span)
        => BitConverter.ToInt32(span);

    public static uint ReadUInt(ReadOnlySpan<byte> span)
        => BitConverter.ToUInt32(span);

    public static long ReadLong(ReadOnlySpan<byte> span)
        => BitConverter.ToInt64(span);

    public static ulong ReadULong(ReadOnlySpan<byte> span)
        => BitConverter.ToUInt64(span);

    public static float ReadFloat(ReadOnlySpan<byte> span)
        => BitConverter.ToSingle(span);

    public static double ReadDouble(ReadOnlySpan<byte> span)
        => BitConverter.ToDouble(span);

    public static decimal? ReadDecimal(ReadOnlySpan<byte> span)
    {
        Span<int> bits = [
            (span[0]) | (span[1] << 8) | (span[2] << 16) | (span[3] << 24), //low
            (span[4]) | (span[5] << 8) | (span[6] << 16) | (span[7] << 24), //middle
            (span[8]) | (span[9] << 8) | (span[10] << 16) | (span[11] << 24), //high
            (span[12]) | (span[13] << 8) | (span[14] << 16) | (span[15] << 24) //flags
        ];
        return new decimal(bits);
    }

    public static string? ReadString(ReadOnlySpan<byte> span, out int len)
    {
        len = 0;
        if (span[0] == byte.MaxValue && span[1] == byte.MaxValue && span[2] == byte.MaxValue && span[3] == byte.MaxValue) return null;
        
        var last = span.Length - 3;
        while (len < last)
        {
            if (span[len + 4] == byte.MaxValue && span[len + 5] == byte.MaxValue && span[len + 6] == byte.MaxValue && span[len + 7] == byte.MaxValue) break;
            len++;
        }
        return Convert.ToBase64String(span[..len]);
    }

    public static object? ReadType(ReadOnlySpan<byte> span, Type type, out int len)
    {
        len = 0;
        try
        {
            var nxt = 0;
            var idx = -1;
            var slen = span.Length;
            var result = Activator.CreateInstance(type);
            if (result == null) return null;

            var props = result.GetProps();
            foreach (var p in props)
            {
                if (idx >= slen) break;

                if (ReadByTypeCode(p, result, span[idx..], out nxt))
                {
                    idx += nxt;
                }
                else if (ReadByStrongType(p, result, span[idx..], out nxt))
                {
                    idx += nxt;
                }
                else
                {
                    p.SetValue(result, ReadType(span[idx..], p.PropertyType, out nxt));
                    idx += nxt;
                }
            }

            len = idx + 1;
            return result;
        }
        catch { }

        return null;
    }

    private static bool ReadByTypeCode(PropertyInfo prop, object result, ReadOnlySpan<byte> span, out int len)
    {
        len = 0;
        switch (Type.GetTypeCode(prop.PropertyType))
        {
            case TypeCode.Empty:
                if (span[0] == byte.MaxValue && span[1] == byte.MaxValue && span[2] == byte.MaxValue && span[3] == byte.MaxValue && span[4] == byte.MaxValue && span[5] == byte.MaxValue)
                    prop.SetValue(result, null);
                return true;
            case TypeCode.Boolean:
                prop.SetValue(result, ReadBool(span));
                len = 1;
                return true;
            case TypeCode.Byte:
                prop.SetValue(result, span[0]);
                len = 1;
                return true;
            case TypeCode.SByte:
                prop.SetValue(result, (sbyte)span[0]);
                len = 1;
                return true;
            case TypeCode.Char:
                prop.SetValue(result, ReadChar(span));
                len = 2;
                return true;
            case TypeCode.Int16:
                prop.SetValue(result, ReadShort(span));
                len = 2;
                return true;
            case TypeCode.UInt16:
                prop.SetValue(result, ReadUShort(span));
                len = 2;
                return true;
            case TypeCode.Int32:
                prop.SetValue(result, ReadInt(span));
                len = 4;
                return true;
            case TypeCode.UInt32:
                prop.SetValue(result, ReadUInt(span));
                len = 4;
                return true;
            case TypeCode.Int64:
                prop.SetValue(result, ReadLong(span));
                len = 8;
                return true;
            case TypeCode.UInt64:
                prop.SetValue(result, ReadULong(span));
                len = 8;
                return true;
            case TypeCode.Single:
                prop.SetValue(result, ReadFloat(span));
                len = 4;
                return true;
            case TypeCode.Double:
                prop.SetValue(result, ReadDouble(span));
                len = 8;
                return true;
            case TypeCode.Decimal:
                prop.SetValue(result, ReadDecimal(span));
                len = 16;
                return true;
            case TypeCode.DateTime:
                prop.SetValue(result, new DateTime(ReadLong(span)));
                len = 8;
                return true;
            case TypeCode.String:
                prop.SetValue(result, ReadString(span, out int nxt));
                len = nxt + 8;
                return true;
        }

        return false;
    }

    private static bool ReadByStrongType(PropertyInfo prop, object result, ReadOnlySpan<byte> span, out int len)
    {
        len = 0;
        if (prop.PropertyType.IsInstanceOfType(typeof(Epoch)))
        {
            prop.SetValue(result, new DateTime(ReadLong(span)));
            len = 8;
            return true;
        }
        else if (prop.PropertyType.IsInstanceOfType(typeof(Price)))
        {
            var prc = ReadDecimal(span) ?? decimal.MinValue;
            len = 16;

            var unit = "";
            if (ReadChar(span[len..]) == '$')
            {
                len += 2;
                unit = ReadString(span[len..], out int nxt) ?? "";
                len += nxt + 4;
            }

            prop.SetValue(result, new Price(prc, unit));
            return true;
        }
        else if (prop.PropertyType.IsInstanceOfType(typeof(Sequence)))
        {
            prop.SetValue(result, new Sequence(ReadULong(span)));
            len = 8;
            return true;
        }
        else if (prop.PropertyType.IsInstanceOfType(typeof(Volume)))
        {
            prop.SetValue(result, new Volume(ReadDecimal(span) ?? decimal.MinValue));
            len = 16;
            return true;
        }

        return false;
    }
    #endregion

    #region Write to byte array
    public static void WriteBool(MemoryStream strm, bool value)
        => strm.WriteByte((byte)(value ? 1 : 0));

    public static void WriteChar(MemoryStream strm, char value)
        => strm.Write([(byte)value, (byte)(value >> 8)]);

    public static void WriteShort(MemoryStream strm, short value)
        => strm.Write(BitConverter.GetBytes(value));

    public static void WriteUShort(MemoryStream strm, ushort value)
        => strm.Write(BitConverter.GetBytes(value));

    public static void WriteInt(MemoryStream strm, int value)
        => strm.Write(BitConverter.GetBytes(value));

    public static void WriteUInt(MemoryStream strm, uint value)
        => strm.Write(BitConverter.GetBytes(value));

    public static void WriteLong(MemoryStream strm, long value)
        => strm.Write(BitConverter.GetBytes(value));

    public static void WriteULong(MemoryStream strm, ulong value)
        => strm.Write(BitConverter.GetBytes(value));

    public static void WriteFloat(MemoryStream strm, float value)
        => strm.Write(BitConverter.GetBytes(value));

    public static void WriteDouble(MemoryStream strm, double value)
        => strm.Write(BitConverter.GetBytes(value));

    public static void WriteDecimal(MemoryStream strm, decimal value)
    {
        Span<int> arrs = stackalloc int[4];
        decimal.GetBits(value, arrs);
        foreach (var i in arrs)
        {
            for (int j = 0; j < 4; j ++)
            {
                strm.WriteByte((byte)(i >> (j * 8)));
            }
        }
    }

    public static void WriteString(MemoryStream strm, string value)
    {
        strm.Write(StrSeparator);
        strm.Write(Convert.FromBase64String(value));
        strm.Write(StrSeparator);
    }

    public static void WriteType(MemoryStream strm, object value)
    {
        if (value == null) return;

        try
        {
            var props = value.GetProps();
            foreach (var p in props)
            {
                var v = p.GetValue(value);
                if (v == null)
                {
                    strm.Write(NulSeparator);
                    continue;
                }

                if (WriteByTypeCode(strm, v)) continue;
                if (!WriteByStrongType(strm, v)) continue;

                WriteType(strm, v);
            }
        }
        catch { }
    }

    private static bool WriteByTypeCode(MemoryStream strm, object value)
    {
        switch (Type.GetTypeCode(value.GetType()))
        {
            case TypeCode.Boolean:
                WriteBool(strm, (bool)value);
                return true;
            case TypeCode.Byte:
                strm.WriteByte((byte)value);
                return true;
            case TypeCode.SByte:
                strm.WriteByte((byte)value);
                return true;
            case TypeCode.Char:
                WriteChar(strm, (char)value);
                return true;
            case TypeCode.Int16:
                WriteShort(strm, (short)value);
                return true;
            case TypeCode.UInt16:
                WriteUShort(strm, (ushort)value);
                return true;
            case TypeCode.Int32:
                WriteInt(strm, (int)value);
                return true;
            case TypeCode.UInt32:
                WriteUInt(strm, (uint)value);
                return true;
            case TypeCode.Int64:
                WriteLong(strm, (long)value);
                return true;
            case TypeCode.UInt64:
                WriteULong(strm, (ulong)value);
                return true;
            case TypeCode.Single:
                WriteFloat(strm, (float)value);
                return true;
            case TypeCode.Double:
                WriteDouble(strm, (double)value);
                return true;
            case TypeCode.Decimal:
                WriteDecimal(strm, (decimal)value);
                return true;
            case TypeCode.DateTime:
                WriteLong(strm, ((DateTime)value).Ticks);
                return true;
            case TypeCode.String:
                WriteString(strm, (string)value);
                return true;
        }

        return false;
    }

    private static bool WriteByStrongType(MemoryStream strm, object value)
    {
        if (value is Epoch epoch)
        {
            WriteLong(strm, epoch.Timestamp);
            return true;
        }

        if (value is Price price)
        {
            WriteDecimal(strm, price);
            if (!Util.IsEmpty(price.Unit))
            {
                WriteChar(strm, '$');
                WriteString(strm, price.Unit);
            }

            return true;
        }

        if (value is Sequence quant)
        {
            WriteLong(strm, quant);
            return true;
        }

        if (value is Volume vol)
        {
            WriteDecimal(strm, vol);
            return true;
        }

        return false;
    }
    #endregion
}