using L2X.Exchange.Data.Entities;
using L2X.Services.Models.Matching;
using Microsoft.Extensions.FileSystemGlobbing;

namespace L2X.MatchingEngine.Handlers;

public class ConsoleTradeHandler : ITradeHandler
{
    #region Methods
    public Task OnAccept(long id, string owner)
    {
        Console.WriteLine($"Order {id} from user {owner} has been accepted!");
        return Task.CompletedTask;
    }

    public Task OnCancel(long id, string owner, decimal remainVolume, decimal cost, decimal fee, CancelReason reason)
    {
        Console.WriteLine($"Order {id} from user {owner} has been canceled by reason: {reason}!" + Environment.NewLine +
                            $"Order volume remained: {remainVolume}, total matched cost: {cost} with fee amount: {fee}!");
        return Task.CompletedTask;
    }

    public Task OnDecrement(long id, string owner, decimal decrementVolume)
    {
        Console.WriteLine($"Order {id} from user {owner} has been matched {decrementVolume}!");
        return Task.CompletedTask;
    }

    public Task OnOrderTriggered(long id, string owner)
    {
        Console.WriteLine($"Stop Order {id} from user {owner} has been triggered!");
        return Task.CompletedTask;
    }

    public Task OnSelfMatch(long orderId, long matcherId, string owner)
    {
        Console.WriteLine($"Order {orderId} from user {owner} has been self matched with existed Order {matcherId}!");
        return Task.CompletedTask;
    }

    public Task OnTrade(long orderId, long matchId, string orderOwner, string matchOwner, bool orderSide, decimal matchPrice, decimal matchVolume, decimal askRemain, decimal askFee, decimal bidCost, decimal bidFee)
    {
        Console.WriteLine((orderSide ? "Buy" : "Sell") + $" Order {orderId} from user {orderOwner} has been matched with existed Order {matchId} from user {matchOwner}!" + Environment.NewLine +
                            "Match result: " + Environment.NewLine +
                            $"Price: {matchPrice}; Volume: {matchVolume}; Remain: {askRemain}; Taker Cost: {bidCost}; Maker Fee: {askFee}; Taker Fee: {bidFee};");
        return Task.CompletedTask;
    }

    public Task OnOrderBookChange(bool side, decimal price, decimal volume)
    {
        var sideChg = side ? "BUY" : "SELL";
        Console.WriteLine($"Order book of {sideChg} has changed at price {price} with volume {volume}!");
        return Task.CompletedTask;
    }
    #endregion
}