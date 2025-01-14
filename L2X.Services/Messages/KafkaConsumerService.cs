namespace L2X.Services.Messages;

public class KafkaConsumerService<T> : IMessageConsumerService<T>, IDisposable
{
    private readonly IConfiguration _config;
    private readonly ILogger _logger;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IConsumeHandler<T> _handler;
    private readonly CancellationTokenSource _cancellation;

    private bool _running;
    private int _interval;
    private Task? _consuming;

    public KafkaConsumerService(IConfiguration configuration, ILoggerFactory logFactory, IConsumeHandler<T> handler)
    {
        _config = configuration;
        _logger = logFactory.CreateLogger(GetType());
        _consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig
        {
            BootstrapServers = _config["Kafka:BootstrapServers"],
            GroupId = _config["Kafka:GroupIdChannel"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            IsolationLevel = IsolationLevel.ReadCommitted,
        }).Build();

        _handler = handler;
        _running = false;
        _interval = 0;
        _cancellation = new CancellationTokenSource();
    }

    #region Overridens
    public void Dispose()
    {
        _cancellation.Cancel();
        _running = false;

        _consumer.Unsubscribe();
        _consumer.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task StopAsync(CancellationToken token)
    {
        if (_consuming == null) return;

        try
        {
            _cancellation.Cancel();
            _running = false;
        }
        finally
        {
            await Task.WhenAny(_consuming, Task.Delay(Timeout.Infinite, token));
        }
    }

    public async Task StartAsync(CancellationToken token)
    {
        if (token.IsCancellationRequested) return;

        using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(_interval));

        _running = true;
        while (await timer.WaitForNextTickAsync(token))
        {
            //if (!_running || _consumer == null) return;
            if (!_running) return;

            try
            {
                var result = _consumer.Consume(token);
                //WriteLog($"Received message: {result.Message.Value}");

                var data = Deserialize(result.Message.Value);
                if (data == null)
                {
                    _logger.WriteLog("Received message can not be deserialized to consume");
                }
                else
                {
                    //_consuming = _handler.Consume(data);
                    //_consuming.Start();
                    await _handler.Consume(data);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(ex);
            }
        }

        _consumer.Close();
    }

    public async Task Subscribe(string topic, int interval)
    {
        _interval = interval > 0 ? interval : 0;
        _consumer.Subscribe(topic);
        await StartAsync(_cancellation.Token);
    }
    #endregion

    public virtual T? Deserialize(string? message)
    {
        if (Util.IsEmpty(message)) return default;
        if (typeof(T) == typeof(string)) return (T)(object)message;

        ReadOnlyMemory<byte> bytes = Convert.FromBase64String(message);
        return Serializer.Deserialize<T>(bytes);
    }
}