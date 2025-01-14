using L2X.Services.Cronjobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace L2X.BrokingEngine;

public static class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Start BrokingEngine......");

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureHostConfiguration(cfgs => {
                cfgs.AddJsonFile("appsettings.json");
            })
            .ConfigureServices((contx, srvs) => {
				L2X.Data.Startup.ConfigureServices(contx.Configuration, srvs);

				srvs.AddDbContext<L2XDbContext>((provider, opts) =>
				    {
					    opts.UseNpgsql(contx.Configuration.GetConnectionString("NpgsqlDB"), opts => opts.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                        //opts.AddInterceptors(provider.GetServices<ISaveChangesInterceptor>());
                    }
                );

		        srvs.AddSingleton<DbContext, L2XDbContext>();

		        srvs.AddLogging(l => l.ClearProviders().AddConsole());
                srvs.AddScoped<PreOrderCronJob>();

                srvs.AddHostedService<CronService<PreOrderCronJob>>();
            })
            .UseConsoleLifetime()
            .Build();

        Console.WriteLine("Waiting for new order to process");
        Console.WriteLine("");
        Console.WriteLine("------------------------------");
        Console.WriteLine("");

        host.Run();
    }
}