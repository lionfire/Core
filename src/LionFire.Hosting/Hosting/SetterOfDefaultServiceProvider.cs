using LionFire.Dependencies;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Hosting
{
    public class SetterOfDefaultServiceProvider : BackgroundService
    {
        // ENH: Change to IHostedService and unset on shutdown?  Might break things during shutdown

        public SetterOfDefaultServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DependencyContext.Default.ServiceProvider = ServiceProvider;
            return Task.CompletedTask;
        }
    }
}
