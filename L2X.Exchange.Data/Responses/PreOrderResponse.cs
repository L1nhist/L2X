namespace L2X.Exchange.Data.Responses;

public class PreOrderResponse
{
	/// <summary>
	/// Mã Uid độc lập để định danh cho lệnh
	/// </summary>
	public long Id { get; set; }

	public string? Owner { get; set; }

	public string? Market { get; set; }

	public bool Side { get; set; }

	public string? Type { get; set; }

	public string? Condition { get; set; }

	public decimal? Price { get; set; }

	public decimal? StopPrice { get; set; }

	public decimal? Volume { get; set; }

	public decimal? Amount { get; set; }

	public decimal? Origin { get; set; }

	public Epoch CreatedAt { get; set; }

	public Epoch? ExpiredAt { get; set; }
}