﻿using LionFire.Configuration.Hosting;
using LionFire.Structures.Keys;
using LionFire.Types.Scanning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Diagnostics;
using System.IO;

namespace LionFire.Hosting;

public static class DefaultsX
{

    /// <summary>
    /// Add LionFire defaults for IHostBuilder
    /// </summary>
    public static ILionFireHostBuilder Defaults(this ILionFireHostBuilder lf)
    {
        bool isTest = TestInfo.IsTest == true;

        var builder = lf.HostBuilder;

        builder.UseContentRoot(AppContext.BaseDirectory);

        #region Configuration

        #region appsettings.json

        builder.ConfigureAppConfiguration((context, config) => config
            //.SetBasePath(AppContext.BaseDirectory)
            //.SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"), true, true)
       );

        // FUTURE: Config parent dir?  For things like /etc/lionfire/{appName}/appsettings.json

        #endregion

        #region appsettings.{Environment}.json, appsettings.test.json, env: LionFire_*, copy example settings

        if (lf.HostBuilder.WrappedHostBuilder != null)
        {
            lf.HostBuilder.WrappedHostBuilder.ConfigureHostConfiguration(c =>
            {
                if(isTest) c.AddJsonFile("appsettings.test.json", optional: true, reloadOnChange: false);

                c.AddEnvironmentVariables(prefix: "LionFire_");
            });

            lf.HostBuilder.WrappedHostBuilder.ConfigureAppConfiguration(c =>
            {
                if (isTest) c.AddJsonFile("appsettings.test.json", optional: true, reloadOnChange: false);

                c.AddEnvironmentVariables(prefix: "LionFire_");
            });
            lf.HostBuilder.WrappedHostBuilder
                .ReleaseChannel() // adds appsettings.{ReleaseChannel}.json
                .CopyExampleAppSettings()
                ;
        }
        else if (lf.HostBuilder.WrappedHostApplicationBuilder != null)
        {
            var c = lf.HostBuilder.WrappedHostApplicationBuilder.Configuration;

            if (isTest) c.AddJsonFile("appsettings.test.json", optional: true, reloadOnChange: false);
            c.AddEnvironmentVariables(prefix: "LionFire_");

            lf.HostBuilder.WrappedHostApplicationBuilder
                .ReleaseChannel() // adds appsettings.{Environment}.json
                .CopyExampleAppSettings()
                ;
        }
        else
        {
            throw new ArgumentNullException("lf.HostBuilder must have WrappedHostApplicationBuilder or WrappedHostBuilder");
        }

        #endregion

        // TODO
        //   IHostEnvironment env = hostingContext.HostingEnvironment;
        //    if (env.IsDevelopment() && env.ApplicationName is { Length: > 0 })
        //    {
        //        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
        //        if (appAssembly is not null)
        //        {
        //            config.AddUserSecrets(appAssembly, optional: true, reloadOnChange: reloadOnChange);
        //        }
        //    }

        // Configured supplemental AppSettings file
        builder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            var appSettingsFile = hostingContext.Configuration[ConfigurationKeys.AppSettingsFileKey];
            if (appSettingsFile != null)
            {
                Console.WriteLine($"Adding config source: {appSettingsFile}  (exists: {File.Exists(appSettingsFile)})");
                config
                      .AddJsonFile(appSettingsFile, optional: true, reloadOnChange: true);
            }
        });

        #endregion

        lf.Serilog();

        lf.HostBuilder.UseDependencyContext();

        lf.ConfigureServices(services =>
        {
            services
                .AddSingleton<TypeScanner>()
                .Configure<TypeScannerOptions>(o =>
                {
                    o.DllPrefixWhitelist.Add("LionFire.");
                })
                .AddSingleton<IKeyProviderService<string>, KeyProviderService<string>>();
        });

        //Console.WriteLine($"Current Directory: {Environment.CurrentDirectory}");
        //Console.WriteLine($"{Path.Combine(Environment.CurrentDirectory, "appsettings.json")} exists? {File.Exists(Path.Combine(Environment.CurrentDirectory, "appsettings.json"))}");
        //Console.WriteLine($"{Path.Combine(AppContext.BaseDirectory, "appsettings.json")} exists? {File.Exists(Path.Combine(AppContext.BaseDirectory, "appsettings.json"))}");

        // ----------------

        // Reference: Microsoft.Extensions.Hosting version 6
        // https://github.com/dotnet/runtime/blob/8048fe613933a1cd91e3fad6d571c74f726143ef/src/libraries/Microsoft.Extensions.Hosting/src/HostingHostBuilderExtensions.cs

        //builder.UseContentRoot(Directory.GetCurrentDirectory()); // Overridden 
        //builder.ConfigureHostConfiguration(config =>
        //{
        //    config.AddEnvironmentVariables(prefix: "DOTNET_");
        //    if (args is { Length: > 0 })
        //    {
        //        config.AddCommandLine(args);
        //    }
        //});

        //builder.ConfigureAppConfiguration((hostingContext, config) =>
        //{
        //    IHostEnvironment env = hostingContext.HostingEnvironment;
        //    bool reloadOnChange = GetReloadConfigOnChangeValue(hostingContext);

        //    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: reloadOnChange)
        //            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: reloadOnChange);

        //    if (env.IsDevelopment() && env.ApplicationName is { Length: > 0 })
        //    {
        //        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
        //        if (appAssembly is not null)
        //        {
        //            config.AddUserSecrets(appAssembly, optional: true, reloadOnChange: reloadOnChange);
        //        }
        //    }

        //    config.AddEnvironmentVariables();

        //    if (args is { Length: > 0 })
        //    {
        //        config.AddCommandLine(args);
        //    }
        //})
        //            .ConfigureLogging((hostingContext, logging) =>
        //            {
        //                bool isWindows =
        //#if NET6_0_OR_GREATER
        //                    OperatingSystem.IsWindows();
        //#else
        //                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        //#endif

        //                // IMPORTANT: This needs to be added *before* configuration is loaded, this lets
        //                // the defaults be overridden by the configuration.
        //                if (isWindows)
        //                {
        //                    // Default the EventLogLoggerProvider to warning or above
        //                    logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
        //                }

        //                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        //#if NET6_0_OR_GREATER
        //                if (!OperatingSystem.IsBrowser())
        //#endif
        //                {
        //                    logging.AddConsole();
        //                }
        //                logging.AddDebug();
        //                logging.AddEventSourceLogger();

        //                if (isWindows)
        //                {
        //                    // Add the EventLogLoggerProvider on windows machines
        //                    logging.AddEventLog();
        //                }

        //                logging.Configure(options =>
        //                {
        //                    options.ActivityTrackingOptions =
        //                        ActivityTrackingOptions.SpanId |
        //                        ActivityTrackingOptions.TraceId |
        //                        ActivityTrackingOptions.ParentId;
        //                });

        //            })
        //            .UseDefaultServiceProvider((context, options) =>
        //            {
        //                bool isDevelopment = context.HostingEnvironment.IsDevelopment();
        //                options.ValidateScopes = isDevelopment;
        //                options.ValidateOnBuild = isDevelopment;
        //            });

        return lf;

    }
}
