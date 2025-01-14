namespace L2X.Exchange.Data.Entities;

/// <summary>
/// Thông tin lượt khớp lệnh giữa các lệnh đặt do hệ thống khớp được
/// </summary>
[Table("Match")]
public class Match : Entity, IRemovable
{
    /// <summary>
    /// Mã Uid của thành viên đặt lệnh
    /// </summary>
    public Guid MakerId { get; set; }

    /// <summary>
    /// Mã Uid của thành viên đặt lệnh
    /// </summary>
    public Guid TakerId { get; set; }

    /// <summary>
    /// Mã Uid của kênh giao dịch đang khớp
    /// </summary>
    public Guid MarketId { get; set; }

    /// <summary>
    /// Mã Uid của lệnh đặt đã khớp
    /// </summary>
    public Guid MkrOrdId { get; set; }

    /// <summary>
    /// Mã Uid của lệnh đặt đã khớp
    /// </summary>
    public Guid TkrOrdId { get; set; }

    /// <summary>
    /// Trade taker order type (sell or buy)
    /// </summary>
    /// <see cref="OrderSide"/>
    [StringLength(20)]
    public bool TakerType { get; set; }

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

    /// <summary>
    /// Ngày tạo dạng EpouchTimestamp (tính đến milliseconds)
    /// </summary>
    public long CreatedAt { get; set; }

    /// <summary>
    /// Nếu true là đã bị loại bỏ và không hiển thị nữa, đang sử dụng thì là false
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey(nameof(MakerId))]
    public virtual Member? Maker { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey(nameof(TakerId))]
    public virtual Member? Taker { get; set; }

    /// <summary>
    /// Kênh giao dịch tương ứng
    /// </summary>
    [ForeignKey(nameof(MarketId))]
    public virtual Market? Market { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey(nameof(MkrOrdId))]
    public virtual Order? MakerOrder { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [ForeignKey(nameof(TkrOrdId))]
    public virtual Order? TakerOrder { get; set; }
}