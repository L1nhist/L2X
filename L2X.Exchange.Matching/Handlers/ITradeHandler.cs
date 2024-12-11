namespace L2X.Exchange.Matching.Handlers;

public interface ITradeHandler
{
    Task OnAccept(OrderId orderId, string owner);

    Task OnCancel(OrderId orderId, string owner, decimal remainVolume, decimal cost, decimal fee, CancelReason cancelReason);

    Task OnDecrement(OrderId orderId, string owner, decimal decrementVolume);

    Task OnOrderTriggered(OrderId orderId, string owner);

    Task OnSelfMatch(OrderId orderId, OrderId matcherId, string owner);

    Task OnTrade(OrderId orderId, OrderId matchId, string orderOwner, string matchOwner, bool orderSide, decimal matchPrice, decimal matchVolume, decimal askRemainVolume, decimal askFee, decimal bidCost, decimal bidFee);
}