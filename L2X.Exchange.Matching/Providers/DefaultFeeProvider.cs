namespace L2X.Exchange.Matching.Providers;

public class DefaultFeeProvider(decimal maker = 0, decimal taker = 0) : IFeeProvider
{
    private readonly decimal _maker = maker;

    private readonly decimal _taker = taker;

    public async Task<decimal> GetMakerFee(int feeId) => _maker;

    public async Task<decimal> GetTakerFee(int feeId) => _taker;
}