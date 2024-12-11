namespace L2X.Exchange.Data.Entities;

[Table("Member")]
[Index("Name", IsUnique = true)]
[Index("Email", IsUnique = true)]
[Index("Phone", IsUnique = true)]
public class Member : Entity<Guid>, IMomentable, IRemovable
{
    /// <summary>
    /// Mã uid liên kết với nhóm thành viên tương ứng
    /// </summary>
    public Guid? GroupId { get; set; }

    /// <summary>
    /// Mã uid liên kết với phân quyền tương ứng
    /// </summary>
    public Guid? RoleId { get; set; }

    /// <summary>
    /// Tên đăng nhập của thành viên
    /// </summary>
    [Required]
    [StringLength(50)]
    public required string Name { get; set; }

    /// <summary>
    /// Địa chỉ email của thành viên
    /// </summary>
    [Required]
    [StringLength(250)]
    public required string Email { get; set; }

    /// <summary>
    /// Số điện thoại liên lạc của thành viên
    /// </summary>
    [StringLength(250)]
    public required string Phone { get; set; }

    /// <summary>
    /// Mật khẩu đăng nhập của thành viên
    /// </summary>
    [Required]
    [StringLength(250)]
    public required string Password { get; set; }

    /// <summary>
    /// Mã xác nhận của thành viên, có thể xảy ra 2 trường hợp:
    /// Khi vừa đăng nhập, validrate = 0:
    ///  --> passcode là mã xác nhận địa chỉ email đã đăng ký
    /// Sau khi đã xác nhận email, validrate > 0:
    ///  --> passcode là mã xác nhận khi thành viên yêu cầu đổi mật khẩu
    /// </summary>
    [StringLength(10)]
    public string? Passcode { get; set; }

    /// <summary>
    /// Mã giới thiệu nhập khi đăng ký
    /// Đây là dãy số ngẫu nhiên 8 ký tự mà mỗi thành viên được cung cấp để giới thiệu thành viên mới
    /// </summary>
    [StringLength(8)]
    public string? Affiliate { get; set; }

    /// <summary>
    /// Mã uid thay đổi mỗi lần cập nhật thông tin của thành viên để đảm bảo dữ liệu cập nhật là duy nhất,
    /// tránh rủi ro cập nhật lại thông tin cũ
    /// </summary>
    public string? Secure { get; set; }

    /// <summary>
    /// Thứ hạng của thành viên, kết hợp với Nhóm thành viên để cung cấp các quyền lợi tương ứng
    /// Thứ hạng sẽ tăng dần theo quy định hoặc thiết lập từ hệ thống
    /// </summary>
    public byte? Level { get; set; }

    /// <summary>
    /// Thứ hạng của thành viên, kết hợp với Phân quyền thành viên để cung cấp các quyền lợi tương ứng
    /// </summary>
    public byte? Rank { get; set; }

    /// <summary>
    /// 0 mới đăng ký, 1 đã xác minh email, 2 đã xác minh số điện thoại, 3 hoàn thành hồ sơ cá nhân, 4 đã xác minh số ID, 5 đã xác minh địa chỉ
    /// </summary>
    public byte? ValidRate { get; set; }

    /// <summary>
    /// Tổng số lần nhập sai mật khẩu đăng nhập
    /// </summary>
    public byte? LogFailed { get; set; }

    /// <summary>
    /// Thời điểm kết thúc bị khóa do nhập sai mật khẩu quá số lần quy định
    /// Thời điểm tính theo dạng EpouchTimestamp (tính đến milliseconds)
    /// </summary>
    public long? LockedTo { get; set; }

    /// <summary>
    /// Ngày đăng nhập gần nhất dạng EpouchTimestamp (tính đến milliseconds)
    /// </summary>
    public long? LastLogin { get; set; }

    /// <summary>
    /// Ngày đăng ký dạng EpouchTimestamp (tính đến milliseconds)
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
    /// Nhóm người dùng tương ứng
    /// </summary>
    //[ForeignKey(nameof(GroupId))]
    //public virtual Group? Group { get; set; }

    ///// <summary>
    ///// Phân quyền tương ứng
    ///// </summary>
    //[ForeignKey(nameof(RoleId))]
    //public virtual Role? Role { get; set; }

    /// <summary>
    /// Danh sách các tài khoản thuộc về loại tiền tệ này
    /// </summary>
    public virtual ICollection<Account>? Accounts { get; set; }

    ///// <summary>
    ///// Danh sách các khoản lợi nhuận sử dụng loại tiền tệ này
    ///// </summary>
    //public virtual ICollection<WidAddress>? Addresses { get; set; }

    ///// <summary>
    ///// Danh sách các khoản nạp tiền sử dụng loại tiền tề này
    ///// </summary>
    //public ICollection<Deposit>? Deposits { get; set; }

    ///// <summary>
    ///// Danh sách các khoản rút tiền sử dụng loại tiền tệ này
    ///// </summary>
    //public ICollection<Withdraw>? Withdraws { get; set; }

    /// <summary>
    /// Danh sách lệnh đặt tương ứng
    /// </summary>
    public ICollection<Order>? Orders { get; set; }

    /// <summary>
    /// Danh sách lệnh đặt tương ứng
    /// </summary>
    //public ICollection<Matcher>? MakerMatchers { get; set; }

    ///// <summary>
    ///// Danh sách lệnh đặt tương ứng
    ///// </summary>
    //public ICollection<Matcher>? TakerMatchers { get; set; }

    ///// <summary>
    ///// 
    ///// </summary>
    //public ICollection<Operator>? Operators { get; set; }

    ///// <summary>
    ///// Danh sách các ví sử dụng loại tiền tệ này
    ///// </summary>
    //public virtual ICollection<Wallet>? Wallets { get; set; }
}