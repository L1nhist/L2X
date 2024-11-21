using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using L2X.Services.Managements;

namespace L2X.Services;

public static class Startup
{
    public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        Data.Startup.ConfigureServices(configuration, services);

        services.AddScoped(typeof(IDataManagementService<>), typeof(DataManagementService<>));

    }
}