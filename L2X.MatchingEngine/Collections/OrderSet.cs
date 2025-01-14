using L2X.Services.Models.Matching;

namespace L2X.MatchingEngine.Collections;

internal class OrderSet(IEnumerable<MOrder>? orders = null) : IEnumerable<MOrder>, IEnumerable
{
    private readonly HashSet<MOrder> _orders = orders == null ? [] : new(orders);

    #region Overridens
    public IEnumerator<MOrder> GetEnumerator()
        => _orders.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _orders.GetEnumerator();
    #endregion

    #region Methods
    public bool Add(MOrder order)
        => order != null && _orders.Add(order);

    public MOrder? Get(OrderId id)
    {
        foreach (var o in _orders)
        {
            if (o.Id == id) return o;
        }

        return null;
    }

    public bool Remove(MOrder order)
        => order != null && _orders.Remove(order);

    public bool TryGetOrder(OrderId id, out MOrder? order)
    {
        order = Get(id);
        return order != null;
    }
    #endregion
}