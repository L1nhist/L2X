namespace L2X.Exchange.Data.Responses;

public class OrderResponse : IResponse
{
    /// <summary>
    /// Mã Uid độc lập để định danh cho lệnh
    /// </summary>
    public Uuid Id { get; set; }

    /// <summary>
    /// Ký hiệu riêng của mỗi lệnh đặt
    /// </summary>
    public long OrderNo { get; set; }

    public string? Owner { get; set; }

    public string? Market { get; set; }

    /// <summary>
    /// Loại lệnh, gồm: [buy] lệnh mua loại tiền tệ chính, [sell] lệnh bán loại tiền tệ chính
    /// </summary>
    /// <see cref="OrderSide"/>
    public bool Side { get; set; }

    /// <summary>
    /// Loại lệnh đặt gồm có: [limit] lệnh giới hạn (mua theo giá & khối lượng),
    /// [market] lệnh thị trường (mua khối lượng theo giá thị trường),
    /// [post_only] lệnh đăng để xây dựng thị trường 
    /// </summary>
    /// <see cref="OrderType"/>
    public string? Type { get; set; }

    /// <summary>
    /// Điều kiện để thực hiện hoặc hủy lệnh, gồm có: [fok] khớp toàn bộ hoặc hủy,
    /// [ioc] khớp ngay hoặc tạm dừng, [mmp] lệnh tạo thị trường
    /// </summary>
    /// <see cref="OrderCondition"/>
    public string? Condition { get; set; }

    /// <summary>
    /// Giá đặt lệnh (trong trường hợp loại lệnh không phải là market)
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Giá điều kiện (nếu mua là giá tối đa sẽ mua, bán là giá tối thiểu sẽ bán) khi thị trường đến mức giá này thì dừng khớp lệnh này 
    /// </summary>
    public decimal? StopPrice { get; set; }

    /// <summary>
    /// Khối lượng đặt lệnh
    /// </summary>
    public decimal? Volume { get; set; }

    /// <summary>
    /// Tổng giá trị đặt lệnh (= Giá * Khối lượng) dùng cho lệnh thị trường
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// Khối lượng đã khớp lệnh
    /// </summary>
    public decimal? Origin { get; set; }

    /// <summary>
    /// Tổng giá trị đã khóa trong tài khoản
    /// </summary>
    public decimal? Locked { get; set; }

    /// <summary>
    /// Tổng giá trị đã trả sau khi khớp lệnh
    /// </summary>
    public decimal? Funded { get; set; }

    /// <summary>
    /// Tổng số lần khớp lệnh
    /// </summary>
    public int? Matched { get; set; }

    /// <summary>
    /// Trạng thái hiện tại của lệnh, gồm: [canceled] đã hủy, [filled] đã khớp hết,
    /// [waiting] chờ khớp, [mmp_canceled] hủy trong lệnh tạo thị trường
    /// </summary>
    /// <see cref="OrderState"/>
    public string? State { get; set; }

    /// <summary>
    /// Ngày tạo dạng EpouchTimestamp (tính đến milliseconds)
    /// </summary>
    public Epoch CreatedAt { get; set; }

    /// <summary>
    /// Ngày chỉnh sửa cuối cùng dạng EpouchTimestamp (tính đến milliseconds)
    /// </summary>
    public Epoch ModifiedAt { get; set; }

	/// <summary>
	/// Ngày kết thúc (khi khớp hoàn toàn lệnh hoặc hủy lệnh thành công) dạng EpouchTimestamp (tính đến milliseconds)
	/// </summary>
	public Epoch ExpiredAt { get; set; }

	/// <summary>
	/// Ngày kết thúc (khi khớp hoàn toàn lệnh hoặc hủy lệnh thành công) dạng EpouchTimestamp (tính đến milliseconds)
	/// </summary>
	public Epoch FinishedAt { get; set; }
}