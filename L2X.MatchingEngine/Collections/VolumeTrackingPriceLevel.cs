using L2X.Services.Models.Matching;

namespace L2X.MatchingEngine.Collections;

public class VolumeTrackingPriceLevel : PriceLevel, IEnumerable<MOrder>, IEnumerable
{
    #region Overridens
    public new decimal Volume { get; private set; }

    public new bool AddOrder(MOrder order)
    {
        Volume += order.Volume ?? 0m;
        return AddOrder(order);
    }

    public new bool RemoveOrder(MOrder order)
    {
        Volume -= order.Volume ?? 0m;
        return RemoveOrder(order);
    }

    public new bool DecreaseOrder(MOrder order, decimal decrement)
    {
        if (TryGetOrder(order, out var update))
        {
            var oldVolume = update.Volume;
            if (update.Decrease(decrement))
            {
                Volume += (update.Volume - oldVolume) ?? 0m;
                return true;
            }
        }

        return false;
    }

    public new bool FillOrder(MOrder order, decimal volume)
    {
        if (order.Volume < volume) throw MatchingException.New("MOrder volume is less then requested fill volume.");

        Volume -= volume;
        order.Volume -= volume;
		return order.IsFilled && base.RemoveOrder(order);
    }
    #endregion
}