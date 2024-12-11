namespace L2X.Exchange.Models;

/// <summary>
/// A symbol is a marketplace for user exchange between a pair of tickers 
/// </summary>
public interface ISymbol
{
    /// <summary>
    /// Name of the symbol (pair of tickers that trade)
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Minimal Volume of Order to be placed
    /// </summary>
    decimal? MinVolume { get; set; }

    /// <summary>
    /// Step to increase Price of Order
    /// </summary>
    decimal? PriceStep { get; set; }

    /// <summary>
    /// Step to increase Volume of Order
    /// </summary>
    decimal? VolumeStep { get; set; }

    /// <summary>
    /// The exact amount of decimals number for price
    /// </summary>
    byte? PricePrecision { get; set; }

    /// <summary>
    /// The exact amount of decimals number for volume
    /// </summary>
    byte? QuantityDecimals { get; set; }
}