using L2X.Core.Utilities;

namespace L2X.MessageQueue.Services;

public class KafkaConsumerService : BackgroundService, IMessageConsumerService, IDisposable
{
    private readonly IConfiguration _config;
    private readonly ILogger _logger;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly CancellationTokenSource _cancellation;

    private bool _isRunning;
    private Task? _consuming;

    public int Interval { get; set; } = 0;

    public KafkaConsumerService(IConfiguration configuration, ILoggerFactory logFactory)
    {
        _config = configuration;
        _logger = logFactory.CreateLogger(GetType());
        _consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig
        {
            BootstrapServers = _config["Kafka:BootstrapServers"],
            GroupId = _config["Kafka:GroupIdChannel"],
            AutoOffsetReset = AutoOffsetReset.Earliest
        }).Build();

        _consuming = null;
        _isRunning = false;
        _cancellation = new CancellationTokenSource();
    }

    #region Overridens
    public override void Dispose()
    {
        _cancellation.Cancel();
        _isRunning = false;

        _consumer.Unsubscribe();
        _consumer.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }

    public new async Task StopAsync(CancellationToken token)
    {
        if (_consuming == null) return;

        try
        {
            _cancellation.Cancel();
            _isRunning = false;
        }
        finally
        {
            await Task.WhenAny(_consuming, Task.Delay(Timeout.Infinite, token));
        }
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(1));

        while (await timer.WaitForNextTickAsync(token))
        {
            if (_isRunning || _consumer == null) return;

            try
            {
                var result = _consumer.Consume(token);
                await Consume(result.Message.Value);

                WriteLog($"Received message: {result.Message.Value}");
            }
            catch (Exception ex)
            {
                WriteLog($"Error consumming Kafka message: {ex.Message}");
            }
        }

        _consumer.Close();
    }

    public async Task Subscribe(string topic, int interval)
    {
        Interval = interval > 0 ? interval : 0;
        _consumer.Subscribe(topic);
        await StartAsync();
    }

    public virtual Task<bool> Consume(string? message)
        => Task.FromResult(!Util.IsEmpty(message));
    #endregion

    public async Task StartAsync()
        => await StartAsync(_cancellation.Token);

    public void WriteLog(string message)
        => _logger.LogInformation(message);
}

public class KafkaConsumerService<T>(IConfiguration configuration, ILoggerFactory logFactory) :
    KafkaConsumerService(configuration, logFactory), IMessageConsumerService<T>, IDisposable
{
    #region Overridens
    public override async Task<bool> Consume(string? message)
    {
        if (Util.IsEmpty(message)) return false;

        var data = Deserialize(message);
        return await Consume(data);
    }

    public virtual Task<bool> Consume(T? data)
        => Task.FromResult(data is not null);
    #endregion

    public virtual T? Deserialize(string? message)
    {
        if (Util.IsEmpty(message)) return default;

        ReadOnlyMemory<byte> bytes = Convert.FromBase64String(message);
        return Serializer.Deserialize<T>(bytes);
    }
}