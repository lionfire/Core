#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace LionFire.Hosting;

public static class HostApplicationBuilderX
{
    /// <summary>
    /// Fluent builder for LionFire's initialization of HostApplicationBuilder
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="action"></param>
    /// <param name="useDefaults"></param>
    /// <returns></returns>
    public static HostApplicationBuilder LionFire(this HostApplicationBuilder hostBuilder, Action<ILionFireHostBuilder>? action = null, bool useDefaults = true)
    {
        var lf = new LionFireHostBuilder(hostBuilder); // TODO - reuse existing from Properties if it exists

        if (useDefaults) { lf.Defaults(); }

        action?.Invoke(lf);

        return hostBuilder;
    }
    public static HostApplicationBuilder LionFire(this HostApplicationBuilder hostBuilder, int basePort, Action<ILionFireHostBuilder>? action = null, bool useDefaults = true) 
        => hostBuilder.BasePort(basePort).LionFire(action, useDefaults);

    // FUTURE - sort out IHostApplicationBuilder vs implementors such as HostApplicationBuilder (and nothing else so far?)
    //public static IHostApplicationBuilder LionFire(this IHostApplicationBuilder hostBuilder, int basePort, Action<ILionFireHostBuilder>? action = null, bool useDefaults = true)
    //{
    //    hostBuilder.LionFire(basePort, action, useDefaults);
    //    return hostBuilder;
    //}

    #region UseConsoleLifetime

    public static HostApplicationBuilder UseConsoleLifetime(this HostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services
            .AddSingleton<IHostLifetime, Microsoft.Extensions.Hosting.Internal.ConsoleLifetime>()
             ;
        return hostApplicationBuilder;
    }
    public static HostApplicationBuilder UseConsoleLifetime(this HostApplicationBuilder hostApplicationBuilder, Action<ConsoleLifetimeOptions> configureOptions)
    {
        hostApplicationBuilder.Services
            .AddSingleton<IHostLifetime, Microsoft.Extensions.Hosting.Internal.ConsoleLifetime>()
             .Configure(configureOptions)
             ;
        return hostApplicationBuilder;
    }

    #endregion
        
}
