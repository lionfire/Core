﻿#nullable enable
using Serilog;
using System;
using Microsoft.Extensions.Configuration;
using Serilog.Formatting;
using System.IO;
using LionFire.Configuration.Logging;
#if OLD
using Serilog.Sinks.Loki;
using Serilog.Sinks.Loki.Labels;
#endif
using LionFire.Hosting;
using Serilog.Sinks.Grafana.Loki;
using Microsoft.Extensions.Hosting;

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
        Console(LionFireSerilogDefaults.LongConsoleTemplate);
        File(); // Writes if unit tests, or if a dir is specified in config
        Loki();

        FromConfiguration();

        TraceListener(); // Listen to Trace, and log

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
        if (Configuration != null)
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
        var SinkName = "Console";
        bool enabled;
        if (!bool.TryParse(Configuration?[LionFireConfigKeys.Log.SinkEnabled(SinkName)] as string, out enabled))
        {
            // Default: enabled
            enabled = true;
        }
        if (!enabled)
        {
#if DEBUG
            System.Console.WriteLine("DEBUG: Console log disabled.");
#endif
            return this;
        }

        LoggerConfiguration.WriteTo.Console(textFormatter ?? LionFireSerilogDefaults.ConsoleTemplate);
        return this;
    }


    public LionFireSerilogBuilder File(ITextFormatter? textFormatter = null, string? path = null, bool throwOnMissingDirConfig = false, string? appName = null)
    {
        var SinkName = "File";
        if (LogBootstrappingState.IsBootstrapping && !LogBootstrappingState.FileLogDuringBootstrap) return this;

        appName ??= AppInfoFromConfiguration.ApplicationNameOrFallback(Configuration);

        bool enabled;

        if (!bool.TryParse(Configuration?[LionFireConfigKeys.Log.SinkEnabled(SinkName)] as string, out enabled))
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
                dir = Configuration?[LionFireConfigKeys.Log.TestDir];
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
            dir ??= Configuration?[LionFireConfigKeys.Log.Dir];

            if (dir == null)
            {
                if (throwOnMissingDirConfig) throw new ArgumentNullException($"Configuration[\"{LionFireConfigKeys.Log.Dir}\"]");
                else
                {
                    var msg = $"Not logging to file, because log dir is missing in configuration: {LionFireConfigKeys.Log.Dir}.  Set {LionFireConfigKeys.Log.Enabled} to false to silence this message.";
                    System.Diagnostics.Debug.WriteLine(msg);
                    System.Console.WriteLine(msg);
                    // TODO: Log via bootstrap logger?
                    return this;
                }
            }

            path = Path.Combine(dir, appName + (LogBootstrappingState.IsBootstrapping ? ".bootstrap" : "") + LionFireSerilogDefaults.DefaultLogFileExtension);
        }

        LoggerConfiguration.WriteTo.File(textFormatter ?? LionFireSerilogDefaults.FileTemplate(appName), path: path);
        return this;
    }

    #region Loki

    /// <summary>
    /// For reference, default loki ports:
    /// - http: 3100
    /// - grpc: 9096
    /// </summary>
    public static string DefaultLokiUrl { get; set; } = "http://localhost:3100";

    //public bool LogStart { get; set; } = true;
    //public bool LogStop { get; set; } = true; // TODO TOIMPLEMENT

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <param name="appName"></param>
    /// <returns></returns>
    /// <remarks>
    /// Recommendation:
    /// 
    /// Enable via environment variable:
    ///    [System.Environment]::SetEnvironmentVariable("LionFire__LionFire__Logging__Loki__Enabled", "true", "User")
    ///    [System.Environment]::SetEnvironmentVariable("LionFire__LionFire__Logging__File__Enabled", "false", "User")
    ///
    /// Disable via environment variable:
    /// 
    ///    [System.Environment]::SetEnvironmentVariable("LionFire__LionFire__Logging__Loki__Enabled", "false", "User")
    ///    [System.Environment]::SetEnvironmentVariable("LionFire__LionFire__Logging__File__Enabled", "true", "User")
    ///    
    /// Grafana query:
    /// 
    ///   {transit="direct"} |= `` | json | line_format "{{.Message}}"
    /// 
    /// </remarks>
    public LionFireSerilogBuilder Loki(string? url = null, string? appName = null)
    {
        appName ??= AppInfoFromConfiguration.ApplicationNameOrFallback(Configuration);

        // TODO: Use gRPC instead of http: https://www.nuget.org/packages/Serilog.Sinks.Loki.gRPC

        var SinkName = "Loki";
        bool enabled;
        if (!bool.TryParse(Configuration?[LionFireConfigKeys.Log.SinkEnabled(SinkName)] as string, out enabled))
        {
            // Default: Disabled
            enabled = false;
        }
        if (!enabled) { return this; }

        LoggerConfiguration.WriteTo.GrafanaLoki(url ?? DefaultLokiUrl, [new LokiLabel { Key = "app", Value = appName }, new LokiLabel { Key = "transit", Value = "direct" }], propertiesAsLabels: ["app"]);

#if false
        //logger.Information("The God of the day is {@God}", "the One and Only");
        // https://github.com/serilog-contrib/serilog-sinks-grafana-loki
        var appsettingsExample = """
            {
              "Serilog": {
                "Using": [
                  "Serilog.Sinks.Grafana.Loki"
                ],
                "MinimumLevel": {
                  "Default": "Debug"
                },
                "WriteTo": [
                  {
                    "Name": "GrafanaLoki",
                    "Args": {
                      "uri": "http://localhost:3100",
                      "labels": [
                        {
                          "key": "app",
                          "value": "web_app"
                        }
                      ],
                      "propertiesAsLabels": [
                        "app"
                      ]
                    }
                  }
                ]
              }
            }
            """;
#endif

#if OLD // Serilog.Sinks.Loki - abandoned library
        LoggerConfiguration.WriteTo.LokiHttp(() => new LokiSinkConfiguration()
        {
            LokiUrl = url ?? DefaultLokiUrl,
            LogLabelProvider = new DefaultLogLabelProvider(),
        });
#endif

        return this;
    }

    #endregion
    //.WriteTo.TestCorrelator()

}