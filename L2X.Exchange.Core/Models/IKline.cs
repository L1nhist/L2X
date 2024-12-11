namespace L2X.Exchange.Models;

/// <summary>
/// Market data at specific time of Exchange Session
/// </summary>
public interface IKline
{
    /// <summary>
    /// Price at the open time
    /// </summary>
    public decimal? OpenPrice { get; set; }

    /// <summary>
    /// Highest price of the kline
    /// </summary>
    public decimal? HighPrice { get; set; }

    /// <summary>
    /// Lowest price of the kline
    /// </summary>
    public decimal? LowPrice { get; set; }

    /// <summary>
    /// Close price of the kline
    /// </summary>
    public decimal? ClosePrice { get; set; }

    /// <summary>
    /// Volume of the kline
    /// </summary>
    public decimal? Volume { get; set; }

    /// <summary>
    /// Opening time of the kline
    /// </summary>
    public long Timestamp { get; set; }
}
