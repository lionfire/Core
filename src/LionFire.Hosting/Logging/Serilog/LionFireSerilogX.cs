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
using LionFire.Configuration.Logging;

namespace LionFire.Hosting;

public static class LionFireSerilogX
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configure"></param>
    /// <param name="configureFallbackToDefaults"></param>
    /// <param name="runForBootstrap"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <remarks>
    /// This gets more complex when Configuration is not available because we have IHostBuilder and not IHostApplicationBuilder.Configuration, so it has to be generated.
    /// 
    /// The Serilog SilentLogger is what you start with.
    /// The Serilog ReloadableLogger is the bootstrap logger Serilog gives you after calling CreateBootstrapLogger.
    /// Once the real logger is created, it is frozen.
    /// </remarks>
    public static ILionFireHostBuilder Serilog(this ILionFireHostBuilder builder, Action<LionFireSerilogBuilder>? configure = null, bool configureFallbackToDefaults = true, bool runForBootstrap = false)
    {
        if (!runForBootstrap
            && global::Serilog.Log.Logger != null
            //&& global::Serilog.Log.Logger.GetType().Name != "ReloadableLogger" // REVIEW
            && global::Serilog.Log.Logger.GetType().Name != "SilentLogger"
            )
        {
            if (configure != null && !LogBootstrappingState.IsBootstrapping) throw new NotImplementedException("Serilog already configured.  TODO: Allow configuring existing builder.");
            // else we are in LionFireHostBuilder.GetLogBootstrapConfiguration and not really setting up Serilog, just getting a LionFireHostBuilder to use its Configuration because the program is IHostBuilder based.  DEPRECATED: IHostBuilder is deprecated because IHostApplicationBuilder is preferred so we don't have to do this to get a Configuration.
            return builder;
        }
        //if (global::Serilog.Log.Logger is not Silent null && ) { return builder; }

        bool alreadyBootstrapping = LogBootstrappingState.IsBootstrapping;
        LogBootstrappingState.IsBootstrapping = true;

        #region Configuration

        if (configureFallbackToDefaults) configure ??= lionFireSerilogBuilder => lionFireSerilogBuilder.Defaults();
        var Configuration = builder.GetConfigurationForLogBootstrap(new LionFireHostBuilderBootstrapOptions { UseDefaults = true });

        #endregion

        #region Bootstrap

        global::Serilog.Log.Logger = createReloadableLogger(configure, Configuration);

        #endregion

        #region Log Start

        var logStart = bool.Parse(Configuration[LionFireConfigKeys.Log.LogStart] ?? "true");
        if (logStart == true && !LogBootstrappingState.State.Value!.HasLoggedStart)
        {
            LogBootstrappingState.State.Value!.HasLoggedStart = true;
            global::Serilog.Log.Logger.Information($"----- {AppInfoFromConfiguration.ApplicationNameOrFallback(Configuration)} START @ {DateTime.Now.ToShortTimeString()} -----");
        }
        if (!alreadyBootstrapping)
        {
            global::Serilog.Log.Logger.Debug($"Host type: {builder.UnderlyingHostType?.FullName} ");
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
                LogBootstrappingState.IsBootstrapping = false;

                configure?.Invoke(new LionFireSerilogBuilder(loggerConfiguration, context?.Configuration));

                global::Serilog.Log.Logger.Verbose($"Log bootstrapping finished.");
            }, writeToProviders: true);
        });
#endif

        return builder;

        #region Local Functions

        static ReloadableLogger createReloadableLogger(Action<LionFireSerilogBuilder>? configure, Microsoft.Extensions.Configuration.IConfiguration Configuration)
        {
            var bootstrapLoggerConfiguration = new LoggerConfiguration();

            configure?.Invoke(new LionFireSerilogBuilder(bootstrapLoggerConfiguration, Configuration));

            return bootstrapLoggerConfiguration.CreateBootstrapLogger();
        }

        #endregion
    }
}
