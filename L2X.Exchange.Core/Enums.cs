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

public enum OrderMode : byte
{
    /// <summary>
    /// Limit type
    /// </summary>
    Limit = 1,
    /// <summary>
    /// Market type
    /// </summary>
    Market = 2,
    /// <summary>
    /// Stop Limit type
    /// </summary>
    Stop_Limit = 3,
    /// <summary>
    /// Stop Market type
    /// </summary>
    Stop_Market = 4,
    /// <summary>
    /// Iceberg (Special limit that match partialy)
    /// </summary>
    Iceberg = 5,
    /// <summary>
    /// Other order type
    /// </summary>
    Others = 9,
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