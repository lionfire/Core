using LionFire.Applications;
using LionFire.ExtensionMethods.Dumping;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;

namespace LionFire.Hosting;

public class AppInfoLogger : IHostedService
{
    public AppInfoLogger(IServiceProvider serviceProvider, ILogger<AppInfoLogger> logger)
    {
        {
            AppInfo? appInfo = serviceProvider.GetService<AppInfo>();

            if (appInfo == null)
            {
                logger.LogDebug("AppInfo: none.");
            }
            else
            {
                logger.LogInformation("AppInfo: {appInfo}", appInfo.Dump());
            }
        }

        {
            AppInfo? appInfo = serviceProvider.GetService<IOptionsSnapshot<AppInfo>>()?.Value;

            if (appInfo == null)
            {
                logger.LogDebug("AppInfo2: none.");
            }
            else
            {
                logger.LogInformation("AppInfo2: {appInfo}", appInfo.Dump());
            }
        }
                logger.LogInformation("AppContext.BaseDirectory: {BaseDirectory}", AppContext.BaseDirectory);
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
