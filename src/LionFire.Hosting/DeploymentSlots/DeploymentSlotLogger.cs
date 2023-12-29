using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace LionFire.Hosting;

public class DeploymentSlotLogger : IHostedService
{
    public DeploymentSlotLogger(ILogger<DeploymentSlotLogger> logger, IConfiguration configuration)
    {
        logger.LogInformation($"Deployment slot: {configuration["slot"]}");
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
