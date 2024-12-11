namespace L2X.Exchange.Matching.Collections;

internal class Side<Tpl, TOrd>(IComparer<decimal> priceComparer, IComparer<Tpl> levelComparer)
    where Tpl : class, IPriceLevel<TOrd>, new()
    where TOrd : class, IOrder
{
    #region Singleton Comparer
    private class PriceComparer(bool ascending = true) : IComparer<decimal>
    {
        private readonly bool _ascending = ascending;

        public int Compare(decimal x, decimal y)
            => _ascending ? x.CompareTo(y) : y.CompareTo(x);
    }

    private class PriceLevelComparer(bool ascending = true) : IComparer<Tpl>
    {
        private readonly bool _ascending = ascending;

        public int Compare(Tpl? x, Tpl? y)
            => _ascending ? x!.Price.CompareTo(y!.Price) : y!.Price.CompareTo(x!.Price);
    }

    private readonly static PriceComparer _ascPricing = new();
    private readonly static PriceComparer _descPricing = new(false);

    private readonly static PriceLevelComparer _ascLeveling = new();
    private readonly static PriceLevelComparer _descLeveling = new(false);

    public static Side<Tpl, TOrd> Ascending => new(_ascPricing, _ascLeveling);
    public static Side<Tpl, TOrd> Descending => new(_descPricing, _descLeveling);
    #endregion

    #region Properties
    private readonly Tpl _current = new();

    private readonly IComparer<decimal> _comparer = priceComparer;

    private readonly SortedSet<Tpl> _levels = new(levelComparer);

    public int Count => _levels.Count;

    public Tpl? BestPriceLevel { get; private set; } = null;

    public IEnumerable<Tpl> PriceLevels => _levels;
    #endregion

    #region Methods
    public void AddOrder(TOrd order, decimal price)
    {
        Tpl level = GetOrAddLevel(price);
        level.AddOrder(order);
    }

    private Tpl GetOrAddLevel(decimal price)
    {
        _current.SetPrice(price);
        if (!_levels.TryGetValue(_current, out Tpl? level))
        {
            level = new Tpl();
            level.SetPrice(price);
            _levels.Add(level);

            if (BestPriceLevel == null || _comparer.Compare(price, BestPriceLevel.Price) < 0)
                BestPriceLevel = level;
        }

        return level;
    }

    public bool RemoveOrder(TOrd order, decimal price)
    {
        bool removed = false;
        _current.SetPrice(price);
        if (_levels.TryGetValue(_current, out Tpl? level))
        {
            removed = level.RemoveOrder(order);
            RemoveIfEmpty(level);
        }
        return removed;
    }

    public void DecreaseOrder(TOrd order, decimal decrement)
    {
        _current.SetPrice(order.Price ?? 0m);
        if (_levels.TryGetValue(_current, out Tpl? level))
            level.DecreaseOrder(order, decrement);
    }

    public bool FillOrder(TOrd order, decimal volume)
    {
        _current.SetPrice(order.Price ?? 0m);
        if (!_levels.TryGetValue(_current, out Tpl? level)) return false;

        bool filled = level.FillOrder(order, volume);
        RemoveIfEmpty(level);
        return filled;
    }

    public bool LimitOrderCanBeFilled(decimal volume, decimal limitPrice)
    {
        decimal sum = 0;
        foreach (var l in _levels)
        {
            if (limitPrice != 0 && _comparer.Compare(limitPrice, l.Price) < 0) break;

            sum += l.Volume;
            if (sum >= volume) return true;
        }

        return false;
    }

    public bool MarketOrderCanBeFilled(decimal amount)
    {
        decimal sum = 0;
        foreach (var l in _levels)
        {
            sum += l.Volume * l.Price;
            if (sum >= amount) return true;
        }

        return false;
    }

    private void RemoveIfEmpty(Tpl level)
    {
        if (level.Count == 0)
        {
            _levels.Remove(level);
            if (BestPriceLevel?.Price == level.Price)
                BestPriceLevel = _levels.Min;
        }
    }

    public IReadOnlyList<Tpl>? RemoveTill(decimal price)
    {
        List<Tpl>? removes = null;
        if (BestPriceLevel != null && _comparer.Compare(BestPriceLevel.Price, price) <= 0)
        {
            removes = [];
            BestPriceLevel = null;

            foreach (var l in _levels)
            {
                if (_comparer.Compare(l.Price, price) > 0)
                    BestPriceLevel = l;
                else
                    removes.Add(l);
            }

            foreach (var l in removes)
            {
                _levels.Remove(l);
            }
        }
        return removes;
    }
    #endregion
}