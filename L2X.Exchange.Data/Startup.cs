using L2X.Data.Extensions;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
                opts.AddInterceptors(provider.GetServices<ISaveChangesInterceptor>());
            }
        );
        services.AddScoped<DbContext, L2XDbContext>();
    }
}