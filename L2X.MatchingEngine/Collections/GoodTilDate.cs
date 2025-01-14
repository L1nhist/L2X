using L2X.Services.Models.Matching;

namespace L2X.MatchingEngine.Collections;

internal class GoodTillDate : IEnumerable<KeyValuePair<long, HashSet<long>>>
{
    private long _minTime = long.MinValue;
    private readonly SortedDictionary<long, HashSet<long>> _orderSets = [];

    public void Add(MOrder order)
    {
        if (order.CancelOn > 0)
            Add(order.CancelOn, order.Id);
    }

    void Add(long time, long orderId)
    {
        if (!_orderSets.TryGetValue(time, out HashSet<long>? orderIds))
        {
            orderIds = [];
            _orderSets.Add(time, orderIds);

            if (time < _minTime)
                _minTime = time;
        }
        orderIds.Add(orderId);
    }

    public List<HashSet<long>>? GetExpiredOrders(long timeNow)
    {
        List<HashSet<long>>? expireds = null;
        if (_minTime <= timeNow)
        {
            expireds = [];
            foreach (var time in _orderSets)
            {
                if (time.Key > timeNow) break;

                expireds.Add(time.Value);
            }
        }
        return expireds;
    }

    public void Remove(MOrder order)
    {
        if (order.CancelOn > 0)
        {
            Remove(order.CancelOn, order.Id);
        }
    }

    void Remove(long time, long orderId)
    {
        if (_orderSets.TryGetValue(time, out var orderIds))
        {
            orderIds.Remove(orderId);
            if (orderIds.Count == 0)
            {
                _orderSets.Remove(time);
                if (time == _minTime)
                    _minTime = _orderSets.Count > 0 ? _orderSets.First().Key : long.MinValue;
            }
        }
    }

    #region Interface Implementation
    public IEnumerator<KeyValuePair<long, HashSet<long>>> GetEnumerator()
        => _orderSets.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _orderSets.GetEnumerator();
    #endregion
}