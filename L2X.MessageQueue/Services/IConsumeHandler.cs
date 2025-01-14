namespace L2X.MessageQueue.Services;

public interface IConsumeHandler<T>
{
	Task Consume(T? data);
}