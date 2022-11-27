using LionFire.Structures.Keys;
using LionFire.Types.Scanning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace LionFire.Hosting
{
    public static class DefaultsExtensions
    {
        /// <summary>
        /// Add LionFire defaults for IHostBuilder
        /// </summary>
        public static LionFireHostBuilder Defaults(this LionFireHostBuilder lf)
        {
            var builder = lf.HostBuilder;

            lf
                .ReleaseChannel()
                .CopyExampleAppSettings()
                .Log()
                ;

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

            //builder.ConfigureLogging((context, logger) => logger
            //    .AddLionFireNLog(context.Configuration.GetSection("Logging"), null)
            //);

            builder.UseContentRoot(AppContext.BaseDirectory);
            builder.ConfigureAppConfiguration((context, config) => config
                //.SetBasePath(AppContext.BaseDirectory)
                //.SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"), true, true)
           );

            //Console.WriteLine($"Current Directory: {Environment.CurrentDirectory}");
            //Console.WriteLine($"{Path.Combine(Environment.CurrentDirectory, "appsettings.json")} exists? {File.Exists(Path.Combine(Environment.CurrentDirectory, "appsettings.json"))}");
            //Console.WriteLine($"{Path.Combine(AppContext.BaseDirectory, "appsettings.json")} exists? {File.Exists(Path.Combine(AppContext.BaseDirectory, "appsettings.json"))}");

            builder.ConfigureAppConfiguration((hostingContext, config) =>
             {
                 var appSettingsFile = hostingContext.Configuration["AppSettingsFile"];
                 if (appSettingsFile != null)
                 {
                     Console.WriteLine($"Adding config source: {appSettingsFile}  (exists: {File.Exists(appSettingsFile)})");
                     config
                           .AddJsonFile(appSettingsFile, optional: true, reloadOnChange: true);
                 }
             });

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
}
