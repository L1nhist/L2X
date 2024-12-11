namespace L2X.Exchange.Models;

public interface ITrade
{
    /// <summary>
    /// Price of the trade
    /// </summary>
    decimal Price { get; set; }

    /// <summary>
    /// Quantity of the trade
    /// </summary>
    decimal Volume { get; set; }

    /// <summary>
    /// Timestamp of the trade
    /// </summary>
    long Timestamp { get; set; }
}