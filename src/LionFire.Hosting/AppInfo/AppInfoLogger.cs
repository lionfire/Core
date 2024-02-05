using LionFire.Applications;
using LionFire.ExtensionMethods.Dumping;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace LionFire.Hosting;

public class AppInfoLogger : IHostedService
{
    public AppInfoLogger(AppInfo appInfo, ILogger<AppInfoLogger> logger)
    {
        logger.LogInformation("AppInfo: {appInfo}", appInfo.Dump());
        logger.LogInformation("AppContext.BaseDirectory: {BaseDirectory}", AppContext.BaseDirectory);
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
