using L2X.MessageQueue.Models.Matching;
using L2X.MessageQueue.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProtoBuf.Meta;

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
                srvs.AddLogging(l => l.ClearProviders().AddConsole());
                srvs.AddScoped(typeof(KafkaConsumerService<MOrder>));
            })
            .UseConsoleLifetime()
            .Build();

        var consumer = host.Services.GetService<KafkaConsumerService<MOrder>>();
        if (consumer == null)
        {
            Console.WriteLine("Can not create a Consumer");
            Console.ReadLine();
            return;
        }

        await consumer.Subscribe("Order", 100);

        Console.WriteLine("Waiting for message");
        Console.WriteLine("");
        Console.WriteLine("------------------------------");
        Console.WriteLine("");
        Console.ReadLine();
    }
}