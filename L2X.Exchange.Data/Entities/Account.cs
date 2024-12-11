namespace L2X.Exchange.Data.Entities;

/// <summary>
/// Tài khoản dành cho từng loại tiền của thành viên
/// </summary>
[Table("Account")]
[Index("OwnerId", "TickerId", IsUnique = true)]
public class Account : Entity<Guid>, IMomentable,IRemovable
{
    /// <summary>
    /// Mã Uid định danh Thành viên sở hữu tài khoản
    /// </summary>
    public Guid? OwnerId { get; set; }

    /// <summary>
    /// Mã Uid định dạnh Đơn vị tiền tệ của Tài khoản
    /// </summary>
    public Guid? TickerId { get; set; }

    /// <summary>
    /// Giá trị hiện tại trong tài khoản (giá trị tổng thể)
    /// </summary>
    public decimal? Balance { get; set; }

    /// <summary>
    /// Giá trị đang sử dụng (giá trị treo, sẽ bị trừ đi khi hoàn thành giao dịch.
    /// </summary>
    public decimal? LockAmount { get; set; }

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
    /// Thành viên sở hữu Tài khoản tương ứng
    /// </summary>
    [ForeignKey(nameof(OwnerId))]
    public Member? Owner { get; set; }

    /// <summary>
    /// Đơn vị tiền tệ mà Tài khoản sử dụng
    /// </summary>
    [ForeignKey(nameof(TickerId))]
    public Ticker? Ticker { get; set; }

    //public ICollection<Operator>? Operators { get; set; }

    //public ICollection<DepAddress>? Addresses { get; set; }
}