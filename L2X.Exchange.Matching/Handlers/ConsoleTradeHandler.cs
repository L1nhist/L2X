namespace L2X.Exchange.Matching.Handlers;

public class ConsoleTradeHandler : ITradeHandler
{
    #region Methods
    public async Task OnAccept(OrderId id, string owner)
        => Console.WriteLine($"Order {id} from user {owner} has been accepted!");

    public async Task OnCancel(OrderId id, string owner, decimal remainVolume, decimal cost, decimal fee, CancelReason reason)
        => Console.WriteLine($"Order {id} from user {owner} has been canceled by reason: {reason}!" + Environment.NewLine +
                            $"Order volume remained: {remainVolume}, total matched cost: {cost} with fee amount: {fee}!");

    public async Task OnDecrement(OrderId id, string owner, decimal decrementVolume)
        => Console.WriteLine($"Order {id} from user {owner} has been matched {decrementVolume}!");

    public async Task OnOrderTriggered(OrderId id, string owner)
        => Console.WriteLine($"Stop Order {id} from user {owner} has been triggered!");

    public async Task OnSelfMatch(OrderId orderId, OrderId matcherId, string owner)
        => Console.WriteLine($"Order {orderId} from user {owner} has been self matched with existed Order {matcherId}!");

    public async Task OnTrade(OrderId orderId, OrderId matchId, string orderOwner, string matchOwner, bool orderSide, decimal matchPrice, decimal matchVolume, decimal askRemain, decimal askFee, decimal bidCost, decimal bidFee)
        => Console.WriteLine((orderSide ? "Buy" : "Sell") + $" Order {orderId} from user {orderOwner} has been matched with existed Order {matchId} from user {matchOwner}!" + Environment.NewLine +
                            "Match result: " + Environment.NewLine +
                            $"Price: {matchPrice}; Volume: {matchVolume}; Remain: {askRemain}; Taker Cost: {bidCost}; Maker Fee: {askFee}; Taker Fee: {bidFee};");
    #endregion
}