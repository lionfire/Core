using LionFire.ExtensionMethods.Dumping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LionFire.AspNetCore;

public class WebHostConfigLogger<TConfig> : IHostedService
    where TConfig : WebHostConfig
{
    public IConfiguration Configuration { get; }
    public ILogger<WebHostConfigLogger<TConfig>> Logger { get; }

    public WebHostConfigLogger(IConfiguration configuration, ILogger<WebHostConfigLogger<TConfig>> logger)
    {
        Configuration = configuration;
        Logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
         var c = (TConfig)Activator.CreateInstance(typeof(TConfig), Configuration)!;

        Logger.LogInformation("WebHostConfig:" + Environment.NewLine + c.DumpProperties());

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
