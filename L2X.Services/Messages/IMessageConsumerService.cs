namespace L2X.Services.Messages;

public interface IMessageConsumerService
{
    int Interval { get; }

    void Subscribe(string topic, int interval);

    Task<bool> Consume(string message);
}

public interface IMessageConsumerService<T> : IMessageConsumerService
{
    T? Deserialize(string message);

    Task<bool> Consume(T? data);
}