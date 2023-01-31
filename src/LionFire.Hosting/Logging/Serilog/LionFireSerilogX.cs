﻿#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Hosting;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Logging.Serilog;
using System.Threading;

namespace LionFire.Hosting;

public static class LionFireSerilogX
{
    public static ILionFireHostBuilder Serilog(this ILionFireHostBuilder b, Action<LionFireSerilogBuilder>? configure = null, bool configureFallbackToDefaults = true)
    {
        LogBootstrappingState.IsBootstrapping = true;

        if (configureFallbackToDefaults) configure ??= lionFireSerilogBuilder => lionFireSerilogBuilder.Defaults();

        #region Bootstrap

        var configuration = b.GetBootstrapConfiguration(new LionFireHostBuilderBootstrapOptions
        {
            BootstrapLionFireHostBuilder = lf => lf.Serilog(configure, configureFallbackToDefaults),
            UseDefaults = true
        });

        var bootstrapLoggerConfiguration = new LoggerConfiguration();
        var bootstrapBuilder = new LionFireSerilogBuilder(bootstrapLoggerConfiguration, configuration);

        configure?.Invoke(bootstrapBuilder);

        global::Serilog.Log.Logger = bootstrapLoggerConfiguration.CreateBootstrapLogger();

        if (bootstrapBuilder?.LogStart == true && !LogBootstrappingState.State.Value!.HasLoggedStart)
        {
            LogBootstrappingState.State.Value!.HasLoggedStart = true;
            global::Serilog.Log.Logger.Information($"----- {AppInfoFromConfiguration.ApplicationNameOrFallback(configuration)} START @ {DateTime.Now.ToShortTimeString()} -----"); // TODO - don't log if option is off
        }

        #endregion

        b.HostBuilder.UseSerilog((context, serviceProvider, loggerConfiguration) =>
        {
            LogBootstrappingState.IsBootstrapping = false;
            global::Serilog.Log.Logger.Verbose($"----- BOOTSTRAP FINISHED -----");
            configure?.Invoke(new LionFireSerilogBuilder(loggerConfiguration, context?.Configuration));
        }, preserveStaticLogger: true);

        return b;
    }
}