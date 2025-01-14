using System.Diagnostics.CodeAnalysis;

namespace L2X.MatchingEngine.Collections;

internal class RangeTracker(long startId, int length) : IEnumerable<long>, IEnumerable
{
    #region Properties
    private BitArray? _bitArray = length > 1 ? new BitArray(length) : null;

    private readonly long _startId = startId;

    internal bool Compacted => _bitArray == null;

    public long FromOrderId { get; private set; } = startId;

    public long ToOrderId { get; private set; } = startId + length - 1;
    #endregion

    #region Overridens
    public IEnumerator<long> GetEnumerator()
    {
        for (var i = FromOrderId; i <= ToOrderId; i++)
        {
            if (IsMarked(i)) yield return i;
            else continue;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
    #endregion

    [MemberNotNullWhen(true, nameof(_bitArray))]
    private bool IsInRange(long orderId)
        => _bitArray != null && orderId >= _startId && orderId <= _startId + _bitArray.Length - 1;

    public bool TryMark(long orderId)
    {
        if (IsInRange(orderId))
        {
            int index = (int)(orderId - _startId);
            if (!_bitArray.Get(index))
            {
                _bitArray.Set(index, true);
                return true;
            }
        }
        return false;
    }

    public int CountUnmarked()
    {
        if (_bitArray == null) return 0;

        int count = 0;
        for (int i = 0; i < _bitArray.Length; i++)
        {
            if (!_bitArray[i]) count++;
        }

        if (count == 0) _bitArray = null;
        return count;
    }

    public void ExtendMarkFromOrderId(long fromOrderId)
    {
        FromOrderId = fromOrderId;
    }

    public void ExtendMarkToOrderId(long toOrderId)
    {
        ToOrderId = toOrderId;
    }

    public bool IsMarked(long orderId)
    {
        if (orderId < FromOrderId || orderId > ToOrderId) return false;

        if (_bitArray != null && IsInRange(orderId))
        {
            int index = (int)(orderId - _startId);
            return _bitArray[index];
        }

        return true;
    }
}