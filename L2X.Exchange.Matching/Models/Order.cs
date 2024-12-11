namespace L2X.Exchange.Matching.Models;

public class Order : IOrder
{
    #region Properties
    public OrderId Id { get; set; }

    string IOrder.Id
    {
        get => Id.ToString();
        set => Id = new(value);
    }

    public string Owner { get; set; } = "";

    public bool Side { get; set; }

    public OrderCondition Condition { get; set; }

    public SelfMatchAction SelfMatch { get; set; }

    public decimal? Price { get; set; }

    public decimal? StopPrice { get; set; }

    public decimal? Volume { get; set; }

    public decimal? Amount { get; set; }

    public decimal? TipVolume { get; set; }

    public decimal? TipAmount { get; set; }

    public decimal? TotalCost { get; set; }

    public int FeeId { get; set; }

    public decimal FeeCost { get; set; }

    public long Timestamp { get; set; }

    public long CancelOn { get; set; }

    public ulong Sequence { get; set; }

    public bool IsFilled => Volume == 0;

    public bool IsStop => StopPrice > 0;

    public bool IsTip => TipVolume > 0 && TipAmount > 0;
    #endregion

    #region Overriden
    string IOrder.Owner
    {
        get => Owner.ToString();
        set => Owner = new(value);
    }

    public override bool Equals(object? obj)
        => obj is Order order ? Id == order.Id : base.Equals(obj);

    public override int GetHashCode()
        => Id.GetHashCode();
    #endregion

    public bool Decrease(decimal volume)
    {
        var total = IsTip ? Volume + TipAmount : Volume;
        if (total <= volume || volume <= 0) return false;

        if (IsTip)
        {
            var t = Math.Max(volume, TipAmount!.Value);
            TipAmount -= t;
            volume -= t;
        }

        Volume -= volume;
        return true;
    }
}