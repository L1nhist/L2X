namespace L2X.Services.Messages;

public interface IConsumeHandler<T>
{
    Task Consume(T? data);
}