namespace L2X.Exchange.Models;

/// <summary>
/// OrderBook keeps all buy and sell orders of each ongoing marketplace
/// </summary>
public interface IOrderBook
{
    /// <summary>
    /// All sell orders in OrderBook, sorted by Price descending
    /// </summary>
    IEnumerable<IBookEntry> Asks { get; }

    /// <summary>
    /// All buy orders in OrderBook, sorted by Price ascending
    /// </summary>
    IEnumerable<IBookEntry> Bids { get; }
}