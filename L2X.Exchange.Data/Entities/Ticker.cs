namespace L2X.Exchange.Data.Entities;

/// <summary>
/// Đơn vị tiền tệ phục vụ giao dịch trên hệ thống
/// </summary>
[Table("Ticker")]
[Index("Code", IsUnique = true)]
public class Ticker : Entity<Guid>, IAuditable, ISortable, IRemovable
{
    /// <summary>
    /// Mã Uid định danh cho Đơn vị tiền tệ cấp cao hơn
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Mã Uid định danh cho blockchain của đơn vị tiền tệ này
    /// </summary>
    public Guid? ChainId { get; set; }

    /// <summary>
    /// Ký hiệu riêng của mỗi đơn vị tiền tệ
    /// </summary>
    [StringLength(30)]
    public string? Name { get; set; }

    /// <summary>
    /// Tên đầy đủ của Đơn vị tiền tệ
    /// </summary>
    [StringLength(120)]
    public string? Fullname { get; set; }

    /// <summary>
    /// Địa chỉ website của Đơn vị tiền tệ
    /// </summary>
    [StringLength(250)]
    public string? Site { get; set; }

    /// <summary>
    /// Đường dẫn đến hình ảnh đại diện của Đơn vị tiền tệ
    /// </summary>
    [StringLength(250)]
    public string? Logo { get; set; }

    /// <summary>
    /// Thông tin giới thiệu chi tiết về Đơn vị tiền tệ
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Dữ liệu meta cấu hình
    /// </summary>
    [StringLength(1600)]
    public string? Options { get; set; }

    /// <summary>
    /// Phân loại của Đơn vị tiền tệ: [coin] tiền số, [fiat] tiền thực
    /// </summary>
    /// <see cref="TickerType"/>
    [StringLength(20)]
    public string? Type { get; set; }

    /// <summary>
    /// Chỉ sử dụng nội bộ, không cho phép giao dịch
    /// </summary>
    /// <see cref="TickerMode"/>
    [StringLength(50)]
    public string? Usage { get; set; }

    /// <summary>
    /// Đơn vị cơ sở để tính toán, vd: 100000000
    /// </summary>
    public long? BaseFactor { get; set; }

    /// <summary>
    /// Độ chính xác sau dấu thập phân (chỉ dành cho đồng tiền số), vd: 8 --> độ chính xác là 8 số sau dấu thập phân
    /// </summary>
    public short? Precision { get; set; }

    /// <summary>
    /// Phân cấp của đơn vị tiền tệ này
    /// </summary>
    public short? SubUnits { get; set; }

    /// <summary>
    /// Giá hiện tại tính theo USD
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Số lượng tối thiểu để hệ thống thu thập
    /// If the deposit equal or higher than 'Min deposit amount' the system do the 'Min collection amount' check.
    /// If the deposit lower than 'Min collection amount' the system doesn't initialise a deposit collecting process but updates user balance.
    /// If the deposit equal or higher than 'Min collection amount' the system initialise a deposit collecting process to move funds from the deposit wallet to exchange wallet/wallets
    /// </summary>
    public decimal? MinCollect { get; set; }

    /// <summary>
    /// Số lượng nạp tối thiểu
    /// If the deposit lower than 'Min deposit amount' the system doesn't recognise that deposit and don't update user balance. 
    /// </summary>
    public decimal? MinDeposit { get; set; }

    /// <summary>
    /// Số lượng rút tối thiểu
    /// </summary>
    public decimal? MinWithdraw { get; set; }

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
    /// Blockchain tương ứng thuộc về đơn vị tiền tệ này
    /// </summary>
    //[ForeignKey(nameof(ChainId))]
    //public virtual Blockchain? Blockchain { get; set; }

    /// <summary>
    /// Danh sách các Đơn vị tiền tệ cấp cha
    /// </summary>
    [ForeignKey(nameof(ParentId))]
    public virtual Ticker? Parent { get; set; }

    /// <summary>
    /// Danh sách đơn vị tiền tệ cấp con
    /// </summary>
    public virtual ICollection<Ticker>? Children { get; set; }

    /// <summary>
    /// Danh sách các Kênh giao dịch
    /// </summary>
    public virtual IEnumerable<Symbol>? BaseSymbols { get; set; }

    /// <summary>
    /// Danh sách các Kênh giao dịch
    /// </summary>
    public virtual IEnumerable<Symbol>? QuoteSymbols { get; set; }

    /// <summary>
    /// Danh sách các tài khoản thuộc về đơn vị tiền tệ này
    /// </summary>
    public virtual ICollection<Account>? Accounts { get; set; }

    ///// <summary>
    ///// Danh sách các khoản nạp tiền sử dụng đơn vị tiền tề này
    ///// </summary>
    //public ICollection<Deposit>? Deposits { get; set; }

    ///// <summary>
    ///// Danh sách các khoản rút tiền sử dụng đơn vị tiền tệ này
    ///// </summary>
    //public ICollection<Withdraw>? Withdraws { get; set; }

    ///// <summary>
    ///// Danh sách các địa chỉ nạp tiền
    ///// </summary>
    //public ICollection<DepAddress>? DepAddrs { get; set; }

    ///// <summary>
    ///// Danh sách các khoản lợi nhuận sử dụng đơn vị tiền tệ này
    ///// </summary>
    //public virtual ICollection<WidAddress>? WidAddrs { get; set; }

    ///// <summary>
    ///// Danh sách các Hạn mức tiền tệ
    ///// </summary>
    //public ICollection<TickerRule>? Quotas { get; set; }

    ///// <summary>
    ///// Danh sách các ví sử dụng đơn vị tiền tệ này
    ///// </summary>
    //public virtual ICollection<Wallet>? Wallets { get; set; }
}