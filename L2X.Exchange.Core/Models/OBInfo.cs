namespace L2X.Exchange.Models;

public class OBInfo : IOrderBookEntry
{
    public decimal Price { get; set; }

    public decimal Volume { get; set; }
}