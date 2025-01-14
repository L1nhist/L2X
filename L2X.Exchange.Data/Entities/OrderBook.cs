using L2X.Exchange.Models;

namespace L2X.Exchange.Data.Entities;

public class OrderBook : Entity<long>, IOrderBookEntry
{
    /// <summary>
    /// Mã Uid độc để dịnh danh cho kênh trao đổi tương ứng
    /// </summary>
    public Guid MarketId { get; set; }

    public string? Symbol { get; set; }

    public decimal Price { get; set; }

    public decimal Volume { get; set; }

    public int TotalOrder { get; set; }

    public long ModifiedAt { get; set; }
}