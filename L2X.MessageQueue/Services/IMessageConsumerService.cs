namespace L2X.MessageQueue.Services;

public interface IMessageConsumerService<T>
{
    Task Subscribe(string topic, int interval);

    T? Deserialize(string? message);
}