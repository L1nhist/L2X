using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using L2X.Core.Authentications;
using L2X.Core.Validations;

namespace L2X.Core;

public static class Startup
{
    public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddTransient<IAuthContext, AuthContext>();
        services.AddScoped(typeof(IValidator<>), typeof(Validator<>));
    }

    public static IServiceCollection AddCoreScope(this IServiceCollection services)
        => services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                    .AddScoped<IAuthContext, AuthContext>()
                    .AddScoped(typeof(IValidator<>), typeof(Validator<>));
}