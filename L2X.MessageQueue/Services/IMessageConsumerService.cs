namespace L2X.MessageQueue.Services;

public interface IMessageConsumerService
{
    int Interval { get; }

    Task Subscribe(string topic, int interval);

    Task<bool> Consume(string message);
}

public interface IMessageConsumerService<T> : IMessageConsumerService
{
    T? Deserialize(string message);

    Task<bool> Consume(T? data);
}