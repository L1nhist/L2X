using L2X.Core.Extensions;
using L2X.Core.Utilities;

namespace L2X.MessageQueue.Services;

public class KafkaPublisherService<T> : IMessagePublisherService<T>, IDisposable
{
    private readonly IConfiguration _config;

    private readonly ILogger _logger;

    private readonly IProducer<Null, string> _producer;

    public KafkaPublisherService(IConfiguration configuration, ILoggerFactory logFactory)
    {
        _config = configuration;
        _logger = logFactory.CreateLogger(GetType());
        _producer = new ProducerBuilder<Null, string>(new ProducerConfig
        {
            BootstrapServers = _config["Kafka:BootstrapServers"],
            AllowAutoCreateTopics = true,
            Acks = Acks.Leader,
        }).Build();
    }

    #region Overridens
    public virtual void Dispose()
    {
        _producer.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task<bool> Publish(string topic, T? data)
    {
        if (Util.IsEmpty(data)) return false;

        try
        {
			//WriteLog($"Published message: {message}");
			var msg = Serialize(data);
            if (msg == null)
			{
                _logger.WriteLog($"Data can not be serialized to publish");
				return false;
            }

			var result = await _producer.ProduceAsync(topic, new() { Value = msg, });
            return result.Status != PersistenceStatus.NotPersisted;
        }
        catch (Exception ex)
        {
			_logger.WriteLog($"Error processing Kafka message: {ex.Message}");
        }

        return false;
    }
	#endregion

	public virtual string? Serialize(T? data)
	{
        if (Util.IsEmpty(data)) return null;
        if (typeof(T) == typeof(string)) return data.ToString();

        using MemoryStream strm = new();
		Serializer.Serialize(strm, data);
		var bytes = strm.ToArray();
		return Convert.ToBase64String(bytes);
	}
}