namespace L2X.Exchange.Data.Requests;

public class MatchRequest
{
	/// <summary>
	/// Mã Uid của thành viên đặt lệnh
	/// </summary>
	public string? Maker { get; set; }

	/// <summary>
	/// Mã Uid của thành viên đặt lệnh
	/// </summary>
	public string? Taker { get; set; }

	/// <summary>
	/// Mã Uid của kênh giao dịch đang khớp
	/// </summary>
	public string? Market { get; set; }

	/// <summary>
	/// Mã Uid của lệnh đặt đã khớp
	/// </summary>
	public Uuid MakerOrder { get; set; }

	/// <summary>
	/// Mã Uid của lệnh đặt đã khớp
	/// </summary>
	public Uuid TakerOrder { get; set; }

	/// <summary>
	/// Trade taker order type (sell or buy)
	/// </summary>
	/// <see cref="OrderSide"/>
	[StringLength(20)]
	public string? TakerType { get; set; }

	/// <summary>
	/// Giá khớp lệnh
	/// </summary>
	public decimal? Price { get; set; }

	/// <summary>
	/// Khối lượng khớp lệnh
	/// </summary>
	public decimal? Volume { get; set; }

	/// <summary>
	/// Tổng giá trị khớp lệnh (Giá * Khối lượng)
	/// </summary>
	public decimal? Amount { get; set; }

	/// <summary>
	/// Mức phí thực tế cho Lệnh mua tương ứng
	/// </summary>
	public decimal? MakerFee { get; set; }

	/// <summary>
	/// Mức phí thực tế cho Lệnh bán đối ứng
	/// </summary>
	public decimal? TakerFee { get; set; }
}