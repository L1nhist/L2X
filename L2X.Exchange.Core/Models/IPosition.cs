namespace L2X.Exchange.Models;

public interface IPosition
{
    /// <summary>
    /// Id of the position
    /// </summary>
    string? Id { get; set; }

    /// <summary>
    /// Identifier key for the user who place this order
    /// </summary>
    string Owner { get; }

    /// <summary>
    /// Symbol of the position
    /// </summary>
    string Symbol { get; set; }

    /// <summary>
    /// Position side
    /// </summary>
    bool? Side { get; set; }

    /// <summary>
    /// Leverage
    /// </summary>
    decimal Leverage { get; set; }

    /// <summary>
    /// Position quantity
    /// </summary>
    decimal Volume { get; set; }

    /// <summary>
    /// Entry price
    /// </summary>
    decimal? EntryPrice { get; set; }
    /// <summary>
    /// Liquidation price
    /// </summary>
    decimal? LiquidPrice { get; set; }

    /// <summary>
    /// Unrealized profit and loss
    /// </summary>
    decimal? UnrealizedPNL { get; set; }

    /// <summary>
    /// Realized profit and loss
    /// </summary>
    decimal? RealizedPNL { get; set; }

    /// <summary>
    /// Mark price
    /// </summary>
    decimal? MarkPrice { get; set; }

    /// <summary>
    /// Auto adding margin
    /// </summary>
    bool? AutoMargin { get; set; }

    /// <summary>
    /// Position margin
    /// </summary>
    decimal? PositionMargin { get; set; }

    /// <summary>
    /// Maintenance margin
    /// </summary>
    decimal? MaintananceMargin { get; set; }

    /// <summary>
    /// Is isolated
    /// </summary>
    bool? Isolated { get; set; }
}