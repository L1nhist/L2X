namespace L2X.Exchange.Data.Entities;

[Table("Market")]
[Index("Name", IsUnique = true)]
public class Market : Entity, IAuditable, IRemovable, ISortable
{
    /// <summary>
    /// Mã Uid của Engine
    /// </summary>
    public Guid? EngineId { get; set; }

    /// <summary>
    /// Mã Uid của đơn vị tiền tệ chính trong cặp tiền tệ trao đổi
    /// </summary>
    public Guid? BaseId { get; set; }

    /// <summary>
    /// Mã Uid của đơn vị tiền tệ đối ứng trong cặp tiền tệ trao đổi
    /// </summary>
    public Guid? QuoteId { get; set; }

    /// <summary>
    /// Mã rút gọn (tên viết tắt) cho kênh giao dịch,
    /// thường sử dụng là tên ghép của cặp tiền tệ đang trao đổi theo định dạng xxx-yyy
    /// </summary>
    [StringLength(30)]
    public string? Name { get; set; }

    /// <summary>
    /// Tên kênh giao dịch
    /// </summary>
    [StringLength(120)]
    public string? Fullname { get; set; }

    /// <summary>
    /// Dữ liệu meta phục vụ rút tiền
    /// Bank Account details for fiat Beneficiary in JSON format.For crypto it's blockchain address
    /// </summary>
    [StringLength(1600)]
    public string? Data { get; set; }

    /// <summary>
    /// Độ chính xác sau dấu thập phân của khối lượng đặt lệnh
    /// </summary>
    public short? VolumePrecision { get; set; }

    /// <summary>
    /// Độ chính xác sau dấu thập phân của giá đặt lệnh
    /// </summary>
    public short? PricePrecision { get; set; }

    /// <summary>
    /// Khối lượng đặt lệnh thấp nhất, không thể đặt thấp hơn
    /// </summary>
    public decimal? MinVolumn { get; set; }

    /// <summary>
    /// Giá khớp lệnh thấp nhất
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Giá khớp lệnh cao nhất
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Loại hình giao dịch trên kênh giao dịch này, gồm:
    /// [swap], [future], [margin], [option]
    /// </summary>
    /// <see cref="InstrumentType"></see>
    [StringLength(20)]
    public string? Type { get; set; }

    /// <summary>
    /// Trạng thái của kênh giao dịch, gồm:
    /// [enabled], [disabled], [hidden]
    /// </summary>
    /// <see cref="EntityState"></see>
    [StringLength(20)]
    public string? State { get; set; }

    /// <summary>
    /// Thứ tự để sắp xếp
    /// </summary>
    public int? Position { get; set; }

    /// <summary>
    /// Người tạo (Username/Email)
    /// </summary>
    [StringLength(250)]
    public string? Creator { get; set; }

    /// <summary>
    /// Người chỉnh sửa cuối cùng (Username/Email)
    /// </summary>
    [StringLength(250)]
    public string? Modifier { get; set; }

    /// <summary>
    /// Ngày tạo dạng EpouchTimestamp (tính đến milliseconds)
    /// </summary>
    public long CreatedAt { get; set; }

    /// <summary>
    /// Ngày chỉnh sửa cuối cùng dạng EpouchTimestamp (tính đến milliseconds)
    /// </summary>
    public long? ModifiedAt { get; set; }

    /// <summary>
    /// Nếu true là đã bị loại bỏ và không hiển thị nữa, đang sử dụng thì là false
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Loại tiền tệ chính được quy định trong cặp tiền tệ trao đổi
    /// </summary>
    //[ForeignKey(nameof(EngineId))]
    //public Engine? Engine { get; set; }

    /// <summary>
    /// Loại tiền tệ chính được quy định trong cặp tiền tệ trao đổi
    /// </summary>
    [ForeignKey(nameof(BaseId))]
    public Ticker? BaseUnit { get; set; }

    /// <summary>
    /// Loại tiền tệ để trao đổi trong cặp tiền tệ trao đổi
    /// </summary>
    [ForeignKey(nameof(QuoteId))]
    public Ticker? QuoteUnit { get; set; }

    /// <summary>
    /// Danh sách các lệnh đặt
    /// </summary>
    public ICollection<Order>? Orders { get; set; }

    /// <summary>
    /// Danh sách các lệnh đặt
    /// </summary>
    public ICollection<PreOrder>? PreOrders { get; set; }

    /// <summary>
    /// Danh sách các lệnh khớp
    /// </summary>
    public ICollection<Match>? Matchers { get; set; }

    ///// <summary>
    ///// Danh sách các Hạn mức giao dịch
    ///// </summary>
    //public ICollection<TradeRule>? Quotas { get; set; }
}