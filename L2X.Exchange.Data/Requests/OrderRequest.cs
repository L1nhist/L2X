namespace L2X.Exchange.Data.Requests;

public class OrderRequest : IRequest
{
    /// <summary>
    /// Mã Uid độc lập để định danh cho thành viên đặt lệnh
    /// </summary>
    public string? Owner { get; set; }

    /// <summary>
    /// Mã Uid độc để dịnh danh cho kênh trao đổi tương ứng
    /// </summary>
    public string? Symbol { get; set; }

    /// <summary>
    /// Loại lệnh, gồm: [buy] lệnh mua loại tiền tệ chính, [sell] lệnh bán loại tiền tệ chính
    /// </summary>
    /// <see cref="OrderSide"/>
    [StringLength(20)]
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
    [StringLength(20)]
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
}