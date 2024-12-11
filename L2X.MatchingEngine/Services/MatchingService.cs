using L2X.MatchingEngine.Handlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace L2X.MatchingEngine.Services;

public class MatchingService : BackgroundService, ITradeHandler
{
    private readonly ILogger _logger;

    private readonly IServiceProvider _services;

    private readonly OrderMatcher _matcher;

    public int Current;
    public int LastIndex;
    public int TotalOrders => _matcher.AllOrders.Count();

    public int OrderInQueue => _matcher.Book.Orders.Count();

    //static int OrderFailed;
    //static int OrderMatched;

    public MatchingService(IServiceProvider services, ILoggerFactory logFactory)
    {
        _logger = logFactory.CreateLogger(GetType());
        _services = services;
        _matcher = new(this);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        // When the timer should have no due-time, then do the work once now.
        await DoWork();

        using PeriodicTimer timer = new(TimeSpan.FromSeconds(1));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DoWork();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");
        }
    }

    private Task DoWork()
    {
        //int i = LastIndex + Current;
        //if (i >= OrderPool.Count) break;

        //var order = OrderPool[i];
        //OrdIdList.Add(order.OrderId);
        //++OrderInQueue;
        //++Current;

        //var result = _engine.AddOrder(order, 1);
        //if (result != OrderMatchResult.OrderAccepted)
        //{
        //    OrdIdList.Remove(order.OrderId);
        //    ++OrderFailed;
        //}
        return Task.CompletedTask;
    }

    public Task OnAccept(OrderId orderId, string owner)
        => Task.CompletedTask;

    public Task OnCancel(OrderId orderId, string owner, decimal remainVolume, decimal cost, decimal fee, CancelReason cancelReason)
        => Task.CompletedTask;

    public Task OnDecrement(OrderId orderId, string owner, decimal decrementVolume)
        => Task.CompletedTask;

    public Task OnOrderTriggered(OrderId orderId, string owner)
        => Task.CompletedTask;

    public Task OnSelfMatch(OrderId orderId, OrderId matcherId, string owner)
    {
        //OrdIdList.Remove(orderId);
        //if (OrdIdList.Contains(orderId))
        //{

        //}
        return Task.CompletedTask;
    }

    public Task OnTrade(OrderId orderId, OrderId matchId, string orderOwner, string matchOwner, bool orderSide, decimal matchPrice, decimal matchVolume, decimal askRemainVolume, decimal askFee, decimal bidCost, decimal bidFee)
    {
        //var bidId = orderSide ? orderId : matchId;
        //var askId = orderSide ? matchId : orderId;
        //++OrderMatched;
        //--OrderInQueue;

        //if (askRemainVolume != null)
        //{
        //    OrdIdList.Remove(askId);
        //}

        //if (bidCost != null)
        //{
        //    OrdIdList.Remove(bidId);
        //}

        //if (matchPrice > maxPrice)
        //{
        //    maxPrice = matchPrice;
        //}
        //if (matchPrice < minPrice)
        //{
        //    minPrice = matchPrice;
        //}

        //sumPrice += matchPrice;
        return Task.CompletedTask;
    }
}