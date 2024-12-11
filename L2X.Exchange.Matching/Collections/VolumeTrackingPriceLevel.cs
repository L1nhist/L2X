using L2X.Exchange.Matching.Exceptions;

namespace L2X.Exchange.Matching.Collections;

public class VolumeTrackingPriceLevel : PriceLevel, IEnumerable<Order>, IEnumerable
{
    #region Overridens
    public new decimal Volume { get; private set; }

    public new bool AddOrder(Order order)
    {
        Volume += order.Volume ?? 0m;
        return AddOrder(order);
    }

    public new bool RemoveOrder(Order order)
    {
        Volume -= order.Volume ?? 0m;
        return RemoveOrder(order);
    }

    public new bool DecreaseOrder(Order order, decimal decrement)
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

    public new bool FillOrder(Order order, decimal volume)
    {
        if (order.Volume < volume) throw new MatchedInvalidException("Volume");

        Volume -= volume;
        order.Volume -= volume;
        return order.IsFilled && base.RemoveOrder(order);
    }
    #endregion
}