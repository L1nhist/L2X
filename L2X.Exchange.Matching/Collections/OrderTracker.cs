using L2X.Exchange.Matching.Exceptions;

namespace L2X.Exchange.Matching.Collections;

public class OrderIdTracker(int rangeSize = 256) : IEnumerable<OrderId>
{
    private class RangeTrackerComparer : IComparer<RangeTracker>
    {
        public int Compare(RangeTracker? x, RangeTracker? y)
        {
            if (x!.FromOrderId < y!.FromOrderId) return -1;
            if (x.ToOrderId > y.ToOrderId) return 1;
            if (x.FromOrderId >= y.FromOrderId && x.ToOrderId <= y.ToOrderId) return 0;

            throw new MatchingException("RangeTrackerComparer not expected");
        }
    }

    private static readonly RangeTrackerComparer _comparer = new();

    #region Properties
    private readonly int _rangeSize = rangeSize;
    private readonly SortedSet<RangeTracker> _ranges = new(_comparer);

    private int _markIndex = 0;
    private RangeTracker? _current;

    public int Count => _ranges.Count;

    internal IEnumerable<RangeTracker> Ranges => _ranges;
    #endregion

    #region Overridens
    public IEnumerator<OrderId> GetEnumerator()
    {
        foreach (var range in Ranges)
            foreach (var orderId in range)
                yield return orderId;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
    #endregion

    #region Methods
    public void Clear()
    {
        _current = null;
        _markIndex = 0;
        _ranges.Clear();
    }

    public void Compact()
    {
        RangeTracker? prev = null;
        List<RangeTracker> removes = [];
        foreach (var range in _ranges)
        {
            prev ??= range;
            if (range.CountUnmarked() == 0 && prev.ToOrderId + 1 == range.FromOrderId)
            {
                prev.ExtendMarkToOrderId(range.ToOrderId);
                removes.Add(range);
            }
            else
            {
                prev = range;
            }
        }

        foreach (var r in removes)
        {
            _ranges.Remove(r);
        }
    }

    private RangeTracker CreateRange(OrderId orderId)
    {
        _current = new RangeTracker(orderId - orderId % _rangeSize, _rangeSize);
        _ranges.Add(_current);
        return _current;
    }

    private RangeTracker? GetRange(OrderId orderId)
    {
        if (_current == null || orderId < _current.FromOrderId || orderId > _current.ToOrderId)
            _current = _ranges.FirstOrDefault(r => orderId >= r.FromOrderId && orderId <= r.ToOrderId);

        return _current;
    }

    public bool IsMarked(OrderId orderId)
        => GetRange(orderId)?.IsMarked(orderId) ?? false;

    public bool TryMark(OrderId orderId)
    {
        var result = (GetRange(orderId) ?? CreateRange(orderId)).TryMark(orderId);
        if (result && ++_markIndex >= _rangeSize * 4)
        {
            Compact();
            _markIndex = 0;
        }

        return result;
    }
    #endregion
}