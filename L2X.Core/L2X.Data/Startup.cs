using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using L2X.Data.Extentions;
using L2X.Data.Repositories;
using System.Reflection;

namespace L2X.Data;

public static class Startup
{
    public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        Core.Startup.ConfigureServices(configuration, services);

        services.AddScoped(typeof(IRepository<>), typeof(DataRepository<>));
        services.AddScoped<ISaveChangesInterceptor, AuditingInterceptor>();

        //Register AutoMapper by MapperProfiles
        var type = typeof(Profile);
        var profiles = AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a => a != Assembly.GetAssembly(type))
                        .SelectMany(a => a.GetExportedTypes().Where(t => t.IsSubclassOf(type)));

        services.AddAutoMapper(opts =>
        {
            foreach (var p in profiles)
            {
                opts.AddProfile(p);
            }
        });
    }
}