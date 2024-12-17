using System.Threading;

namespace LionFire.Hosting;

public class DynamicHostedService : IHostedService
{
    public IServiceProvider ServiceProvider { get; }

    public DynamicHostedService(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public Func<IServiceProvider, CancellationToken, ValueTask>? OnStart { get; set; }
    public Func<IServiceProvider, CancellationToken, ValueTask>? OnStop { get; set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (OnStart != null) await OnStart(ServiceProvider, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (OnStop != null) await OnStop(ServiceProvider, cancellationToken);
    }
}
