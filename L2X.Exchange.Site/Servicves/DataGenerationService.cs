using L2X.Core.Utilities;
using L2X.Data.Repositories;
using L2X.Exchange.Data;

namespace L2X.Exchange.Site.Servicves;

public class DataGenerationService(
    IRepository<Account> accRepo,
    IRepository<Market> mktRepo,
    IRepository<Member> memRepo,
    IRepository<PreOrder> proRepo,
    IRepository<Ticker> tkrRepo
)
{
    private readonly IRepository<Account> _accRepo = accRepo;
    private readonly IRepository<Market> _mktRepo = mktRepo;
    private readonly IRepository<Member> _memRepo = memRepo;
    private readonly IRepository<PreOrder> _proRepo = proRepo;
    private readonly IRepository<Ticker> _tkrRepo = tkrRepo;

	public async Task<IResult<int>> NewAccount()
	{
		try
		{
			var lst = new List<Account>();
			var mems = await _memRepo.GetList();
			var tiks = await _tkrRepo.GetList();

			foreach (var m in mems)
			{
				foreach (var t in tiks)
				{
					lst.Add(new()
					{
						OwnerId = m.Id,
						TickerId = t.Id,
						Balance = 1000000,
						LockAmount = 0,
						CreatedAt = Epoch.Now.Timestamp,
						IsDeleted = false,
					});
				}
			}

			return Result.New<int>().Ok(await _accRepo.Insert(lst));
		}
		catch (Exception ex)
		{
			return Result.Error<int>(ex);
		}
	}

	public async Task<IResult<int>> NewMarket()
	{
		try
		{
			return Result.New<int>().Ok(await _mktRepo.Insert(new Market
			{
				Name = "BTC-USDT",
				Fullname = "Bitcoin vs USDT",
				VolumePrecision = 8,
				PricePrecision = 8,
				MinVolumn = 0.001m,
				MinPrice = 0.1m,
				MaxPrice = 0,
				Type = "spot",
				State = "open",
				Position = 1,
				Creator = "master@l2x.xyz",
				CreatedAt = Epoch.Now.Timestamp,
				IsDeleted = false,
				BaseUnit = new()
				{
					Name = "BTC",
					Fullname = "Bitcoin",
					Type = "coin",
					Usage = "all",
					BaseFactor = 8,
					Precision = 8,
					SubUnits = 3,
					Price = 105017.4m,
					MinCollect = 0,
					MinDeposit = 1,
					MinWithdraw = 1,
					Position = 1,
					Creator = "master@l2x.xyz",
					CreatedAt = Epoch.Now.Timestamp,
					IsDeleted = false,
				},
				QuoteUnit = new()
				{
					Name = "USDT",
					Fullname = "USD Tether",
					Type = "coin",
					Usage = "all",
					BaseFactor = 8,
					Precision = 8,
					SubUnits = 3,
					Price = 1m,
					MinCollect = 0,
					MinDeposit = 1,
					MinWithdraw = 1,
					Position = 1,
					Creator = "master@l2x.xyz",
					CreatedAt = Epoch.Now.Timestamp,
					IsDeleted = false,
				},
			}));
		}
		catch (Exception ex)
		{
			return Result.Error<int>(ex);
		}
	}

	public async Task<IResult<int>> NewMember()
	{
		try
		{
			var lst = new List<Member>()
				{
					new()
					{
						RoleId = new Uuid(RoleKeys.MASTER),
						Name = "System Master",
						Email = "master@l2x.xyz",
						Phone = "0987654321",
						Password = "M4st3r",
						Secure = Uuid.New.ToString(),
						ValidRate = 5,
						CreatedAt = Epoch.Now.Timestamp,
						IsDeleted = false,
					},
					new()
					{
						RoleId = new Uuid(RoleKeys.ADMIN),
						Name = "Administrator",
						Email = "admin@l2x.xyz",
						Phone = "1234567890",
						Password = "Adm1n",
						Secure = Uuid.New.ToString(),
						ValidRate = 5,
						CreatedAt = Epoch.Now.Timestamp,
						IsDeleted = false,
					},
					new()
					{
						RoleId = new Uuid(RoleKeys.USER),
						Name = "Nguyen AB",
						Email = "ab@abc.com",
						Phone = "09865431",
						Password = "123456",
						Secure = Uuid.New.ToString(),
						ValidRate = 5,
						CreatedAt = Epoch.Now.Timestamp,
						IsDeleted = false,
					},
					new()
					{
						RoleId = new Uuid(RoleKeys.USER),
						Name = "Tran CD",
						Email = "cd@abc.com",
						Phone = "098532145",
						Password = "123456",
						Secure = Uuid.New.ToString(),
						ValidRate = 5,
						CreatedAt = Epoch.Now.Timestamp,
						IsDeleted = false,
					},
					new()
					{
						RoleId = new Uuid(RoleKeys.USER),
						Name = "Le GH",
						Email = "gh@abc.com",
						Phone = "099978842",
						Password = "123456",
						Secure = Uuid.New.ToString(),
						ValidRate = 5,
						CreatedAt = Epoch.Now.Timestamp,
						IsDeleted = false,
					},
					new()
					{
						RoleId = new Uuid(RoleKeys.USER),
						Name = "Bui UI",
						Email = "ui@abc.com",
						Phone = "0874633245",
						Password = "123456",
						Secure = Uuid.New.ToString(),
						ValidRate = 5,
						CreatedAt = Epoch.Now.Timestamp,
						IsDeleted = false,
					},
					new()
					{
						RoleId = new Uuid(RoleKeys.USER),
						Name = "Dang PQ",
						Email = "pq@abc.com",
						Phone = "095362423",
						Password = "123456",
						Secure = Uuid.New.ToString(),
						ValidRate = 5,
						CreatedAt = Epoch.Now.Timestamp,
						IsDeleted = false,
					},
				};

			return Result.New<int>().Ok(await _memRepo.Insert(lst));
		}
		catch (Exception ex)
		{
			return Result.Error<int>(ex);
		}
	}

	public async Task<IResult<int>> NewPreOrder(string market, int number)
	{
		var result = new BaseResult<int>();
		try
		{
			var uid = new Uuid(market);
			if (!uid.IsEmpty)
			{
				_mktRepo.Query(m => m.Id.Equals(uid));
			}
			else if (!string.IsNullOrEmpty(market?.Trim()))
			{
				_mktRepo.Query(m => market.Trim().ToLower().Equals(m.Name.ToLower()));
			}

			var mkt = await _mktRepo.JoinBy(m => m.BaseUnit).JoinBy(m => m.QuoteUnit).GetFirst();
			if (mkt == null) return result.BadRequest("Can not find Market");

			var lst = new List<PreOrder>();
			var mems = await _memRepo.GetList();

			for (var i = 0; i < number; i++)
			{
				var num = Crypto.Random.NewNumber(0, 15);
				var rand = Crypto.Random.NewNumber(-1000, 1000) / 100000m;
				var mem = mems[Crypto.Random.NewNumber(0, mems.Count)];

				lst.Add(new()
				{
					OwnerId = mem.Id,
					MarketId = mkt.Id,
					Symbol = mkt.Name,
					Side = num % 2 == 1,
					Type = OrderType.AllTypes[num % OrderType.AllTypes.Length],
					Price = 0,
					Volume = num * Math.Abs(rand),
					Amount = 0,
					Origin = 0,
					CreatedAt = Epoch.Now.Timestamp,
					IsDeleted = false,
				});

				var ord = lst[^1];
				switch (ord.Type)
				{
					case OrderType.LIMIT:
						ord.Price = (mkt.BaseUnit.Price / mkt.QuoteUnit.Price) * (1m + rand);
						break;
					case OrderType.STOP_LIMIT:
						ord.Price = (mkt.BaseUnit.Price / mkt.QuoteUnit.Price) * (1m + rand);
						ord.StopPrice = ord.Price * (1m + rand * 2);
						break;
					case OrderType.STOP_MARKET:
						ord.Price = (mkt.BaseUnit.Price / mkt.QuoteUnit.Price) * (1m + rand * 3);
						break;
					case OrderType.ICEBERG:
						ord.Price = (mkt.BaseUnit.Price / mkt.QuoteUnit.Price) * (1m + rand * 2);
						ord.Origin = ord.Volume;
						ord.Volume /= 10;
						break;
					case OrderType.GOOD_TILL_DATE:
						ord.Price = (mkt.BaseUnit.Price / mkt.QuoteUnit.Price) * (1m + rand * 2);
						ord.ExpiredAt = Epoch.Now.AddMinutes((int)(rand * 100));
						ord.Condition = OrderType.GOOD_TILL_DATE;
						break;
					case OrderType.IMMEDIATE_OR_CANCEL:
						ord.Price = (mkt.BaseUnit.Price / mkt.QuoteUnit.Price) * (1m + rand * 2);
						ord.Condition = OrderType.IMMEDIATE_OR_CANCEL;
						break;
					case OrderType.FILL_OR_KILL:
						ord.Price = (mkt.BaseUnit.Price / mkt.QuoteUnit.Price) * (1m + rand);
						ord.Condition = OrderType.FILL_OR_KILL;
						break;
				}

				ord.Price = Math.Round(ord.Price ?? 0m, mkt.PricePrecision ?? mkt.QuoteUnit.Precision ?? 3);
				ord.StopPrice = Math.Round(ord.StopPrice ?? 0m, mkt.PricePrecision ?? mkt.QuoteUnit.Precision ?? 3);
				ord.Volume = Math.Round(ord.Volume ?? 0m, mkt.VolumePrecision ?? mkt.BaseUnit.Precision ?? 3);
			}

			return result.Ok(await _proRepo.Insert(lst));
		}
		catch (Exception ex)
		{
			return result.Error(ex);
		}
	}
}