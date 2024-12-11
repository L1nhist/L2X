using System.Security.Cryptography;

namespace L2X.Core.Utilities;

/// <summary>
/// All supported class for hash algorithm, randomize,
/// encode, decode & encrypt, decrypt with text content
/// </summary>
public sealed class Crypto
{
    #region Properties
    private const string _charseeds = "UM7vgDhK1afd5ERrWs2tGHu3BwLYcmZ8jyn4pJFQz6bSBox9qCXTkeR0";
    private const string _cryptcode = "CB06cfE507a1";

    public static Crypter DES => new(1);

    public static Crypter RC2 => new(2);

    public static Crypter AES => new(3);

    public static Crypter TripleDES => new(0);

    public static Hasher MD5 => new(0);

    public static Hasher SHA1 => new(1);

    public static Hasher SHA256 => new(2);

    public static Hasher SHA384 => new(3);

    public static Hasher SHA512 => new(5);

    public static Encoder ASII => new(1);

    public static Encoder UTF8 => new(2);

    public static Encoder UNICODE => new(3);

    public static Encoder BASE64 => new(4);

    public static Encoder HEX => new(0);

    public static Randomizer Random => new();
    #endregion

    public static string PasswordHash(string password, string? salt = "")
    {
        if (!Util.IsEmpty(salt, true))
            password += salt;

        var result = MD5.Hash(password);
        return result + _charseeds[result.Length % _charseeds.Length];
    }

    public static bool IsPassword(string password, string hash, string? salt = "")
        => Util.IsEquals(PasswordHash(password, salt), hash, true);

    #region Cryption sub classes
    public class Crypter
    {
        private byte[] mKey = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24];
        private byte[] mIV = [65, 110, 68, 26, 69, 178, 200, 219];
        private readonly byte[] _saltBytes = [0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76];
        private string _password = _cryptcode;
        private readonly SymmetricAlgorithm _algo;

        #region Constructors
        internal Crypter(int type)
        {
            _algo = GetAlgorithm(type);
            CalculateNewKeyAndIV();
        }
        #endregion

        #region Encryption
        /// <summary>
        /// Encrypt a string
        /// </summary>
        /// <param name="text">A text content</param>
        /// <returns>An encrypted result</returns>
        public string Encrypt(string text)
            => string.IsNullOrEmpty(text) ? "" : Convert.ToBase64String(Crypting(Encoding.UTF8.GetBytes(text), true));

        /// <summary>
        /// Encrypt string with user defined password
        /// </summary>
        /// <param name="text">A text content</param>
        /// <param name="password">Password for the cryption</param>
        /// <returns>An encrypted result</returns>
        public string Encrypt(string text, string password)
        {
            _password = password;
            return Encrypt(text);
        }
        #endregion

        #region Decryption
        /// <summary>
        /// Decrypts a string
        /// </summary>
        /// <param name="text">An encrypted content</param>
        /// <returns>A decrypted result</returns>
        public string Decrypt(string text)
            => string.IsNullOrEmpty(text) ? "" : Encoding.UTF8.GetString(Crypting(Convert.FromBase64String(text), false));

        /// <summary>
        /// Decrypts a string using a user defined password key
        /// </summary>
        /// <param name="text">An encrypted content</param>
        /// <param name="password">Password for the cryption</param>
        /// <returns>a decrypted string</returns>
        public string Decrypt(string text, string password)
        {
            _password = password;
            return Decrypt(text);
        }
        #endregion

        #region Symmetric Engine
        /// <summary>
        /// Performs the actual enc/dec.
        /// </summary>
        /// <param name="bytes">Input byte array</param>
        /// <param name="isEncrypt">Wheather or not to perform enc/dec</param>
        /// <returns>Byte array output</returns>
        private byte[] Crypting(byte[] bytes, bool isEncrypt = true)
        {
            using var stream = new MemoryStream();
            ICryptoTransform transform = GetTransform(isEncrypt);
            using var cryptStream = new CryptoStream(stream, transform, CryptoStreamMode.Write);
            cryptStream.Write(bytes, 0, bytes.Length);
            cryptStream.FlushFinalBlock();

            //get result
            byte[] result = stream.ToArray();
            cryptStream.Close();
            return result;
        }

        /// <summary>
        /// Get the symmetric engine and creates the encyptor/decryptor
        /// </summary>
        /// <param name="isEncrypt">Wheather or not to perform enc/dec</param>
        /// <returns>Cryption engine reuslt</returns>
        private ICryptoTransform GetTransform(bool isEncrypt)
        {
            _algo.Key = mKey;
            _algo.IV = mIV;
            return isEncrypt ? _algo.CreateEncryptor() : _algo.CreateDecryptor();
        }

        /// <summary>
        /// Get the specific symmetric algorithm acc. to the cryptotype
        /// </summary>
        /// <returns>SymmetricAlgorithm</returns>
        private SymmetricAlgorithm GetAlgorithm(int type)
        {
            return type switch
            {
                1 => System.Security.Cryptography.DES.Create(),
                2 => System.Security.Cryptography.RC2.Create(),
                3 => System.Security.Cryptography.Aes.Create(),
                _ => System.Security.Cryptography.TripleDES.Create(),
            };
        }

        /// <summary>
        /// Calculates the key and IV acc. to the symmetric method from the password
        /// key and IV size dependant on symmetric method
        /// </summary>
        private void CalculateNewKeyAndIV()
        {
            //use salt so that key cannot be found with dictionary attack
            _password = string.IsNullOrWhiteSpace(_password) ? _cryptcode : _password;
            var pdb = new PasswordDeriveBytes(_password, _saltBytes);
            mKey = pdb.GetBytes(_algo.KeySize / 8);
            mIV = pdb.GetBytes(_algo.BlockSize / 8);
        }
        #endregion
    }

    public class Hasher
    {
        private readonly HashAlgorithm _algo;

        #region Construction
        internal Hasher(int type)
        {
            _algo = GetAlgorithm(type);
        }
        #endregion

        #region Hash text
        public string Hash(string text)
            => string.IsNullOrEmpty(text) ? "" : ComputeHash(text);

        /// <summary>
        ///     returns true if the input text is equal to hashed text
        /// </summary>
        /// <param name="text">unhashed text to test</param>
        /// <param name="hash">already hashed text</param>
        /// <returns>boolean true or false</returns>
        public bool IsEquals(string text, string hash)
            => Util.IsEquals(Hash(text), hash, true);
        #endregion

        #region Hashing Engine
        /// <summary>
        ///     computes the hash code and converts it to string
        /// </summary>
        /// <param name="text">input text to be hashed</param>
        /// <returns>hashed string</returns>
        private string ComputeHash(string text)
            => Convert.ToBase64String(_algo.ComputeHash(UTF8.AsBytes(text)));

        /// <summary>
        ///     returns the specific hashing alorithm
        /// </summary>
        /// <param name="hashingType">type of hashing to use</param>
        /// <returns>HashAlgorithm</returns>
        private HashAlgorithm GetAlgorithm(int type)
        {
            return type switch
            {
                1 => System.Security.Cryptography.SHA1.Create(),
                2 => System.Security.Cryptography.SHA256.Create(),
                3 => System.Security.Cryptography.SHA384.Create(),
                5 => System.Security.Cryptography.SHA512.Create(),
                _ => System.Security.Cryptography.MD5.Create(),
            };
        }
        #endregion
    }
    #endregion

    #region Encode sub classes
    public class Encoder
    {
        private readonly int _type;

        internal Encoder(int type)
        {
            _type = type;
        }

        public byte[] AsBytes(string text)
        {
            if (Util.IsEmpty(text)) return [];

            try
            {
                switch (_type)
                {
                    case 1: return Encoding.ASCII.GetBytes(text);
                    case 2: return Encoding.UTF8.GetBytes(text);
                    case 3: return Encoding.Unicode.GetBytes(text);
                    case 4: return Convert.FromBase64String(text);
                    default:
                        var bytes = new byte[text.Length / 2];
                        for (var i = 0; i < bytes.Length; i++)
                        {
                            bytes[i] = Convert.ToByte(text.Substring(i * 2, 2), 16);
                        }
                        return bytes;
                }
            }
            catch { }

            return [];
        }

        public string AsString(byte[]? bytes)
        {
            if (Util.IsEmpty(bytes)) return "";

            try
            {
                return _type switch
                {
                    1 => Encoding.ASCII.GetString(bytes),
                    2 => Encoding.UTF8.GetString(bytes),
                    3 => Encoding.Unicode.GetString(bytes),
                    4 => Convert.ToBase64String(bytes),
                    _ => new(bytes.SelectMany(b => b.ToString("X2").ToCharArray()).ToArray()),
                };
            }
            catch { }

            return "";
        }

        public string Encode(string text, Encoding? encoding = null)
            => Util.IsEmpty(text) ? "": AsString((encoding ?? Encoding.UTF8).GetBytes(text));

        public string Decode(string text, Encoding? encoding = null)
            => Util.IsEmpty(text) ? "" : (encoding ?? Encoding.UTF8).GetString(AsBytes(text));
    }

    public class Randomizer
    {
        private int _last = -1;

        internal Randomizer()
        { }

        public byte[] NewBytes(int length)
            => RandomNumberGenerator.GetBytes(length);

        public int NewNumber(int min = int.MinValue, int max = int.MaxValue)
        {
            var result = _last;
            while (result == _last)
            {
                result = RandomNumberGenerator.GetInt32(min, max);
            }

            return (_last = result);
        }

        public int NewNumber(int length = 1)
        {
            if (length < 1) return 0;

            var result = _last;
            var min = (int)Math.Pow(10, length - 1);
            var max = (int)Math.Pow(10, length) - 1;
            while (result == _last)
            {
                result = RandomNumberGenerator.GetInt32(min, max);
            }

            return result;
        }

        public char NewChar()
            => _charseeds[NewNumber(0, _charseeds.Length)];

        public string NewString(int length)
        {
            var sb = new StringBuilder();
            while (sb.Length < length)
            {
                sb.Append(NewChar());
            }

            return sb.ToString();
        }

        public string Replace(string text, string replace)
        {
            if (Util.IsEmpty(text)) return "";
            if (Util.IsEmpty(replace)) return text;

            if (replace.Length > _charseeds.Length)
            {
                while (replace.Length > 0)
                {
                    if (replace.Length <= _charseeds.Length) return Replace(text, replace);
                    
                    text = Replace(text, replace[.._charseeds.Length]);
                    replace = replace[_charseeds.Length..];
                }
            }

            var num = NewNumber(0, _charseeds.Length);
            var rands = _charseeds[(num - replace.Length < 0 ? 0 : num)..replace.Length];
            for (var i = 0; i < replace.Length; i++)
            {
                text = text.Replace(replace[i], rands[i]);
            }

            return text;
        }

        public string NewGuid()
        {
            var code = Convert.ToBase64String(Guid.NewGuid().ToByteArray())[..^2];
            foreach (char c in "+/")
            {
                if (code.Contains(c))
                {
                    code = code.Replace(c, NewChar());
                }
            }

            return code;
        }
    }
    #endregion
}