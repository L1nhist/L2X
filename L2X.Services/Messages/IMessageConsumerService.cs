namespace L2X.Services.Messages;

public interface IMessageConsumerService<T>
{
    Task Subscribe(string topic, int interval);

    T? Deserialize(string? message);
}