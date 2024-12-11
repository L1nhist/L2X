using L2X.Exchange.Matching.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace L2X.Exchange.Matching.Collections;

public class PriceLevel : IPriceLevel<Order>, IEnumerable<Order>, IEnumerable
{
    #region Singleton Comparer
    private class OrderSequenceComparer : IComparer<Order>
    {
        public int Compare(Order? x, Order? y)
            => x == null ? -1 : y == null ? 1 : x.Timestamp.CompareTo(y.Timestamp);
    }

    private static readonly OrderSequenceComparer _comparer = new();
    #endregion

    #region Properties
    private readonly SortedSet<Order> _orders = new(_comparer);

    public int Count => _orders.Count;

    public decimal Price { get; private set; } = new();

    public decimal Volume => _orders.Sum(o => o.Volume ?? 0m);

    public Order? First => _orders.Min;
    #endregion

    internal bool TryGetOrder(Order order, [NotNullWhen(true)] out Order? result)
        => _orders.TryGetValue(order, out result);

    #region Overridens
    public IEnumerator<Order> GetEnumerator()
        => _orders.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _orders.GetEnumerator();

    public bool AddOrder(Order order)
        => _orders.Add(order);

    public bool RemoveOrder(Order order)
        => _orders.Remove(order);

    public void SetPrice(decimal price)
        => Price = _orders.Count < 1 ? price : throw new MatchedInvalidException(_orders.Count);

    public bool DecreaseOrder(Order order, decimal decrement)
        => TryGetOrder(order, out var update) && update.Decrease(decrement);

    public bool FillOrder(Order order, decimal volume)
    {
        order.Volume = order.Volume >= volume ? order.Volume - volume : throw new MatchedInvalidException("Volume");
        return order.IsFilled && _orders.Remove(order);
    }
    #endregion
}