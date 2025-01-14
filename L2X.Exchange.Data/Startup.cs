using L2X.Data;
using L2X.Exchange.Data.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace L2X.Exchange.Data;

public static class Startup
{
    public static void ConfigureServices(IConfiguration Configuration, IServiceCollection services)
    {
        L2X.Data.Startup.ConfigureServices(Configuration, services);

        services.AddDbContext<L2XDbContext>((provider, opts) =>
            {
                opts.UseNpgsql(Configuration.GetConnectionString("NpgsqlDB"), opts => opts.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                //opts.AddInterceptors(provider.GetServices<ISaveChangesInterceptor>());
            }
        );

        services.AddScoped<DbContext, L2XDbContext>();
        services.AddScoped<IOrderService, OrderService>();
    }

    public static IServiceCollection AddPostgreExchangeContext(this IServiceCollection services, IConfiguration Configuration)
        => services.AddDataScope().AddDbContext<L2XDbContext>((provider, opts) =>
                {
                    opts.UseNpgsql(Configuration.GetConnectionString("NpgsqlDB"), opts => opts.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                    //opts.AddInterceptors(provider.GetServices<ISaveChangesInterceptor>());
                })
            .AddScoped<DbContext, L2XDbContext>()
            .AddScoped<IOrderService, OrderService>();
}