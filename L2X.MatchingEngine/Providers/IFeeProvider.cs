﻿namespace L2X.MatchingEngine.Providers;

public interface IFeeProvider
{
    Task<decimal> GetMakerFee(int feeId);

    Task<decimal> GetTakerFee(int feeId);
}