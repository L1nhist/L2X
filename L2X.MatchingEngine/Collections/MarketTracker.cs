using L2X.Core.Structures;

namespace L2X.MatchingEngine.Collections;

public class MarketTracker(int length) : IOrderBook
{
	#region Internal
	private class OBComparer(bool ascending = true) : IComparer<OBEntry>
	{
		private readonly bool _ascending = ascending;

		public int Compare(OBEntry? x, OBEntry? y)
			=> _ascending ? x!.Price.CompareTo(y!.Price) : y!.Price.CompareTo(x!.Price);
	}

	private readonly static OBComparer _ascPricing = new();
	private readonly static OBComparer _descPricing = new(false);

	private class MKTEntry : ITrade
	{
		public decimal Price { get; set; }

		public decimal Volume { get; set; }

		public long Timestamp { get; set; }
	}

	private class OBEntry : IOrderBookEntry
	{
		public decimal Price { get; set; }

		public decimal Volume { get; set; }
	}

	private readonly OBEntry _current = new();

	private decimal _totalPrc = 0m;

	private decimal _totalVol = 0m;

	private readonly List<MKTEntry> _mkts = [];

	private readonly SortedSet<OBEntry> _asks = new(_ascPricing);

	private readonly SortedSet<OBEntry> _bids = new(_descPricing);
	#endregion

	public int MaxLength { get; } = length;

	public decimal PriceLast
		=> _mkts.Count > 0 ? _mkts[^1].Price : 0;

	public decimal VolumeLast
		=> _mkts.Count > 0 ? _mkts[^1].Volume : 0;

	public decimal TotalBids
		=> _totalPrc;

	public decimal TotalAsks
		=> _totalVol;

	public decimal PriceChg
		=> _mkts.Count > 1 ? _mkts[^1].Price - _mkts[0].Price : 0;

    public decimal PriceRate
        => _mkts.Count > 1 ? Math.Round((_mkts[^1].Price - _mkts[0].Price) / _mkts[0].Price * 100, 2) : 0;

    public decimal PriceMin { get; private set; } = 0m;

	public decimal PriceMax { get; private set; } = 0m;

	public IEnumerable<IOrderBookEntry> Asks {
		get => _asks;
	}

	public IEnumerable<IOrderBookEntry> Bids
	{
		get => _bids;
	}

	public IEnumerable<ITrade> Trades
	{
		get => _mkts;
	}

	public void SetTradeInfo(decimal price, decimal volume)
	{
		var ts = Epoch.Now;
		_mkts.Add(new()
		{
			Price = price,
			Volume = volume,
			Timestamp = ts.Timestamp,
		});

		if (PriceMin <= 0 || price < PriceMin)
			PriceMin = price;
		if (PriceMax <= 0 || price > PriceMax)
			PriceMax = price;
		_totalPrc += price;
		_totalVol += volume;

		ts = ts.AddHours(-24);
		for (var i = 0; i < _mkts.Count; i++)
		{
			if (_mkts[i].Timestamp > ts.Timestamp) continue;

            _totalPrc -= _mkts[i].Price;
            _totalVol -= _mkts[i].Volume;
			_mkts.RemoveAt(i);
		}
	}

	public bool SetOrderBook(bool side, decimal price, decimal volume)
	{
		var lst = side ? _bids : _asks;
		_current.Price = price;
		if (lst.TryGetValue(_current, out var entry))
		{
			entry.Volume = volume;
			CleanOrderBook(side);
			return true;
		}
		else if (volume > 0 && lst.Count <= MaxLength)
		{
			lst.Add(new()
			{
				Price = price,
				Volume = volume
			});
			return true;
		}

		return false;
	}

	public void CleanOrderBook(bool side)
	{
		var lst = side ? _bids : _asks;
		var len = 0;
		var rems = new List<OBEntry>();
		foreach (var en in lst)
		{
			if (en.Volume <= 0 || len > MaxLength)
				rems.Add(en);
			else
				++len;
		}
		foreach (var en in rems)
		{
			lst.Remove(en);
		}
	}
}
