#nullable enable
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
    public static ILionFireHostBuilder Serilog(this ILionFireHostBuilder builder, Action<LionFireSerilogBuilder>? configure = null, bool configureFallbackToDefaults = true)
    {
        if (global::Serilog.Log.Logger != null && global::Serilog.Log.Logger.GetType().Name != "SilentLogger")
        {
            if (configure != null) throw new NotImplementedException("Serilog already configured.  TODO: Allow configuring existing builder.");
            return builder;
        }
        //if (global::Serilog.Log.Logger is not Silent null && ) { return builder; }

        LogBootstrappingState.IsBootstrapping = true;

        if (configureFallbackToDefaults) configure ??= lionFireSerilogBuilder => lionFireSerilogBuilder.Defaults();

        #region Bootstrap

        var configuration = builder.GetBootstrapConfiguration(new LionFireHostBuilderBootstrapOptions
        {
            BootstrapLionFireHostBuilder = lf => lf.Serilog(configure, configureFallbackToDefaults),
            UseDefaults = true
        });

        var bootstrapLoggerConfiguration = new LoggerConfiguration();
        //bootstrapLoggerConfiguration.ReadFrom.Configuration(configuration); // REVIEW - needed?

        var bootstrapBuilder = new LionFireSerilogBuilder(bootstrapLoggerConfiguration, configuration);

        configure?.Invoke(bootstrapBuilder);

        global::Serilog.Log.Logger = bootstrapLoggerConfiguration.CreateBootstrapLogger();

        if (bootstrapBuilder?.LogStart == true && !LogBootstrappingState.State.Value!.HasLoggedStart)
        {
            LogBootstrappingState.State.Value!.HasLoggedStart = true;
            global::Serilog.Log.Logger.Information($"----- {AppInfoFromConfiguration.ApplicationNameOrFallback(configuration)} START @ {DateTime.Now.ToShortTimeString()} -----"); // TODO - don't log if option is off
        }

        #endregion

#if OLD
        builder.HostBuilder.UseSerilog((context, serviceProvider, loggerConfiguration) =>
        {
            //loggerConfiguration.ReadFrom.Configuration(configuration); // REVIEW - needed?

            LogBootstrappingState.IsBootstrapping = false;
            global::Serilog.Log.Logger.Verbose($"----- BOOTSTRAP FINISHED -----");
            configure?.Invoke(new LionFireSerilogBuilder(loggerConfiguration, context?.Configuration));
        }, preserveStaticLogger: true);
#else
        builder.ConfigureServices((context, services) => // Avoid HostBuilder
        {
            services.AddSerilog((serviceProvider, loggerConfiguration) =>
            {
                //loggerConfiguration.ReadFrom.Configuration(configuration); // REVIEW - needed?

                LogBootstrappingState.IsBootstrapping = false;
                global::Serilog.Log.Logger.Verbose($"----- BOOTSTRAP FINISHED -----");
                configure?.Invoke(new LionFireSerilogBuilder(loggerConfiguration, context?.Configuration));
            });
        });
#endif


        return builder;
    }
}
