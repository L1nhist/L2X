namespace L2X.Exchange.Models;

public class MarketInfo
{
    public decimal LastPrice { get; set; }

    public decimal MinPrice { get; set; }

    public decimal MaxPrice { get; set; }

    public decimal PriceChg { get; set; }

    public decimal PriceRate { get; set; }

    public decimal TotalAsks { get; set; }

    public decimal TotalBids { get; set; }
}
