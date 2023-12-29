using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public class AppContextLogger(ILogger<ReleaseChannelLogger> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"AppContext.BaseDirectory: {AppContext.BaseDirectory}");
        logger.LogInformation($"AppContext.TargetFrameworkName: {AppContext.TargetFrameworkName}");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

public class ReleaseChannelLogger(ILogger<ReleaseChannelLogger> logger, IConfiguration configuration) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"Release channel: {configuration["releaseChannel"]}");        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
