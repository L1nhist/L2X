namespace L2X.Exchange.Matching.Providers;

public interface IFeeProvider
{
    Task<decimal> GetMakerFee(int feeId);

    Task<decimal> GetTakerFee(int feeId);
}