namespace L2X.Exchange.Models;

/// <summary>
/// BookEntry is represent for each entry in OrderBook
/// </summary>
public interface IOrderBookEntry
{
    /// <summary>
    /// Price step in OrderBook
    /// </summary>
    decimal Price { get; set; }

    /// <summary>
    /// Total Volume at this Price step in OrderBook 
    /// </summary>
    decimal Volume { get; set; }
}