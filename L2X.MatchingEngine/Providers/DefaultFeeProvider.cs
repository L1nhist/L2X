namespace L2X.MatchingEngine.Providers;

public class DefaultFeeProvider(decimal maker = 0, decimal taker = 0) : IFeeProvider
{
    private readonly decimal _maker = maker;

    private readonly decimal _taker = taker;

    public Task<decimal> GetMakerFee(int feeId) => Task.FromResult(_maker);

    public Task<decimal> GetTakerFee(int feeId) => Task.FromResult(_taker);
}