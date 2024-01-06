using Microsoft.Extensions.Hosting;
using NATS.Client.Core;
using Microsoft.Extensions.Logging;

namespace LionFire.Nats_;

public class NatsHostedService : IHostedService
{
    protected NatsConnection nats { get; }
    protected ILogger Logger { get; }

    protected List<Task> responders = new();
    //protected INatsSub<string>? sub = null;

    protected List<IAsyncDisposable> disposables = new();

    public NatsHostedService(NatsConnection natsConnection, ILogger logger)
    {
        nats = natsConnection;
        Logger = logger;
    }

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async virtual Task StopAsync(CancellationToken cancellationToken)
    {
        //if (sub != null) await sub.UnsubscribeAsync();

        foreach (var d in disposables)
        {
            await d.DisposeAsync();
        }
        foreach (var r in responders)
        {
            await r;
        }

        //if (responder != null) await responder;
    }
}
