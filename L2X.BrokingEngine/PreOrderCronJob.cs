using L2X.Core.Extensions;
using L2X.Core.Structures;
using L2X.Exchange.Enums;
using L2X.Services.Cronjobs;
using L2X.Services.Messages;
using L2X.Services.Models.Matching;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace L2X.BrokingEngine;

/// <summary>
/// 
/// </summary>
public class PreOrderCronJob : ICronJob
{
    private readonly IConfiguration _config;
    private readonly ILogger<PreOrderCronJob> _logger;
    private readonly IRepository<Account> _accRepo;
    private readonly IRepository<Market> _mktRepo;
    private readonly IRepository<Order> _ordRepo;
    private readonly IRepository<PreOrder> _preRepo;
    private readonly KafkaPublisherService<MOrder> _publisher;

    private long _curr = 0;

    /// <inheritdoc/>
    public int Delay { get; }

    /// <inheritdoc/>
    public int Interval { get; }

	public int Counter { get; }

    public Market? Market { get; private set; }

    public PreOrderCronJob(
        IConfiguration configuration,
        ILoggerFactory logFactory,
        IRepository<Account> accRepo,
        IRepository<Market> mktRepo,
        IRepository<Order> ordRepo,
        IRepository<PreOrder> preRepo
    )
    {
		_config = configuration ?? throw new NullReferenceException("Configuration must defined");
		_logger = logFactory.CreateLogger<PreOrderCronJob>();
		_accRepo = accRepo;
		_mktRepo = mktRepo;
		_ordRepo = ordRepo;
		_preRepo = preRepo;
		_publisher = new KafkaPublisherService<MOrder>(_config, logFactory);

		Delay = Util.Convert<string, int>(configuration["OrderQueue:Delay"]);
		Interval = Util.Convert<string, int>(configuration["OrderQueue:Interval"]);
        Counter = Util.Convert<string, int>(configuration["OrderQueue:Concurrent"]);
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
		_curr = DateTime.Now.Year * 10000000000000 + DateTime.Now.Month * 100000000000 + DateTime.Now.Day * 1000000000 + DateTime.Now.Hour * 10000000 + DateTime.Now.Minute * 100000 + DateTime.Now.Second * 1000;
	}

	/// <inheritdoc/>
	public async Task DoWork(CancellationToken token)
    {
        if (token.IsCancellationRequested) return;

        var watch = Stopwatch.StartNew();
        if (Market == null)
			await Initialize();

        int cnt = 0;
		try
		{
			var preOrds = await _preRepo.SortBy(o => o.Id).GetList(Counter);
            if (preOrds == null) return;

            var mems = preOrds.Select(p => p.OwnerId).Distinct();
            var accs = await _accRepo.Query(a => a.OwnerId != null && mems.Contains(a.OwnerId.Value)).GetList();

            var syms = preOrds.Select(p => p.MarketId).Distinct();
            var mkts = await _mktRepo.Query(m => syms.Contains(m.Id)).GetList();

            var tm = watch.ElapsedMilliseconds;
            foreach (var po in preOrds)
            {
                try
                {
                    if (string.IsNullOrEmpty(po.Symbol?.Trim())) continue;

                    cnt++;
                    var ord = new Order
                    {
                        Id = Guid.NewGuid(),
                        OrderNo = po.Id,
                        OwnerId = po.OwnerId,
                        MarketId = po.MarketId,
                        Side = po.Side,
                        Symbol = po.Symbol,
                        Type = po.Type,
                        Condition = po.Condition,
                        Price = po.Price ?? 0,
                        Volume = po.Volume ?? 0,
                        Amount = po.Amount ?? 0,
                        StopPrice = po.StopPrice ?? 0,
                        Origin = po.Origin ?? 0,
                        CreatedAt = po.CreatedAt,
                        ExpiredAt = po.ExpiredAt,
                    };
                    var acc = accs.FirstOrDefault(a => a.OwnerId == po.OwnerId);
                    var amt = 0m;

                    if (ord.Type == OrderType.MARKET || ord.Type == OrderType.STOP_MARKET)
                    {
                        amt = (ord.Volume ?? 0m) * (Market?.BaseUnit?.Price ?? 0m);
                    }
                    else
                    {
                        amt = (ord.Volume ?? 0m) * (ord.Price ?? 0m);
                    }

                    if (Market.State == "close" || Market.State == "disable")
                    {
                        ord.State = OrderCancellation.MKT_EMPTY;
                        ord.FinishedAt = Epoch.Now.Timestamp;
                    }
                    else if (amt <= 0)
                    {
                        ord.State = OrderCancellation.AMOUNT_FAIL;
						ord.FinishedAt = Epoch.Now.Timestamp;
					}
                    else if (acc == null || acc.Balance <= amt)
                    {
                        ord.State = OrderCancellation.ACC_EMPTY;
						ord.FinishedAt = Epoch.Now.Timestamp;
					}
                    else if (ord.Price < 0m || Util.IsEmpty(ord.Price) && ord.Type != OrderType.MARKET && ord.Type != OrderType.STOP_MARKET)
                    {
                        ord.State = OrderCancellation.PRICE_FAIL;
						ord.FinishedAt = Epoch.Now.Timestamp;
					}
                    else if (Util.IsEmpty(ord.Volume) || ord.Volume < 0m)
                    {
                        ord.State = OrderCancellation.VOLUME_FAIL;
						ord.FinishedAt = Epoch.Now.Timestamp;
					}
                    else if (!Util.IsEmpty(ord.Condition) && (ord.FinishedAt == 0 || ord.FinishedAt < Epoch.Now.Timestamp))
                    {
                        ord.State = OrderCancellation.BY_EXPIRE;
						ord.FinishedAt = Epoch.Now.Timestamp;
					}
                    else
                    {
                        var mo = new MOrder()
                        {
                            Id = po.Id,
                            Amount = po.Amount,
                            Owner = po.OwnerId.ToString(),
                            Price = po.Price,
                            Side = po.Side,
                            StopPrice = po.StopPrice,
                            Timestamp = po.CreatedAt,
                            Volume = po.Volume,
                            TipVolume = po.Origin ?? 0,
                            TipAmount = 0,
                            CancelOn = po.ExpiredAt ?? 0,
                            Condition = po?.Condition?.Contains("foc") == true ? OrderCondition.FillOrKill : po?.Condition?.Contains("boc") == true ? OrderCondition.BookOrCancel : po?.Condition?.Contains("ioc") == true ? OrderCondition.ImmediateOrCancel : OrderCondition.None
                        };
                        await _publisher.Publish($"ORDER_{Market.Name}", mo);

                        ord.State = OrderCommonState.WAITING;
                        await _accRepo.Query(a => a.Id == acc.Id).UpdateBy(builder => builder.SetProperty(a => a.Balance, a => a.Balance - amt).SetProperty(a => a.LockAmount, a => a.LockAmount + amt));
                        //if (acc.Balance - amt >= 0)
                        //{
                        //    acc.Balance -= amt;
                        //    acc.LockAmount += amt;
                        //}
                    }

                    await _ordRepo.Insert(ord);
                }
                catch (Exception ex)
				{
					_logger.WriteLog($"Exception: {ex.Message}\r\n{ex.StackTrace}");
				}
			}

            await _preRepo.Delete(preOrds);
            //await _accRepo.Update(accs);
            
            if (cnt > 0)
			    _logger.WriteLog($"Execution {cnt} order(s) completed in {watch.ElapsedMilliseconds - tm} ms");
		}
		catch (Exception ex)
        {
            _logger.WriteLog($"Exception: {ex.Message}\r\n{ex.StackTrace}");
        }
    }
}