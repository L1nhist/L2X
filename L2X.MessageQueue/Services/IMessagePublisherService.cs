namespace L2X.MessageQueue.Services;

public interface IMessagePublisherService
{
    Task<bool> Publish(string topic, string? message);
}

public interface IMessagePublisherService<T> : IMessagePublisherService
{
    string? Serialize(T? data);

    Task<bool> Publish(string topic, T? data);
}