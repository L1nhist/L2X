using L2X.MessageQueue.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProtoBuf.Meta;

namespace L2X.MatchingEngine;

public static class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Start MatchingEngine......");

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureHostConfiguration(cfgs => {
                cfgs.AddJsonFile("appsettings.json");
            })
            .ConfigureServices((contx, srvs) => {
                srvs.AddLogging(l => l.ClearProviders().AddConsole());
                srvs.AddScoped(typeof(KafkaPublisherService<>));
            })
            .UseConsoleLifetime()
            .Build();

        Console.WriteLine("Click Enter to send Order");

        Console.WriteLine("");
        Console.WriteLine("------------------------------");
        Console.WriteLine("");

        var publisher = host.Services.GetService<KafkaPublisherService<MOrder>>();

        if (publisher == null)
        {
            Console.WriteLine("Service Provider get failed!");
            Console.ReadLine();
            return;
        }

        long idx = 0;
        var rand = new Random(433323);

        while (Console.ReadLine()?.ToLower() == "o")
        {
            var num = rand.Next(0, 99);

            await publisher.Publish("Order", new MOrder()
            {
                Id = idx++,
                Owner = "Test",
                Price = 10 +  num / 100,
                Side = num % 3 == 0 || num % 2 == 1,
                Volume = num / 21,
            });
        }
    }
}