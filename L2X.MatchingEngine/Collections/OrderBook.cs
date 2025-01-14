namespace L2X.MatchingEngine.Collections;

public class OrderBook : IOrderBook<MOrder>
{
    #region Internal Properties
    private ulong _sequence = 0;

    private readonly GoodTillDate _goodTillDate = [];

    private readonly OrderSet _orders = [];

    private readonly Side<VolumeTrackingPriceLevel, MOrder> _asks = Side<VolumeTrackingPriceLevel, MOrder>.Ascending;

    private readonly Side<VolumeTrackingPriceLevel, MOrder> _bids = Side<VolumeTrackingPriceLevel, MOrder>.Descending;

    private readonly Side<PriceLevel, MOrder> _stopAsks = Side<PriceLevel, MOrder>.Descending;

    private readonly Side<PriceLevel, MOrder> _stopBids = Side<PriceLevel, MOrder>.Ascending;

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

    public IEnumerable<MOrder> Orders => _orders;

    public IEnumerable<IPriceLevel<MOrder>> Asks => _asks.PriceLevels;

    public IEnumerable<IPriceLevel<MOrder>> Bids => _bids.PriceLevels;

    public IEnumerable<IPriceLevel<MOrder>> StopAsks => _stopAsks.PriceLevels;

    public IEnumerable<IPriceLevel<MOrder>> StopBids => _stopBids.PriceLevels;

    public IEnumerable<KeyValuePair<long, HashSet<long>>> GoodTillDates => _goodTillDate;
    #endregion

    public IPriceLevel<MOrder>? GetPriceLevel(bool side, decimal price)
    {
        var lst = side ? _bids : _asks;
        return lst.GetLevel(price);
    }

    #region Methods
    internal bool CheckCanFillOrder(bool side, decimal volume, decimal limitPrice)
        => (side ? _bids : _asks).LimitOrderCanBeFilled(volume, limitPrice);

    internal bool CheckCanFillMarketOrderAmount(bool side, decimal amount)
        => (side ? _bids : _asks).MarketOrderCanBeFilled(amount);

    internal void DecrementVolume(MOrder order, decimal decrement)
        => (order.Side ? _bids : _asks).DecreaseOrder(order, decrement);

    internal MOrder? GetBestOrderToMatch(bool isBuy)
        => isBuy ? _bids.BestPriceLevel?.First : _asks.BestPriceLevel?.First;

    internal void AddStopOrder(MOrder order)
    {
        order.Sequence = ++_sequence;
        (order.Side ? _stopBids : _stopAsks).AddOrder(order, order.StopPrice ?? 0m);
        _orders.Add(order);
        _goodTillDate.Add(order);
    }

    internal void AddOpenOrder(MOrder order)
    {
        order.Sequence = ++_sequence;
        (order.Side ? _bids : _asks).AddOrder(order, order.Price ?? 0m);
        _orders.Add(order);
        _goodTillDate.Add(order);
    }

    internal void RemoveOrder(MOrder order)
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

    internal IReadOnlyList<IPriceLevel<MOrder>>? RemoveStopAsks(decimal price)
        => RemoveFromTracking(_stopAsks.RemoveTill(price));

    internal IReadOnlyList<IPriceLevel<MOrder>>? RemoveStopBids(decimal price)
        => RemoveFromTracking(_stopBids.RemoveTill(price));

    private IReadOnlyList<IPriceLevel<MOrder>>? RemoveFromTracking(IReadOnlyList<IPriceLevel<MOrder>>? priceLevels)
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

    internal bool TryGetOrder(long orderId, out MOrder? order)
        => _orders.TryGetOrder(orderId, out order);

    internal List<HashSet<long>>? GetExpiredOrders(long timeNow)
        => _goodTillDate.GetExpiredOrders(timeNow);

    internal bool FillOrder(MOrder order, decimal volume)
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