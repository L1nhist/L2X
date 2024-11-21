using System.Security.Claims;

namespace L2X.Core.Authentications;

public class AuthUser(ClaimsPrincipal principal) : ClaimsPrincipal(principal), IAuthUser
{
    public Uuid Id => new(Get("id", ClaimTypes.NameIdentifier));

    public string? Email => Get("Email", ClaimTypes.Email);

    public string? Username => Get("Name", "Sub", ClaimTypes.Name);

    public IEnumerable<string>? Roles => Get("Role", ClaimTypes.Role)?.Split(',', StringSplitOptions.RemoveEmptyEntries);

    public IEnumerable<string>? Policies => Get("policies")?.Split(',', StringSplitOptions.RemoveEmptyEntries);

    public string? Get(params string[] claims)
        => FindFirst(c => claims.Contains(c.Type))?.Value;
}