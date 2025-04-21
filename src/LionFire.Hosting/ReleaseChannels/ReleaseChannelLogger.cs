using LionFire.Deployment;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Hosting;


public class ReleaseChannelLogger(ILogger<ReleaseChannelLogger> logger, IConfiguration configuration) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var id = configuration["ReleaseChannel"];

        var releaseChannel = ReleaseChannels.TryParse(id);
        if (releaseChannel == null)
        {
            logger.LogInformation($"Release channel: \"{configuration["ReleaseChannel"]}\"");
        }
        else
        {
            logger.LogInformation($"Release channel: {releaseChannel}");
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
