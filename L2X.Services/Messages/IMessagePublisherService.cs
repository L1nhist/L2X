namespace L2X.Services.Messages;

public interface IMessagePublisherService<T>
{
    Task<bool> Publish(string topic, T? data);

    string? Serialize(T? data);
}