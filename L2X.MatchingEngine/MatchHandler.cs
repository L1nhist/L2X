using AutoMapper;
using L2X.Core.Caching;
using L2X.Core.Extensions;
using L2X.Core.Structures;
using L2X.Core.Utilities;
using L2X.Data.Repositories;
using L2X.Exchange.Data;
using L2X.Exchange.Data.Entities;
using L2X.MatchingEngine.Handlers;
using L2X.Services.Caching;
using L2X.Services.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace L2X.MatchingEngine;

public class MatchHandler : ITradeHandler, IConsumeHandler<MOrder>
{
    private readonly IConfiguration _config;
    private readonly ILogger _logger;
	private readonly IRepository<Market> _mktRepo;
	private readonly IRepository<Match> _mchRepo;
	private readonly IRepository<Order> _ordRepo;
	private readonly KafkaPublisherService<MOrder> _publisher;
	private readonly KafkaConsumerService<MOrder> _consumer;
    private readonly RedisCacheService _redisCache;
    private readonly MatchEngine _matcher;
    private readonly MarketTracker _mktTracker;

	/// <inheritdoc/>
	public int Interval { get; }

	public Market? Market { get; private set; }

	public int Current;
    public int LastIndex;
    public int TotalOrders => _matcher.AllOrders.Count();

    public int OrderInQueue => _matcher.Book.Orders.Count();

    public MatchHandler(
        IConfiguration configuration,
        ILoggerFactory logFactory,
	    IRepository<Market> mktRepo,
		IRepository<Match> mchRepo,
		IRepository<Order> ordRepo
    )
	{
		_config = configuration;
        _logger = logFactory.CreateLogger<MatchHandler>();
        _mktRepo = mktRepo;
        _mchRepo = mchRepo;
        _ordRepo = ordRepo;
        _publisher = new(_config, logFactory);
        _consumer = new(_config, logFactory, this);
        _matcher = new(this);
        _mktTracker = new(100);
        _redisCache = new(_config);

		Interval = Util.Convert<string, int>(configuration["Kafka:Interval"]);
	}

	public async Task Initialize()
	{
		var mkt = _config["Kafka:MarketSymbol"];
		if (Util.IsEmpty(mkt)) throw new NullReferenceException("Market configuration must defined");
		var uid = new Uuid(mkt);
		if (uid.IsEmpty)
		{
			_mktRepo.Query(m => m.Name == mkt);
		}
		else
		{
			_mktRepo.Query(m => m.Id == (Guid)uid);
		}

		Market = await _mktRepo.JoinBy(m => m.BaseUnit).JoinBy(m => m.QuoteUnit).GetFirst() ?? throw new NullReferenceException("Market data could not found in db");

        await _consumer.Subscribe($"ORDER_{Market.Name}", Interval);
        _matcher.InitializeMarketPrice(Market?.BaseUnit?.Price / Market?.QuoteUnit?.Price ?? 0);
	}

	public async Task Consume(MOrder? order)
    {
        if (_matcher == null || order == null) return;

        try
        {
            order.Amount ??= 0;
            order.TipAmount ??= 0;
			order.TipVolume ??= 0;
			var result = await _matcher.AddOrder(order, Epoch.Now.Timestamp);
            var state = result switch
                {
                    MatchState.Order_Valid or MatchState.Order_Accepted => OrderCommonState.WAITING,
                    MatchState.Duplicate_Order => "invalid dublicated",
                    MatchState.Order_Invalid or MatchState.Order_Not_Exists => OrderCommonState.INVALID,
                    MatchState.Cancel_Acepted => OrderCancellation.USER_CLOSE,
                    _ => OrderCommonState.REJECTED,
                };

			await _ordRepo.Query(o => o.OrderNo == order.Id)
                            .UpdateBy(b => b.SetProperty(o => o.FinishedAt, Epoch.Now.Timestamp).SetProperty(o => o.State, state));

            _logger.WriteLog($"Match order: {order.Id} with status {result}");
        }
        catch (Exception ex)
        {
            _logger.WriteLog(ex);
        }
    }

    public Task OnAccept(long orderId, string owner)
        => Task.CompletedTask;

    public async Task OnCancel(long orderId, string owner, decimal remainVolume, decimal cost, decimal fee, CancelReason reason)
	{
        var state = reason switch
		{
			CancelReason.BookOrCancel => OrderCancellation.BY_BOC,
			CancelReason.ImmediateOrCancel => OrderCancellation.BY_IOC,
			CancelReason.FillOrKill => OrderCancellation.BY_FOK,
			CancelReason.UserRequested => OrderCancellation.USER_CLOSE,
			CancelReason.SelfMatch => OrderCancellation.BY_SEFL,
			CancelReason.LessThanStepSize => OrderCancellation.LESS_STEP,
			CancelReason.NoLiquidity => OrderCancellation.NO_LIQUID,
			_ => OrderCommonState.INVALID,
		};

		await _ordRepo.Query(o => o.OrderNo == orderId)
                        .UpdateBy(b => b.SetProperty(o => o.FinishedAt, Epoch.Now.Timestamp).SetProperty(o => o.State, state));
	}

    public Task OnDecrement(long orderId, string owner, decimal decrementVolume)
        => Task.CompletedTask;

    public Task OnOrderTriggered(long orderId, string owner)
        => Task.CompletedTask;

    public Task OnSelfMatch(long orderId, long matcherId, string owner)
    {
        //OrdIdList.Remove(orderId);
        //if (OrdIdList.Contains(orderId))
        //{

        //}
        return Task.CompletedTask;
    }

    public async Task OnTrade(long orderId, long matchId, string orderOwner, string matchOwner, bool orderSide, decimal matchPrice, decimal matchVolume, decimal askRemainVolume, decimal askFee, decimal bidCost, decimal bidFee)
    {
        _mktTracker.SetTradeInfo(matchPrice, matchVolume);
        var inf = new MarketInfo
        {
            LastPrice = _mktTracker.PriceLast,
            MinPrice = _mktTracker.PriceMin,
            MaxPrice = _mktTracker.PriceMax,
            PriceChg = _mktTracker.PriceChg,
            PriceRate = _mktTracker.PriceRate,
            TotalAsks = _mktTracker.TotalAsks,
            TotalBids = _mktTracker.TotalBids,
        };
        await _redisCache.Set("MKT_INFO", inf, 30000);

        var ords = await _ordRepo.Query(o => o.OrderNo == orderId || o.OrderNo == matchId).GetList();
        if (ords.Count != 2) return;

        var mo = ords[0];
        var to = ords[1];
        if (ords[0].OrderNo == matchId)
        {
            mo = ords[1];
            to = ords[0];
        }

        var mch = new Match()
        {
            MkrOrdId = mo.Id,
            TkrOrdId = to.Id,
            MakerId = mo.OwnerId,
            TakerId = to.OwnerId,
            MarketId = mo.MarketId,
            Price = matchPrice,
            Volume = matchVolume,
            TakerType = orderSide,
            MakerFee = askFee,
            TakerFee = bidFee,
            Amount = askRemainVolume,
            IsDeleted = false
        };

        await _mchRepo.Insert(mch);
	}

	public async Task OnOrderBookChange(bool side, decimal price, decimal volume)
    {
        if (_mktTracker.SetOrderBook(side, price, volume))
        {
            var key = side ? "BID_OB" : "ASK_OB";
            await _redisCache.Set(key, side ? _mktTracker.Bids : _mktTracker.Asks, 30000);
        }
    }
}