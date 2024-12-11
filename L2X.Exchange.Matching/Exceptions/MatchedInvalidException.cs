namespace L2X.Exchange.Matching.Exceptions;

public class MatchedInvalidException : MatchingException
{
    private const string _version = "Invalid Version of {0} Message.";
    private const string _volume = "Order volume is less then requested fill volume.";

    private static string GetMessage(string field)
        => (field?.Trim()?.ToLower()) switch
        {
            "version" => string.Format(_version, field),
            "volume" => _volume,
            _ => $"Invalid {field}",
        };

    public MatchedInvalidException(int count)
        : base($"Cannot set price because pricelevel has {count} orders.")
    { }

    public MatchedInvalidException(string field)
        : base(GetMessage(field))
    { }

    public MatchedInvalidException(string type, string kind = "Message Size")
        : base($"Invalid {kind} of {type}.")
    { }

    public MatchedInvalidException(string type, int size)
        : base($"{type} Message Size must be: {size}.")
    { }
}