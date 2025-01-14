using L2X.Services.Models.Matching;
using System.Diagnostics.CodeAnalysis;

namespace L2X.MatchingEngine.Collections;

public class PriceLevel : IPriceLevel<MOrder>, IEnumerable<MOrder>, IEnumerable
{
    #region Singleton Comparer
    private class OrderSequenceComparer : IComparer<MOrder>
    {
        public int Compare(MOrder? x, MOrder? y)
            => x == null ? -1 : y == null ? 1 : x.Timestamp.CompareTo(y.Timestamp);
    }

    private static readonly OrderSequenceComparer _comparer = new();
    #endregion

    #region Properties
    private readonly SortedSet<MOrder> _orders = new(_comparer);

    public int Count => _orders.Count;

    public decimal Price { get; private set; } = new();

    public decimal Volume => _orders.Sum(o => o.Volume ?? 0m);

    public MOrder? First => _orders.Min;
    #endregion

    internal bool TryGetOrder(MOrder order, [NotNullWhen(true)] out MOrder? result)
        => _orders.TryGetValue(order, out result);

    #region Overridens
    public IEnumerator<MOrder> GetEnumerator()
        => _orders.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _orders.GetEnumerator();

    public bool AddOrder(MOrder order)
        => _orders.Add(order);

    public bool RemoveOrder(MOrder order)
        => _orders.Remove(order);

    public void SetPrice(decimal price)
        => Price = _orders.Count < 1 ? price : throw MatchingException.SetPriceLevelFail(_orders.Count);

    public bool DecreaseOrder(MOrder order, decimal decrement)
        => TryGetOrder(order, out var update) && update.Decrease(decrement);

    public bool FillOrder(MOrder order, decimal volume)
    {
        order.Volume = order.Volume >= volume ? order.Volume - volume : throw MatchingException.New("MOrder volume is less then requested fill volume.");
        return order.IsFilled && _orders.Remove(order);
    }
    #endregion
}