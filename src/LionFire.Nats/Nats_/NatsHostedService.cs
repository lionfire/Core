using Microsoft.Extensions.Hosting;
using NATS.Client.Core;
using Microsoft.Extensions.Logging;

namespace LionFire.Nats_;

public class NatsHostedService : IHostedService
{
    protected NatsConnection nats { get; }
    protected ILogger Logger { get; }

    protected Task? responder = null;
    protected INatsSub<string>? sub = null;

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
        if (sub != null) await sub.UnsubscribeAsync();
        if (responder != null) await responder;
    }
}
