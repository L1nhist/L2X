using L2X.Services.Models.Matching;

namespace L2X.MatchingEngine.Handlers;

public interface ITradeHandler
{
    Task OnAccept(long orderId, string owner);

    Task OnCancel(long orderId, string owner, decimal remainVolume, decimal cost, decimal fee, CancelReason cancelReason);

    Task OnDecrement(long orderId, string owner, decimal decrementVolume);

    Task OnOrderTriggered(long orderId, string owner);

    Task OnSelfMatch(long orderId, long matcherId, string owner);

    Task OnTrade(long orderId, long matchId, string orderOwner, string matchOwner, bool orderSide, decimal matchPrice, decimal matchVolume, decimal askRemainVolume, decimal askFee, decimal bidCost, decimal bidFee);
	
    Task OnOrderBookChange(bool side, decimal price, decimal volume);
}