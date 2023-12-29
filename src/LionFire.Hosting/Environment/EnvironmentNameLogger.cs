using Microsoft.Extensions.Logging;
using System.Threading;

namespace LionFire.Hosting;

public class EnvironmentNameLogger : IHostedService
{
    public EnvironmentNameLogger(ILogger<EnvironmentNameLogger> logger, IHostEnvironment env)
    {
        logger.LogInformation($"Environment Name: {env.EnvironmentName}");
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
