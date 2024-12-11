namespace L2X.Exchange.Models;

/// <summary>
/// Tikcer is any kind of fiat or acsset that can exchange on the market 
/// </summary>
public interface ITicker
{
    /// <summary>
    /// Unique key as a shorten name identify the ticker on market 
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Recent matched price
    /// </summary>
    decimal? LastPrice { get; set; }

    /// <summary>
    /// Lowest traded price in 24 hours
    /// </summary>
    decimal? LowPrice { get; set; }

    /// <summary>
    /// Highest traded price in 24 hours
    /// </summary>
    decimal? HighPrice { get; set; }

    /// <summary>
    /// Average traded price in 24 hours
    /// </summary>
    decimal? Price24H { get; set; }

    /// <summary>
    /// Total traded volume in 24 hours
    /// </summary>
    decimal? Volume { get; set; }
}