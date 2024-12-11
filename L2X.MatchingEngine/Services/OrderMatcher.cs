using L2X.MatchingEngine.Handlers;
using L2X.MatchingEngine.Providers;

namespace L2X.MatchingEngine.Services;

public class OrderMatcher(ITradeHandler? tradeHandler = null, IFeeProvider? feeProvider = null, decimal stepSize = 0.00000001m, int pricePrecision = 8)
{
    #region Internal Properties
    private bool _orderTracking = true;

    private decimal _marketPrice;

    private readonly int _pricePrecision = pricePrecision >= 0 ? pricePrecision : throw MatchingException.New("Price Precision must be a integer number over or equal 0");

    private readonly decimal _stepSize = stepSize >= 0 ? stepSize : throw MatchingException.New("Step Size for Volume must be a decimal number over or equal 0");

    private readonly OrderBook _book = new();

    private readonly OrderIdTracker _trackers = new();

    private readonly Queue<IReadOnlyList<IPriceLevel<MOrder>>> _stopOrderQueue = new();

    private readonly IFeeProvider _feeProvider = feeProvider ?? new DefaultFeeProvider();

    private readonly ITradeHandler _tradeHandler = tradeHandler ?? new ConsoleTradeHandler();
    #endregion

    #region Overriden Properties
    public bool OrderTracking
    {
        get => _orderTracking;
        set
        {
            if (!value)
            {
                _trackers.Clear();
                _trackers.Compact();
            }

            _orderTracking = value;
        }
    }

    public IOrderBook<MOrder> Book => _book;

    public decimal MarketPrice => _marketPrice;

    public IEnumerable<MOrder> AllOrders => _book.Orders;

    public IEnumerable<OrderId> AcceptedOrders => _trackers;

    public IEnumerable<KeyValuePair<long, HashSet<OrderId>>> GoodTillDates => _book.GoodTillDates;
    #endregion

    #region Internal Methods
    private static MOrder GetTip(MOrder order)
    {
        if (order.Volume > 0) return order;

        var volume = order.TipVolume < order.TipAmount ? order.TipVolume : order.TipAmount;
        var remain = order.TipAmount - volume;
        return new MOrder
        {
            Id = order.Id,
            Owner = order.Owner,
            Side = order.Side,
            Price = order.Price,
            StopPrice = order.StopPrice,
            Volume = volume,
            CancelOn = order.CancelOn,
            TipVolume = order.TipVolume,
            TipAmount = remain,
            TotalCost = order.TotalCost,
            FeeCost = order.FeeCost,
            FeeId = order.FeeId,
            Condition = order.Condition,
            SelfMatch = order.SelfMatch,
        };
    }

    private async Task<bool> AddTip(MOrder order)
    {
        if (order.TipAmount > 0)
        {
            await MatchAndAddOrder(GetTip(order));
            return true;
        }
        return false;
    }

    private bool CanFillAmount(decimal amount, out decimal volume)
    {
        volume = 0;
        var remain = false;
        foreach (var level in _book.Asks)
        {
            foreach (var order in level)
            {
                if (amount == 0) return remain;

                var total = order.Volume * order.Price ?? 0m;
                if (total <= amount)
                {
                    volume = volume + order.Volume ?? 0m;
                    amount -= total;
                }
                else
                {
                    remain = true;
                    var q = amount / order.Price ?? 0m;
                    q = q - q % _stepSize;
                    if (q <= 0) return amount == 0;

                    volume = volume + q;
                    amount -= q * order.Price ?? 0m;
                }
            }
        }

        return amount == 0 || remain;
    }

    private async Task<MatchState> CancelOrder(OrderId orderId, CancelReason cancelReason)
    {
        if (!_book.TryGetOrder(orderId, out MOrder? order) || order == null) return MatchState.Order_Not_Exists;

        var volume = order.Volume + (order.IsTip ? order.TipAmount : 0) ?? 0m;
        _book.RemoveOrder(order);
        await _tradeHandler.OnCancel(orderId, order.Owner.ToString(), volume, order.TotalCost ?? 0m, order.FeeCost, cancelReason);
        return MatchState.Cancel_Acepted;
    }

    private async Task CancelIncomingOrder(MOrder order)
    {
        var volume = (order.IsTip ? order.Volume + order.TipAmount : order.Volume) ?? 0m;
        await _tradeHandler.OnCancel(order.Id, order.Owner.ToString(), volume, order.TotalCost ?? 0m, order.FeeCost, CancelReason.SelfMatch);
        order.Volume = 0;
        order.TipAmount = 0;
    }

    private async Task<bool> CancelByCondition(MOrder order, long timeStamp, bool canBeFilled)
    {
        if (order.Condition == OrderCondition.BookOrCancel && (order.Side && order.Price >= _book.BestAskPrice || !order.Side && order.Price <= _book.BestBidPrice))
        {
            await _tradeHandler.OnCancel(order.Id, order.Owner.ToString(), order.TipAmount + order.Volume ?? 0m, order.TotalCost ?? 0m, order.FeeCost, CancelReason.BookOrCancel);
            return true;
        }

        if (order.Condition == OrderCondition.FillOrKill && order.TipAmount == 0 && !_book.CheckCanFillOrder(order.Side, order.Volume ?? 0m, order.Price ?? 0m))
        {
            await _tradeHandler.OnCancel(order.Id, order.Owner.ToString(), order.Volume ?? 0m, order.TotalCost ?? 0m, order.FeeCost, CancelReason.FillOrKill);
            return true;
        }

        if (order.Condition == OrderCondition.FillOrKill && order.TipAmount > 0 && !canBeFilled)
        {
            await _tradeHandler.OnCancel(order.Id, order.Owner.ToString(), 0, 0, 0, CancelReason.FillOrKill);
            return true;
        }

        if (order.CancelOn > 0 && order.CancelOn <= timeStamp)
        {
            await _tradeHandler.OnCancel(order.Id, order.Owner.ToString(), order.TipAmount + order.Volume ?? 0m, order.TotalCost ?? 0m, order.FeeCost, CancelReason.ValidityExpired);
            return true;
        }

        return false;
    }

    private async Task DecrementVolume(MOrder order, decimal decrement, OrderBook? orderBook)
    {
        if (orderBook != null)
            orderBook.DecrementVolume(order, decrement);
        else
            order.Decrease(decrement);

        await _tradeHandler.OnDecrement(order.Id, order.Owner.ToString(), decrement);
    }

    private async Task<bool> SelfMatchDecrement(MOrder newOrder, MOrder matchOrder)
    {
        await _tradeHandler.OnSelfMatch(newOrder.Id, matchOrder.Id, newOrder.Owner.ToString());
        bool cancelNew = newOrder.SelfMatch == SelfMatchAction.Cancel_Newest;
        bool cancelOld = newOrder.SelfMatch == SelfMatchAction.Cancel_Oldest;
        if (newOrder.SelfMatch == SelfMatchAction.Decrement)
        {
            var incoming = (newOrder.IsTip ? newOrder.Volume + newOrder.TipAmount : newOrder.Volume) ?? 0m;
            var resting = (matchOrder.IsTip ? matchOrder.Volume + matchOrder.TipAmount : matchOrder.Volume) ?? 0m;
            if (incoming == resting)
            {
                await CancelOrder(matchOrder.Id, CancelReason.SelfMatch);
                await CancelIncomingOrder(newOrder);
                return true;
            }

            if (incoming > resting)
            {
                await DecrementVolume(newOrder, resting, null);
                cancelNew = true;
            }

            if (resting > incoming)
            {
                await DecrementVolume(matchOrder, incoming, _book);
                cancelOld = true;
            }
        }

        if (cancelNew)
        {
            await CancelIncomingOrder(newOrder);
            return true;
        }

        if (cancelOld)
        {
            await CancelOrder(matchOrder.Id, CancelReason.SelfMatch);
            return false;
        }

        return false;
    }

    private async Task MatchAndAddOrder(MOrder order, OrderCondition? condition = null)
    {
        decimal oldPrice = _marketPrice;
        await MatchWithOpenOrders(order);
        if (!order.IsFilled)
        {
            if (condition == OrderCondition.ImmediateOrCancel)
                await _tradeHandler.OnCancel(order.Id, order.Owner.ToString(), order.Volume ?? 0m, order.TotalCost ?? 0m, order.FeeCost, CancelReason.ImmediateOrCancel);
            else if (order.Price == 0)
                await _tradeHandler.OnCancel(order.Id, order.Owner.ToString(), order.Volume ?? 0m, order.TotalCost ?? 0m, order.FeeCost, CancelReason.NoLiquidity);
            else
                _book.AddOpenOrder(order);
        }
        else if (order.IsTip)
        {
            await AddTip(order);
        }

        if (_marketPrice > oldPrice)
        {
            var priceLevels = _book.RemoveStopBids(_marketPrice);
            if (priceLevels != null) _stopOrderQueue.Enqueue(priceLevels);
        }
        else if (_marketPrice < oldPrice)
        {
            var priceLevels = _book.RemoveStopAsks(_marketPrice);
            if (priceLevels != null) _stopOrderQueue.Enqueue(priceLevels);
        }
    }

    private async Task MatchAndAddTriggeredStopOrders()
    {
        while (_stopOrderQueue.TryDequeue(out var levels))
        {
            foreach (var level in levels)
            {
                foreach (var order in level)
                {
                    await _tradeHandler.OnOrderTriggered(order.Id, order.Owner.ToString());

                    if (order.Side && order.Volume == 0)
                    {
                        if (CanFillAmount(order.Amount ?? 0m, out var volume))
                        {
                            order.Volume = volume;
                            await MatchAndAddOrder(order);
                        }
                        else
                        {
                            await _tradeHandler.OnCancel(order.Id, order.Owner.ToString(), 0, 0, 0, CancelReason.NoLiquidity);
                        }
                    }
                    else
                    {
                        await MatchAndAddOrder(order);
                    }
                }
            }
        }
    }

    private async Task MatchWithOpenOrders(MOrder order)
    {
        while (true)
        {
            MOrder? matchOrder = _book.GetBestOrderToMatch(!order.Side);
            if (matchOrder == null) break;
            if (!(order.Side && (matchOrder.Price <= matchOrder.Price || order.Price == 0) || !order.Side && matchOrder.Price >= order.Price)) break;
            if (order.Owner == matchOrder.Owner && order.SelfMatch != SelfMatchAction.Match)
            {
                if (await SelfMatchDecrement(order, matchOrder)) continue;
                else break;
            }

            if (order.Volume <= 0) throw new MatchingException("Not expected");

            var matchPrice = matchOrder.Price ?? 0m;
            var maxVolume = (order.Volume >= matchOrder.Volume ? matchOrder.Volume : order.Volume) ?? 0m;
            order.Volume -= maxVolume;

            var totalCost = Math.Round(maxVolume * matchPrice, _pricePrecision);
            matchOrder.TotalCost += totalCost;
            order.TotalCost += totalCost;

            var takerFee = await _feeProvider.GetTakerFee(order.FeeId);
            var makerFee = await _feeProvider.GetMakerFee(matchOrder.FeeId);
            matchOrder.FeeCost += Math.Round(totalCost * makerFee / 100, _pricePrecision);
            order.FeeCost += Math.Round(totalCost * takerFee / 100, _pricePrecision);

            bool isTipAdded = _book.FillOrder(matchOrder, maxVolume) && matchOrder.IsTip && await AddTip(matchOrder);
            bool isIncomingFilled = order.IsTip ? order.TipAmount == 0 : order.IsFilled;
            bool isRestingFilled = matchOrder.IsFilled && !isTipAdded;

            decimal askRemain = 0;
            decimal askFee = 0;
            decimal bidCost = 0;
            decimal bidFee = 0;
            if (order.Side)
            {
                if (isIncomingFilled)
                {
                    bidCost = order.TotalCost ?? 0m;
                    bidFee = order.FeeCost;
                }
                if (isRestingFilled)
                {
                    askRemain = matchOrder.Volume ?? 0m;
                    askFee = matchOrder.FeeCost;
                }
            }
            else
            {
                if (isRestingFilled)
                {
                    bidCost = matchOrder.TotalCost ?? 0m;
                    bidFee = matchOrder.FeeCost;
                }
                if (isIncomingFilled)
                {
                    askRemain = order.Volume ?? 0m;
                    askFee = order.FeeCost;
                }
            }

            await _tradeHandler.OnTrade(order.Id, matchOrder.Id, order.Owner.ToString(), matchOrder.Owner.ToString(), order.Side, matchPrice, maxVolume, askRemain, askFee, bidCost, bidFee);
            _marketPrice = matchPrice;

            if (order.IsFilled) break;
        }
    }

    private MatchState ValidateOrder(MOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (order.Price < 0 || order.StopPrice < 0 || order.Volume < 0 || order.TipVolume < 0 || order.TipAmount < 0 || order.Amount < 0)
            return MatchState.Order_Invalid; //InvalidPriceQuantityStopPriceOrderAmountOrTotalQuantity;

        if (order.Price < 0 || order.Volume <= 0 && order.Amount == 0 && order.TipAmount == 0 || order.Volume == 0 && order.Amount <= 0 && order.TipAmount == 0 || order.StopPrice < 0 || order.TipAmount < 0)
            return MatchState.Order_Invalid; //InvalidPriceQuantityStopPriceOrderAmountOrTotalQuantity;

        switch (order.Condition)
        {
            case OrderCondition.BookOrCancel:
                if (order.Price == 0 || order.StopPrice != 0) return MatchState.BOC_Cannot_MOS; //BookOrCancelCannotBeMarketOrStopOrder;
                else break;

            case OrderCondition.ImmediateOrCancel:
                if (order.StopPrice != 0 || order.Price == 0) return MatchState.IOC_Cannot_MOS; //ImmediateOrCancelCannotBeMarketOrStopOrder;
                else break;

            case OrderCondition.FillOrKill:
                if (order.StopPrice != 0) return MatchState.Fok_Cannot_Stop_Order; //FillOrKillCannotBeStopOrder;
                else break;
        }

        if (order.Volume % _stepSize != 0 || order.TipAmount % _stepSize != 0 || order.TipVolume % _stepSize != 0)
            return MatchState.Not_Multiple_Of_Step_Size; //QuantityAndTotalQuantityShouldBeMultipleOfStepSize;

        if (order.CancelOn < 0)
            return MatchState.Invalid_Cancel_On_For_GTD; //InvalidCancelOnForGTD;

        if (order.CancelOn > 0 && (order.Price == 0 && !(order.Price == 0 && order.StopPrice != 0) || order.Condition == OrderCondition.FillOrKill || order.Condition == OrderCondition.ImmediateOrCancel))
            return MatchState.GTD_Cannot_Market_IOCFOK; //GoodTillDateCannotBeMarketOrIOCorFOK;

        if (order.Price == 0 && order.Amount != 0 && order.Volume != 0)
            return MatchState.MOO_Not_Both_Amount_Or_Volume; //MarketOrderOnlySupportedOrderAmountOrQuantityNoBoth;

        if (order.Amount != 0 && (order.Price != 0 || !order.Side))
            return MatchState.Market_Buy_Amount_Only; //OrderAmountOnlySupportedForMarketBuyOrder;

        if (order.TipAmount > 0)
        {
            if (order.Condition == OrderCondition.FillOrKill || order.Condition == OrderCondition.ImmediateOrCancel)
                return MatchState.Iceberg_Cannot_FOKIOC; //IcebergOrderCannotBeFOKorIOC;

            if (order.Price == 0 || order.StopPrice != 0 && order.Price == 0)
                return MatchState.Iceberg_Cannot_MOSM; //IcebergOrderCannotBeMarketOrStopMarketOrder;

            if (order.TipAmount <= order.TipVolume)
                return MatchState.Invalid_Iceberg_Volume; //InvalidIcebergOrderTotalQuantity;
        }

        if (_orderTracking)
        {
            if (_trackers.Contains(order.Id))
                return MatchState.Duplicate_Order;

            _trackers.TryMark(order.Id);
        }

        return MatchState.Order_Valid;
    }
    #endregion

    #region Overriden Methods
    public void InitializeMarketPrice(decimal marketPrice)
        => _marketPrice = marketPrice;

    public async Task<MatchState> AddOrder(MOrder order, long timeStamp, bool isOrderTriggered = false)
    {
        var state = ValidateOrder(order);
        if (state != MatchState.Order_Valid) return state;

        await _tradeHandler.OnAccept(order.Id, order.Owner.ToString());

        decimal volume = 0;
        bool canBeFilled = order.Side && order.Volume == 0 && order.StopPrice == 0 && order.Amount > 0 && CanFillAmount(order.Amount ?? 0m, out volume);

        await CancelExpiredOrders(timeStamp);
        if (!await CancelByCondition(order, timeStamp, canBeFilled))
        {
            if (order.TipAmount > 0)
                order = GetTip(order);

            if (order.StopPrice != 0 && !isOrderTriggered && (order.Side && order.StopPrice > _marketPrice || !order.Side && (order.StopPrice < _marketPrice || _marketPrice == 0)))
            {
                _book.AddStopOrder(order);
            }
            else if (order.Side && order.Volume == 0 && order.Amount > 0)
            {
                if (volume > 0)
                {
                    order.Volume = volume;
                    await MatchAndAddOrder(order, order.Condition);
                    await MatchAndAddTriggeredStopOrders();
                }
                else
                {
                    await _tradeHandler.OnCancel(order.Id, order.Owner.ToString(), 0, 0, order.FeeCost, CancelReason.NoLiquidity); //MarketOrderNoLiquidity);
                }
            }
            else
            {
                await MatchAndAddOrder(order, order.Condition);
                await MatchAndAddTriggeredStopOrders();
            }
        }

        return MatchState.Order_Accepted;
    }

    public async Task<MatchState> CancelOrder(OrderId orderId)
        => await CancelOrder(orderId, CancelReason.UserRequested);

    public async Task CancelExpiredOrders(long timeStamp)
    {
        var expires = _book.GetExpiredOrders(timeStamp);
        if (expires == null) return;

        foreach (var orders in expires)
            foreach (var orderId in orders)
                await CancelOrder(orderId, CancelReason.ValidityExpired);
    }
    #endregion
}