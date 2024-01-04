using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using LionFire.Dependencies;

namespace LionFire.Hosting;

// ENH: Support multiple run actions, in serial or parallel

public class RunTaskAndShutdownApplication : BackgroundService
{
    readonly IServiceProvider serviceProvider;
    private readonly RunOptions options;
    readonly IHostApplicationLifetime hostApplicationLifetime;

    public RunTaskAndShutdownApplication(IServiceProvider serviceProvider, RunOptions options, IHostApplicationLifetime hostApplicationLifetime)
    {

        // OLD - now use DependencyContextServiceProviderFactoryWrapper
        // : Use a custom Container factory that is just a wrapper around the default Microsoft (or some other like Autofac) Container provider
        //DependencyContext.Default.ServiceProvider = serviceProvider;

        this.serviceProvider = serviceProvider;
        this.options = options;
        this.hostApplicationLifetime = hostApplicationLifetime;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (options?.Action != null)
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await options.Action(serviceProvider, stoppingToken).ConfigureAwait(false);
                }
                finally
                {
                    if (options.StopApplicationAfterRun)
                    {
                        hostApplicationLifetime.StopApplication();
                    }
                }
            }, TaskCreationOptions.AttachedToParent);
        }
        return Task.CompletedTask;
    }
}
