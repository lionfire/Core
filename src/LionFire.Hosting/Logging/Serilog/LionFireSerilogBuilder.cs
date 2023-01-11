#nullable enable
using Serilog;
using System;
using Microsoft.Extensions.Configuration;
using Serilog.Formatting;
using System.IO;
using LionFire.Configuration.Logging;
using Serilog.Sinks.Loki;
using Serilog.Sinks.Loki.Labels;
using LionFire.Hosting;

namespace LionFire.Logging.Serilog;

public class LionFireSerilogBuilder
{
    public LoggerConfiguration LoggerConfiguration { get; }
    public IConfiguration? Configuration { get; }

    public LionFireSerilogBuilder(LoggerConfiguration loggerConfiguration, IConfiguration? configuration)
    {
        LoggerConfiguration = loggerConfiguration;
        Configuration = configuration;

        if (configuration != null) { LoggerConfiguration.ReadFrom.Configuration(configuration); }
    }
    public LionFireSerilogBuilder Defaults()
    {
        DefaultEnrich();
        Console();
        File();
        Loki();

        FromConfiguration();
        
        TraceListener();

        return this;
    }

    private static bool AttachedToGlobalLogger = false;
    public LionFireSerilogBuilder TraceListener()
    {
        if (!AttachedToGlobalLogger)
        {
            AttachedToGlobalLogger = true;
            var listener = new SerilogTraceListener.SerilogTraceListener();
            System.Diagnostics.Trace.Listeners.Add(listener);
        }

        return this;
    }

    public LionFireSerilogBuilder FromConfiguration()
    {
        if(Configuration != null)
        {
            LoggerConfiguration.ReadFrom.Configuration(Configuration);
        }
        return this;
    }

    public LionFireSerilogBuilder DefaultEnrich()
    {
        LoggerConfiguration.Enrich.FromLogContext();
        return this;
    }

    public LionFireSerilogBuilder Debug(ITextFormatter? textFormatter = null)
    {
        LoggerConfiguration.WriteTo.Debug(textFormatter ?? LionFireSerilogDefaults.DebugTemplate);
        return this;
    }

    public LionFireSerilogBuilder Console(ITextFormatter? textFormatter = null)
    {
        LoggerConfiguration.WriteTo.Console(textFormatter ?? LionFireSerilogDefaults.ConsoleTemplate);
        return this;
    }

    public LionFireSerilogBuilder File(ITextFormatter? textFormatter = null, string? path = null, bool throwOnMissingDirConfig = false, string? appName = null)
    {
        if (LogBootstrappingState.IsBootstrapping && !LogBootstrappingState.FileLogDuringBootstrap) return this;

        appName ??= AppInfoFromConfiguration.ApplicationNameOrFallback(Configuration);

        bool enabled;

        if (!bool.TryParse(Configuration?[LionFireConfigKeys.Log.Enabled] as string, out enabled))
        {
            // Default: Enabled
            enabled = true;
        }

        if (!enabled) return this;

        if (path == null)
        {
            string? dir = null;
            if (LionFireEnvironment.IsUnitTest == true)
            {
                dir = Configuration?[LionFireConfigKeys.Log.TestDir] as string;
                if (dir != null)
                {
                    var testDllName = AppInfoFromConfiguration.TestDllName;
                    if (testDllName != null)
                    {
                        var suffix = ".Tests";
                        if (testDllName.EndsWith(".Tests")) testDllName = testDllName.Substring(0, testDllName.Length - suffix.Length);
                        dir = Path.Combine(dir, testDllName);
                    }
                }
            }
            dir ??= Configuration?[LionFireConfigKeys.Log.Dir] as string;

            if (dir == null)
            {
                if (throwOnMissingDirConfig) throw new ArgumentNullException($"Configuration[\"{LionFireConfigKeys.Log.Dir}\"]");
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Not logging to file, because log dir is missing in configuration: {LionFireConfigKeys.Log.Dir}.  Set {LionFireConfigKeys.Log.Enabled} to false to silence this message.");
                    return this;
                }
            }

            path = Path.Combine(dir, appName + (LogBootstrappingState.IsBootstrapping ? ".bootstrap" : "") + LionFireSerilogDefaults.DefaultLogFileExtension);
        }

        LoggerConfiguration.WriteTo.File(textFormatter ?? LionFireSerilogDefaults.FileTemplate(appName), path: path);
        return this;
    }

    #region Loki

    public static string DefaultLokiUrl { get; set; } = "http://localhost:3100";
    public bool LogStart { get; set; } = true;
    public bool LogStop { get; set; } = true; // TODO TOIMPLEMENT

    public LionFireSerilogBuilder Loki(string? url = null)
    {
        LoggerConfiguration.WriteTo.LokiHttp(() => new LokiSinkConfiguration()
        {
            LokiUrl = url ?? DefaultLokiUrl,
            LogLabelProvider = new DefaultLogLabelProvider(),
        });
        return this;
    }

    #endregion
    //.WriteTo.TestCorrelator()

}