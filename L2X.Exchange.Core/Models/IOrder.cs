namespace L2X.Exchange.Models;

/// <summary>
/// User placed Order to buy or sell their ticker on specific market 
/// </summary>
public interface IOrder
{
    /// <summary>
    /// Unique key for identify each order
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// Identifier key for the user who place this order
    /// </summary>
    string Owner { get; set; }

    /// <summary>
    /// true is BUY and false is SELL
    /// </summary>
    bool Side { get; set; }
    
    /// <summary>
    /// Specific price that user wanted to buy or sell for Limit Order.
    /// For Market Order, price will be null.
    /// </summary>
    decimal? Price { get; set; }
    
    /// <summary>
    /// Specific voulme that user wanted to buy or sell.
    /// For Maket Order, if Amount is specified then Volume will be null.
    /// </summary>
    decimal? Volume { get; set; }

    /// <summary>
    /// Total amount of value (Price * Volume) that user wanted to buy or sell for Market Order.
    /// If Volume is sepcified, Amount will be null.
    /// </summary>
    decimal? Amount { get; set; }

    /// <summary>
    /// System time when order is created 
    /// </summary>
    public long Timestamp { get; set; }
}