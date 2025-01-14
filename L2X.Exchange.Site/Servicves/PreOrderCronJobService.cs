using L2X.Core.Utilities;
using L2X.Data.Repositories;
using L2X.Exchange.Data;
using L2X.Exchange.Data.Services;
using L2X.Exchange.Enums;
using L2X.Services.Cronjobs;
using L2X.Services.Messages;
using L2X.Services.Models.Matching;

namespace L2X.Exchange.Site.Servicves;

/// <summary>
/// 
/// </summary>
public class PreOrderCronJobService(
    ILogger<PreOrderCronJobService> logger,
    IMapper mapper,
	IRepository<Account> accRepo,
	IRepository<Market> mktRepo,
	IRepository<Order> ordRepo,
	IRepository<PreOrder> preRepo,
	KafkaPublisherService<MOrder> publisher) : ICronJob
{
    private readonly ILogger<PreOrderCronJobService> _logger = logger;
    private readonly IMapper _mapper = mapper;
	private readonly IRepository<Account> _accRepo = accRepo;
	private readonly IRepository<Market> _mktRepo = mktRepo;
	private readonly IRepository<Order> _ordRepo = ordRepo;
	private readonly IRepository<PreOrder> _preRepo = preRepo;
    private readonly KafkaPublisherService<MOrder> _publisher = publisher;

    /// <inheritdoc/>
    public int Interval => 1000;

    /// <inheritdoc/>
    public int Delay => 1000;

    /// <inheritdoc/>
    public async Task DoWork(CancellationToken token)
    {
        try
        {
            var preOrds = await _preRepo.GetList(1000);
            if (preOrds == null) return;

            var mems = preOrds.Select(p => p.OwnerId).Distinct();
            var accs = await _accRepo.Query(a => a.OwnerId != null && mems.Contains(a.OwnerId.Value)).GetList();

            var syms = preOrds.Select(p => p.MarketId).Distinct();
            var mkts = await _mktRepo.Query(m => syms.Contains(m.Id)).JoinBy(m => m.BaseUnit).JoinBy(m => m.QuoteUnit).GetList();

            foreach (var po in preOrds)
            {
                if (string.IsNullOrEmpty(po.Symbol?.Trim())) continue;

                var ord = _mapper.Map<Order>(po);
                var acc = accs.FirstOrDefault(a => a.OwnerId == po.OwnerId);
                var mkt = mkts.FirstOrDefault(m => m.Id == po.MarketId);
                var amt = 0m;
                if (mkt != null)
				{
					if (ord.Type == OrderType.MARKET || ord.Type == OrderType.STOP_MARKET)
					{
						amt = (ord.Volume ?? 0m) * (mkt?.BaseUnit?.Price ?? 0m);
					}
					else
					{
						amt = (ord.Volume ?? 0m) * (ord.Price ?? 0m);
					}
				}

                if (mkt == null)
                {
                    ord.State = OrderCancellation.MKT_EMPTY;
                }
                else if (amt <= 0) {
                    ord.State = OrderCancellation.AMOUNT_FAIL;
                }
                else if (acc == null || acc.Balance <= amt)
                {
                    ord.State = OrderCancellation.ACC_EMPTY;
                }
                else if (ord.Price < 0m || (Util.IsEmpty(ord.Price) && ord.Type != OrderType.MARKET && ord.Type != OrderType.STOP_MARKET))
                {
                    ord.State = OrderCancellation.PRICE_FAIL;
                }
                else if (Util.IsEmpty(ord.Volume) || ord.Volume < 0m)
                {
                    ord.State = OrderCancellation.VOLUME_FAIL;
                }
                else if (!Util.IsEmpty(ord.Condition) && (ord.FinishedAt == 0 || ord.FinishedAt < Epoch.Now.Timestamp))
                {
                    ord.State = OrderCancellation.BY_EXPIRE;
                }
                else
                {
                    await _publisher.Publish(po.Symbol, _mapper.Map<MOrder>(po));

                    acc.Balance -= amt;
                    acc.LockAmount += amt;
                    await _accRepo.Update(acc);
                    await _ordRepo.Insert(ord);
                }
			}
        }
        catch (Exception ex)
        {
        }
    }
}
