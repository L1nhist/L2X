namespace L2X.Exchange.Matching.Collections;

public interface IPriceLevel<T> : IEnumerable<T>
    where T : class, IOrder
{
    #region Properties
    public int Count { get; }

    public decimal Price { get; }

    public decimal Volume { get; }
    #endregion

    #region Methods
    bool AddOrder(T order);

    bool RemoveOrder(T order);

    void SetPrice(decimal price);

    bool DecreaseOrder(T order, decimal decrement);

    bool FillOrder(T order, decimal volume);
    #endregion
}