using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using LionFire.DependencyInjection;

namespace LionFire.Hosting
{

    public class RunTaskAndStopApplication : BackgroundService
    {
        readonly IServiceProvider serviceProvider;
        private readonly RunOptions options;
        readonly IApplicationLifetime hostApplicationLifetime;

        public RunTaskAndStopApplication(IServiceProvider serviceProvider, RunOptions options, IApplicationLifetime hostApplicationLifetime) // TODO NETCORE3 - IHostApplicationLifetime
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
                        hostApplicationLifetime.StopApplication();
                    }
                }, TaskCreationOptions.AttachedToParent);
            }
            return Task.CompletedTask;
        }
    }
}
