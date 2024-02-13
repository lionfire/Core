using Microsoft.Extensions.Logging;
using System.Threading;

namespace LionFire.Hosting;

//public class AppContextBaseDirectoryLogger : IHostedService
//{
//    public AppContextBaseDirectoryLogger(ILogger<AppContextBaseDirectoryLogger> logger)
//    {
//        logger.LogInformation("AppContext.BaseDirectory: {BaseDirectory}", AppContext.BaseDirectory);
//    }

//    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

//    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
//}

public class AppContextLogger(ILogger<AppContextLogger> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("AppContext.BaseDirectory: {BaseDirectory}", AppContext.BaseDirectory);
        logger.LogInformation("AppContext.TargetFrameworkName: {TargetFrameworkName}", AppContext.TargetFrameworkName);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

