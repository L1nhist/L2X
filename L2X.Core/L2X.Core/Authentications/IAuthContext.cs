namespace L2X.Core.Authentications;

public interface IAuthContext
{
    bool IsAuthenticated { get; }

    IAuthUser? User { get; }
}