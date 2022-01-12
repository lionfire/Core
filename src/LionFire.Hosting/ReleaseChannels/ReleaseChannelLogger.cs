using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Hosting
{
    public class ReleaseChannelLogger : IHostedService
    {
        public ReleaseChannelLogger(ILogger<ReleaseChannelLogger> logger, IConfiguration configuration)
        {
            logger.LogInformation($"Release channel: {configuration["releaseChannel"]}");
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken)=> Task.CompletedTask;
    }
}
