using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using L2X.Core.Authentications;

namespace L2X.Core;

public static class Startup
{
    public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<IAuthContext, AuthContext>();
    }
}