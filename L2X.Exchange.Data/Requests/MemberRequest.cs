namespace L2X.Exchange.Data.Requests;

public class MemberRequest : IRequest
{
    /// <summary>
    /// Mã uid liên kết với nhóm thành viên tương ứng
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// Mã uid liên kết với phân quyền tương ứng
    /// </summary>
    public string? Role { get; set; }

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
}