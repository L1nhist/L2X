using L2X.Core.Caching;
using L2X.Services.Caching;
using L2X.Services.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace L2X.Services;

public static class Startup
{
    public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        services.AddScoped(typeof(IMessageConsumerService<>), typeof(KafkaConsumerService<>));
		services.AddScoped(typeof(IMessagePublisherService<>), typeof(KafkaPublisherService<>));
		services.AddScoped<RedisCacheService>();
	}
}