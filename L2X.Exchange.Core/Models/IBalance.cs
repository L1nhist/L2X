namespace L2X.Exchange.Models;

/// <summary>
/// Balance amount of specific asset in an account
/// </summary>
public interface IBalance
{
    /// <summary>
    /// The name of asset that account is holding
    /// </summary>
    string Asset { get; set; }

    /// <summary>
    /// Amount of asset available for user in account
    /// </summary>
    decimal? Available { get; set; }

    /// <summary>
    /// Total amount of asset included available and locked amount in account
    /// </summary>
    decimal? Total { get; set; }
}