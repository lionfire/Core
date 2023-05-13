using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
        using Splat.Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace LionFire.Hosting;

public static class ReactiveUIHostingX
{
    public static IServiceCollection UseMicrosoftDIForReactiveUI(this IServiceCollection services)
    {
        services.UseMicrosoftDependencyResolver();
        var resolver = Locator.CurrentMutable;
        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();


        services.AddHostedService<RegisterILoggerFactoryWithSplat>();

        //Locator.CurrentMutable.UseSerilogFullLogger();

        return services;
    }
}

public class RegisterILoggerFactoryWithSplat : IHostedService
{
    public RegisterILoggerFactoryWithSplat(ILoggerFactory loggerFactory)
    {
        Locator.CurrentMutable.UseMicrosoftExtensionsLoggingWithWrappingFullLogger(loggerFactory);
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}