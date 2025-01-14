namespace L2X.Exchange.Data.Responses;

public class MatchFillResponse : IResponse
{
	public string? Buyer { get; set; }

	public string? Seller { get; set; }

	public string? Market { get; set; }

	public bool TakerType { get; set; }

	public decimal? BuyPrice { get; set; }

	public decimal? SellPrice { get; set; }

	public decimal? Price { get; set; }

	public decimal? BuyVolume { get; set; }

	public decimal? SellVolume { get; set; }

	public decimal? Volume { get; set; }

	public decimal? Amount { get; set; }

	public Epoch CreatedAt { get; set; }
}