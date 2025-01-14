using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace L2X.MatchingEngine;

public static class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Start MatchingEngine......");

		var host = Host.CreateDefaultBuilder(args)
			.ConfigureHostConfiguration(cfgs =>
			{
				cfgs.AddJsonFile("appsettings.json");
			})
			.ConfigureServices((contx, srvs) =>
			{
				L2X.Exchange.Data.Startup.ConfigureServices(contx.Configuration, srvs);

				srvs.AddLogging(l => l.ClearProviders().AddConsole());
				srvs.AddScoped<MatchHandler>();
			})
			.UseConsoleLifetime()
			.Build();

		Console.WriteLine("Waiting for Orders in Queue to Match");

		Console.WriteLine("");
		Console.WriteLine("------------------------------");
		Console.WriteLine("");

		var matching = host.Services.GetService<MatchHandler>();

		if (matching == null)
		{
			Console.WriteLine("Service Provider get failed!");
			Console.ReadLine();
			return;
		}

		await matching.Initialize();

		Console.ReadLine();
	}
}