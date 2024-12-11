namespace L2X.Exchange.Enums;

public enum OrderState : byte
{
    /// <summary>
    /// placed and not fully filled order
    /// </summary>
    Active,
    /// <summary>
    /// canceled order
    /// </summary>
    Canceled,
    /// <summary>
    /// filled order
    /// </summary>
    Filled
}

public enum OrderType : byte
{
    /// <summary>
    /// Limit type
    /// </summary>
    Limit,
    /// <summary>
    /// Market type
    /// </summary>
    Market,
    /// <summary>
    /// Other order type
    /// </summary>
    Other
}

public enum OrderCondition : byte
{
    None = 0,
    ImmediateOrCancel = 1,
    BookOrCancel = 2,
    FillOrKill = 4,
}

public enum PositionSide : byte
{
    /// <summary>
    /// Long position
    /// </summary>
    Long,
    /// <summary>
    /// Short position
    /// </summary>
    Short,
    /// <summary>
    /// Both
    /// </summary>
    Both
}