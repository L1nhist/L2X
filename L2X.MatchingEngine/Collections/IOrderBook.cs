namespace L2X.MatchingEngine.Collections;

public interface IOrderBook<T>
    where T : class, IOrder
{
    decimal? BestBidPrice { get; }

    decimal? BestAskPrice { get; }

    decimal? BestStopAskPrice { get; }

    decimal? BestStopBidPrice { get; }

    decimal? BestBidVolume { get; }

    decimal? BestAskVolume { get; }

    IEnumerable<T> Orders { get; }

    IEnumerable<IPriceLevel<T>> Asks { get; }

    IEnumerable<IPriceLevel<T>> Bids { get; }

    IEnumerable<IPriceLevel<T>> StopAsks { get; }

    IEnumerable<IPriceLevel<T>> StopBids { get; }

    IEnumerable<KeyValuePair<long, HashSet<OrderId>>> GoodTillDates { get; }
}