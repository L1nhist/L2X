namespace L2X.Exchange.Data.Responses;

public class MemberResponse : IResponse
{
    /// <summary>
    /// Mã Uid độc lập để định danh cho thành viên
    /// </summary>
    public Uuid Id { get; set; }

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
    /// Ngày đăng nhập gần nhất dạng EpouchTimestamp (tính đến milliseconds)
    /// </summary>
    public Epoch LastLogin { get; set; }

    /// <summary>
    /// Ngày đăng ký dạng EpouchTimestamp (tính đến milliseconds)
    /// </summary>
    public Epoch CreatedAt { get; set; }

    /// <summary>
    /// Ngày chỉnh sửa cuối cùng dạng EpouchTimestamp (tính đến milliseconds)
    /// </summary>
    public Epoch ModifiedAt { get; set; }
}