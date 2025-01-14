namespace L2X.Services.Cronjobs;

public interface ICronJob
{
    int Interval { get; }

    int Delay { get; }

    Task DoWork(CancellationToken token = default);
}