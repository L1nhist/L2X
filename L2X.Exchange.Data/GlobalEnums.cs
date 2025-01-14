namespace L2X.Exchange.Data;

public static class OrderCommonState
{
	public const string INVALID = "invalid";
	public const string CANCELLED = "cancelled";
    public const string FILLED = "filled";
    public const string MATCHED = "matched";
    public const string PLACED = "placed";
    public const string REJECTED = "rejected";
    public const string WAITING = "waiting";

    public static readonly string[] AllStates = [INVALID, PLACED, WAITING, CANCELLED, MATCHED, FILLED, REJECTED];

    public static bool Contains(string? value)
        => AllStates.Contains(value, StringComparer.OrdinalIgnoreCase);
}

public static class OrderCancellation
{
	public const string ACC_EMPTY = "invalid acc_empty";
	public const string MKT_EMPTY = "invalid mkt_empty";
	public const string NOT_ENOUGH = "invalid not_enough";
	public const string PRICE_FAIL = "invalid price_fail";
	public const string VOLUME_FAIL = "invalid volume_fail";
	public const string AMOUNT_FAIL = "invalid amount_fail";
	public const string LESS_STEP = "invalid mo_udr_step";

	public const string BY_IOC = "cancelled by_ioc";
	public const string BY_FOK = "cancelled by_fok";
	public const string BY_BOC = "cancelled by_boc";
	public const string BY_SEFL = "cancelled by_sm";
	public const string BY_EXPIRE = "expired by_val";
	public const string NO_LIQUID = "cancelled mo_no_liq";
	public const string USER_CLOSE = "cancelled by_owner";
}

public static class OrderType
{
    public const string LIMIT = "limit";
    public const string MARKET = "market";
    public const string STOP_LIMIT = "stop_lim";
    public const string STOP_MARKET = "stop_mkt";
    public const string ICEBERG = "iceberg";
    public const string GOOD_TILL_DATE = "gtd";
    public const string IMMEDIATE_OR_CANCEL = "ioc";
    public const string FILL_OR_KILL = "foc";

    public static readonly string[] AllTypes = [LIMIT, STOP_LIMIT, MARKET, STOP_MARKET, ICEBERG, GOOD_TILL_DATE, IMMEDIATE_OR_CANCEL, FILL_OR_KILL];
}

public static class RoleKeys
{
    public static readonly Uuid USER = new("vdhS8fiV@USPOuCsnAV8VQ");
    public static readonly Uuid ADMIN = new("q1mkcW8gQkuVZkYUtzg$tw");
    public static readonly Uuid MASTER = new("Jl2VURCoW0@xYcgOCp01Cw");

    public static bool IsManager(string? value)
        => ADMIN.Equals(value) || MASTER.Equals(value);

    public static bool HasRole(string? value)
        => USER.Equals(value) || IsManager(value);
}