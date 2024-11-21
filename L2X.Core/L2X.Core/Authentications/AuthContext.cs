using Microsoft.AspNetCore.Http;

namespace L2X.Core.Authentications;

public sealed class AuthContext(IHttpContextAccessor accessor) : IAuthContext
{
    private readonly HttpContext _context = accessor.HttpContext ?? throw new NullReferenceException("HttpContextAccessor must not be null");

    public bool IsAuthenticated => _context?.User?.Identity?.IsAuthenticated == true;

    public IAuthUser? User => IsAuthenticated ? new AuthUser(_context.User) : null;
}