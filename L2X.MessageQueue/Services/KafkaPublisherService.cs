using L2X.Core.Utilities;

namespace L2X.MessageQueue.Services;

public class KafkaPublisherService : IMessagePublisherService, IDisposable
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
            BootstrapServers = _config["Kafka:BootstrapServers"]
        }).Build();
    }

    #region Overridens
    public virtual void Dispose()
    {
        _producer.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task<bool> Publish(string topic, string? message)
    {
        if (Util.IsEmpty(message)) return false;

        try
        {
            WriteLog($"Published message: {message}");
            var result = await _producer.ProduceAsync(topic, new() { Value = message, });
            return result.Status != PersistenceStatus.NotPersisted;
        }
        catch (Exception ex)
        {
            WriteLog($"Error processing Kafka message: {ex.Message}");
        }

        return false;
    }
    #endregion

    public void WriteLog(string message)
        => _logger.LogInformation(message);
}

public class KafkaPublisherService<T>(IConfiguration configuration, ILoggerFactory logFactory)
    : KafkaPublisherService(configuration, logFactory), IMessagePublisherService<T>, IDisposable
{
    #region Overridens
    public async Task<bool> Publish(string topic, T? data)
    {
        if (data == null) return false;

        try
        {
            var msg = Serialize(data);
            return await Publish(topic, msg);
        }
        catch (Exception ex)
        {
            WriteLog($"Error processing Kafka message: {ex.Message}");
        }

        return false;
    }
    #endregion

    public virtual string Serialize(T? data)
    {
        using MemoryStream strm = new();
        Serializer.Serialize(strm, data);
        var bytes = strm.ToArray();
        return Convert.ToBase64String(bytes);
    }
}