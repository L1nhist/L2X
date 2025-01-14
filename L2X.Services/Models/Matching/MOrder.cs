namespace L2X.Services.Models.Matching;

[ProtoContract]
public class MOrder : IOrder
{
    #region Properties
    [ProtoMember(1)]
    public long Id { get; set; }

    [ProtoMember(2)]
    public string Owner { get; set; } = "";

    [ProtoMember(3)]
    public bool Side { get; set; }

    [ProtoMember(4)]
    public OrderCondition Condition { get; set; }

    [ProtoMember(5)]
    public SelfMatchAction SelfMatch { get; set; }

    [ProtoMember(6)]
    public decimal? Price { get; set; }

    [ProtoMember(7)]
    public decimal? StopPrice { get; set; }

    [ProtoMember(8)]
    public decimal? Volume { get; set; }

    [ProtoMember(9)]
    public decimal? Amount { get; set; }

    [ProtoMember(10)]
    public decimal? TipVolume { get; set; }

    [ProtoMember(11)]
    public decimal? TipAmount { get; set; }

    [ProtoMember(12)]
    public decimal? TotalCost { get; set; }

    [ProtoMember(13)]
    public int FeeId { get; set; }

    [ProtoMember(14)]
    public decimal FeeCost { get; set; }

    [ProtoMember(15)]
    public long Timestamp { get; set; }

    [ProtoMember(16)]
    public long CancelOn { get; set; }

    [ProtoMember(17)]
    public ulong Sequence { get; set; }

    public bool IsFilled => Volume == 0;

    public bool IsStop => StopPrice > 0;

    public bool IsTip => TipVolume > 0 && TipAmount > 0;
    #endregion

    #region Overriden
    string IOrder.Id
    {
        get => Id.ToString();
        set => Id = long.TryParse(value, out long result) ? result : 0;
    }

    public override bool Equals(object? obj)
        => obj is MOrder order ? Id == order.Id : base.Equals(obj);

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