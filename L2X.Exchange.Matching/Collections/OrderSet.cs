namespace L2X.Exchange.Matching.Collections;

internal class OrderSet(IEnumerable<Order>? orders = null) : IEnumerable<Order>, IEnumerable
{
    private readonly HashSet<Order> _orders = orders == null ? [] : new(orders);

    #region Overridens
    public IEnumerator<Order> GetEnumerator()
        => _orders.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _orders.GetEnumerator();
    #endregion

    #region Methods
    public bool Add(Order order)
        => order != null && _orders.Add(order);

    public Order? Get(OrderId id)
    {
        foreach (var o in _orders)
        {
            if (o.Id == id) return o;
        }

        return null;
    }

    public bool Remove(Order order)
        => order != null && _orders.Remove(order);

    public bool TryGetOrder(OrderId id, out Order? order)
    {
        order = Get(id);
        return order != null;
    }
    #endregion
}