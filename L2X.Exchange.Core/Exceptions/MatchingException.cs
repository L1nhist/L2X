namespace L2X.Exchange.Exceptions;

public class MatchingException : Exception
{
    #region Static Methods
    public static MatchingException New(string message)
        => new(message);

    public static MatchingException InvalidVersion(string field)
        => new($"Invalid Version of {field} Message.");

    public static MatchingException InvalidMessage<T>(string kind = "Message Size")
        => new($"Invalid {kind} of {nameof(T)}.");
    
    public static MatchingException InvalidSize<T>(int size = 1)
        => new($"{nameof(T)} Message Size must be: {size}.");

    public static MatchingException SetPriceLevelFail(int count)
        => new($"Cannot set price because pricelevel has {count} orders.");
    #endregion

    #region Constructions
    public MatchingException()
    { }

    public MatchingException(string message) : base(message)
    { }

    public MatchingException(string message, Exception innerException) : base(message, innerException)
    { }
    #endregion
}