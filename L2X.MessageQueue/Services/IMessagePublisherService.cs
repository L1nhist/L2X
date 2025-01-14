namespace L2X.MessageQueue.Services;

public interface IMessagePublisherService<T>
{
    Task<bool> Publish(string topic, T? data);

    string? Serialize(T? data);
}