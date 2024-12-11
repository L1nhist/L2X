namespace L2X.Exchange.Matching.Collections;

public class OrderBook : IOrderBook<Order>
{
    #region Internal Properties
    private ulong _sequence = 0;

    private readonly GoodTillDate _goodTillDate = [];

    private readonly OrderSet _orders = [];

    private readonly Side<VolumeTrackingPriceLevel, Order> _asks = Side<VolumeTrackingPriceLevel, Order>.Ascending;

    private readonly Side<VolumeTrackingPriceLevel, Order> _bids = Side<VolumeTrackingPriceLevel, Order>.Descending;

    private readonly Side<PriceLevel, Order> _stopAsks = Side<PriceLevel, Order>.Descending;

    private readonly Side<PriceLevel, Order> _stopBids = Side<PriceLevel, Order>.Ascending;

    internal int AskPriceLevelCount => _asks.Count;

    internal int BidPriceLevelCount => _bids.Count;
    #endregion

    #region Public Properties
    public decimal? BestBidPrice => _bids.BestPriceLevel?.Price;

    public decimal? BestAskPrice => _asks.BestPriceLevel?.Price;

    public decimal? BestStopAskPrice => _stopAsks.BestPriceLevel?.Price;

    public decimal? BestStopBidPrice => _stopBids.BestPriceLevel?.Price;

    public decimal? BestBidVolume => _bids.BestPriceLevel?.Volume;

    public decimal? BestAskVolume => _asks.BestPriceLevel?.Volume;

    public IEnumerable<Order> Orders => _orders;

    public IEnumerable<IPriceLevel<Order>> Asks => _asks.PriceLevels;

    public IEnumerable<IPriceLevel<Order>> Bids => _bids.PriceLevels;

    public IEnumerable<IPriceLevel<Order>> StopAsks => _stopAsks.PriceLevels;

    public IEnumerable<IPriceLevel<Order>> StopBids => _stopBids.PriceLevels;

    public IEnumerable<KeyValuePair<long, HashSet<OrderId>>> GoodTillDates => _goodTillDate;
    #endregion

    #region Methods
    internal bool CheckCanFillOrder(bool side, decimal volume, decimal limitPrice)
        => (side ? _bids : _asks).LimitOrderCanBeFilled(volume, limitPrice);

    internal bool CheckCanFillMarketOrderAmount(bool side, decimal amount)
        => (side ? _bids : _asks).MarketOrderCanBeFilled(amount);

    internal void DecrementVolume(Order order, decimal decrement)
        => (order.Side ? _bids : _asks).DecreaseOrder(order, decrement);

    internal Order? GetBestOrderToMatch(bool isBuy)
        => isBuy ? _bids.BestPriceLevel?.First : _asks.BestPriceLevel?.First;

    internal void AddStopOrder(Order order)
    {
        order.Sequence = ++_sequence;
        (order.Side ? _stopBids : _stopAsks).AddOrder(order, order.StopPrice ?? 0m);
        _orders.Add(order);
        _goodTillDate.Add(order);
    }

    internal void AddOpenOrder(Order order)
    {
        order.Sequence = ++_sequence;
        (order.Side ? _bids : _asks).AddOrder(order, order.Price ?? 0m);
        _orders.Add(order);
        _goodTillDate.Add(order);
    }

    internal void RemoveOrder(Order order)
    {
        if (order.Side)
        {
            if (!_bids.RemoveOrder(order, order.Price ?? 0m) && order.IsStop)
                _stopBids.RemoveOrder(order, order.StopPrice ?? 0m);
        }
        else
        {
            if (!_asks.RemoveOrder(order, order.Price ?? 0m) && order.IsStop)
                _stopAsks.RemoveOrder(order, order.StopPrice ?? 0m);
        }

        _orders.Remove(order);
        _goodTillDate.Remove(order);
    }

    internal IReadOnlyList<IPriceLevel<Order>>? RemoveStopAsks(decimal price)
        => RemoveFromTracking(_stopAsks.RemoveTill(price));

    internal IReadOnlyList<IPriceLevel<Order>>? RemoveStopBids(decimal price)
        => RemoveFromTracking(_stopBids.RemoveTill(price));

    private IReadOnlyList<IPriceLevel<Order>>? RemoveFromTracking(IReadOnlyList<IPriceLevel<Order>>? priceLevels)
    {
        if (priceLevels == null) return null;

        foreach (var level in priceLevels)
        {
            foreach (var order in level)
            {
                _orders.Remove(order);
                _goodTillDate.Remove(order);
            }
        }

        return priceLevels;
    }

    internal bool TryGetOrder(OrderId orderId, out Order? order)
        => _orders.TryGetOrder(orderId, out order);

    internal List<HashSet<OrderId>>? GetExpiredOrders(long timeNow)
        => _goodTillDate.GetExpiredOrders(timeNow);

    internal bool FillOrder(Order order, decimal volume)
    {
        if ((order.Side ? _bids : _asks).FillOrder(order, volume))
        {
            _orders.Remove(order);
            _goodTillDate.Remove(order);
            return true;
        }

        return false;
    }
    #endregion
}