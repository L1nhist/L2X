namespace L2X.Services.Models.Matching;

public class MTrade : ITrade
{
    #region Properties
    public OrderId MakerOrder { get; set; }

    public OrderId TakerOrder { get; set; }

    public string MakerOwner { get; set; } = "";

    public string TakerOwner { get; set; } = "";

    public bool TakerSide { get; set; }

    public decimal Price { get; set; }

    public decimal Volume { get; set; }

    public decimal AskRemain { get; set; }

    public decimal AskFee { get; set; }

    public decimal BidCost { get; set; }

    public decimal BidFee { get; set; }

    public long Timestamp { get; set; }
    #endregion
}