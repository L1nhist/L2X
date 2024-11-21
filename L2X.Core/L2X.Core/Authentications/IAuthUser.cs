using System.Security.Principal;

namespace L2X.Core.Authentications;

public interface IAuthUser : IPrincipal
{
    string? Email { get; }

    string? Username { get; }

    IEnumerable<string>? Roles { get; }

    IEnumerable<string>? Policies { get; }
}