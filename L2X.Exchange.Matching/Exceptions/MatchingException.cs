namespace L2X.Exchange.Matching.Exceptions;

public class MatchingException : Exception
{
    public MatchingException()
    { }

    public MatchingException(string message) : base(message)
    { }

    public MatchingException(string message, Exception innerException) : base(message, innerException)
    { }
}