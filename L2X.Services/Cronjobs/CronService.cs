using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace L2X.Services.Cronjobs;

public class CronService<T> : IHostedService, IDisposable
    where T : class, ICronJob
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _provider;
    private readonly SemaphoreSlim _semaCycle;

    private T? _cronJob;

    private CancellationTokenSource? _cancelSrc;

    private PeriodicTimer? _timer;

    public CronService(ILoggerFactory logFactory, IServiceProvider provider)
    {
        _logger = logFactory.CreateLogger(GetType());
        _provider = provider;
        _semaCycle = new(0);

        _cancelSrc = null;
        _cronJob = null;
        _timer = null;
    }

    public async Task StartAsync(CancellationToken token)
    {
        using var scope = _provider.CreateScope();

        _cancelSrc = CancellationTokenSource.CreateLinkedTokenSource(token);
        _cronJob = scope.ServiceProvider.GetRequiredService<T>();
        if (_cronJob == null) return;

        if (_cronJob.Delay > 0)
            await Task.Delay(_cronJob.Delay, token);

        _timer = new(TimeSpan.FromMilliseconds(_cronJob.Interval));
        while (await _timer.WaitForNextTickAsync(token))
        {
            try
            {
                if (!token.IsCancellationRequested)
                {
                    await _cronJob.DoWork(token);
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "{LoggerName}: an error happened during execution of the job", GetType().Name);
            }
            finally
            {
                _semaCycle.Release(); // Let the outer loop know that the next occurrence can be calculated.
            }

            await _semaCycle.WaitAsync(token); // Wait nicely for any timer result.
        }
    }

    public async Task StopAsync(CancellationToken token)
    {
        _timer?.Dispose();

        if (_cancelSrc != null)
            await _cancelSrc.CancelAsync();
    }

    public void Dispose()
    {
        _cronJob = null;
        _timer?.Dispose();
        _cancelSrc?.Dispose();
        _semaCycle.Dispose();
        GC.SuppressFinalize(this);
    }
}