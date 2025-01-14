namespace L2X.Exchange.Data.Requests;

public class AccountRequest
{
	/// <summary>
	/// Mã Uid định danh Thành viên sở hữu tài khoản
	/// </summary>
	public string Owner { get; set; }

	/// <summary>
	/// Mã Uid định dạnh Đơn vị tiền tệ của Tài khoản
	/// </summary>
	public string Ticker { get; set; }

	/// <summary>
	/// Giá trị hiện tại trong tài khoản (giá trị tổng thể)
	/// </summary>
	public decimal? Balance { get; set; }

	/// <summary>
	/// Giá trị đang sử dụng (giá trị treo, sẽ bị trừ đi khi hoàn thành giao dịch.
	/// </summary>
	public decimal? LockAmount { get; set; }
}