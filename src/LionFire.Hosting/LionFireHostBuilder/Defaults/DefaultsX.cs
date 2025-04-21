using LionFire.Applications;
using LionFire.Configuration.Hosting;
using LionFire.ExtensionMethods;
using LionFire.Structures.Keys;
using LionFire.Types.Scanning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace LionFire.Hosting;

public static class DefaultsX
{
    #region Configuration

    public static bool LogConfigFile = true;

    #endregion

    /// <summary>
    /// Add LionFire defaults for IHostBuilder
    /// </summary>
    public static ILionFireHostBuilder Defaults(this ILionFireHostBuilder lf)
    {
        var builder = lf.HostBuilder;

        #region Environment

        builder.ConfigureHostConfiguration(config =>
        {
            config.AddEnvironmentVariables(prefix: "DOTNET_"); 
            config.AddEnvironmentVariables(prefix: "LionFire__"); 

            var env = Environment.GetEnvironmentVariable("DOTNET_Environment");
            if (env is { Length: > 0 })
            {
                config.AddInMemoryCollection([new KeyValuePair<string, string?>(HostDefaults.EnvironmentKey, env)]);
            }
#if REVIEW
            var args = Environment.CommandLine.Split((string?)null,StringSplitOptions.RemoveEmptyEntries);
            if (args is { Length: > 0 })
            {
                config.AddCommandLine(args);
            }
#endif
        });

        #endregion

        lf.TrySetDefaultAppInfo();

        lf.ConfigureServices(s => s
            .AddHostedService<AppInfoLogger>()
            .AddHostedService<AppContextLogger>()
            .AddHostedService<AssemblyVersionLogger>()
            .Configure<AssemblyVersionLoggerOptions>(o =>
            {
                o.PrefixWhitelist.Add("LionFire");

                var assemblyName = Assembly.GetEntryAssembly()?.FullName;
                var index = assemblyName?.IndexOf('.');
                if (assemblyName != null && index.HasValue && index.Value > 0)
                {
                    o.PrefixWhitelist.Add(assemblyName.Substring(0, index.Value));
                }
            })
            .AddFilesystemResilience()
        );

        bool reloadOnChange = true;
        bool isTest = LionFireEnvironment.IsUnitTest == true;


        builder.UseContentRoot(AppContext.BaseDirectory); // REVIEW - compare with Microsoft default behavior, eliminate if not needed, or else document more

        builder.ConfigureServices(s => s.AddHostedService<EnvironmentNameLogger>());

        #region Configuration

        #region appsettings.json

        var configDirFromEnv = Environment.GetEnvironmentVariable($"DOTNET_ConfigDir");
        var configDir = configDirFromEnv ?? Environment.CurrentDirectory;

        var configFileFromEnv = Environment.GetEnvironmentVariable($"DOTNET_ConfigFile");
        var appSettingsPath = configFileFromEnv ?? Path.Combine(configDir, "appsettings.json");

        builder.ConfigureAppConfiguration((context, config) => config
            //.SetBasePath(AppContext.BaseDirectory)
            //.SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile(appSettingsPath, true, true)
        );

        if (LogConfigFile)
        {
            if (File.Exists(appSettingsPath))
            {
#if DEBUG
                //Console.WriteLine($"DEBUG: Using configuration: {appSettingsPath}");
#endif
            }
            else
            {
                Console.WriteLine($"MISSING configuration: {appSettingsPath}");
            }
        }

        // FUTURE: Config parent dir?  For things like /etc/lionfire/{appName}/appsettings.json

        #endregion

        #region appsettings.{Environment}.json, appsettings.test.json, env: LionFire__*, copy example settings

        if (lf.HostBuilder.WrappedHostBuilder != null)
        {
            lf.HostBuilder.WrappedHostBuilder.ConfigureHostConfiguration(c =>
            {
                // REVIEW: releaseChannel is set to test, but this lets us change releaseChannel to something else while still using settings for testing
                if (isTest) c.AddJsonFile("appsettings.test.json", optional: true, reloadOnChange: false);

                c.AddEnvironmentVariables(prefix: "LionFire__");

            });

            #region AddUserSecrets
            lf.HostBuilder.WrappedHostBuilder.ConfigureAppConfiguration((context, config) =>
            {
                IHostEnvironment env = context.HostingEnvironment;
                if (env.IsDevelopment() && env.ApplicationName is { Length: > 0 })
                {
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    if (appAssembly is not null)
                    {
                        config.AddUserSecrets(appAssembly, optional: true, reloadOnChange: false);
                    }
                }
            });
            #endregion

            lf.HostBuilder.WrappedHostBuilder
                .ReleaseChannel() // adds appsettings.{ReleaseChannel}.json
                .DeploymentSlot() // adds appsettings.slot.{slot}.json
                .CopyExampleAppSettings()
                ;
        }
        else if (lf.HostBuilder.WrappedIHostApplicationBuilder != null)
        {
            // REFACTOR - is there a better way to avoid this duplicated code?

            var config = lf.HostBuilder.WrappedIHostApplicationBuilder.Configuration;

            if (isTest) config.AddJsonFile("appsettings.test.json", optional: true, reloadOnChange: false);
            config.AddEnvironmentVariables(prefix: "LionFire__");

            lf.HostBuilder.WrappedIHostApplicationBuilder
                .ReleaseChannel() // adds appsettings.{releaseChannel}.json
                .DeploymentSlot() // adds appsettings.slot.{slot}.json
                .TryAddConfigForDocker() // adds /app/conf/appsettings.json
                //.ConfigureServices(s => s.AddHostedService<AppContextLogger>())
                .CopyExampleAppSettings()
                ;

            #region AddUserSecrets
            IHostEnvironment env = lf.HostBuilder.WrappedIHostApplicationBuilder.Environment;
            if (env.IsDevelopment() && env.ApplicationName is { Length: > 0 })
            {
                try
                {
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    if (appAssembly is not null)
                    {
                        config.AddUserSecrets(appAssembly, optional: true, reloadOnChange: false);
                    }
                }
                catch (FileNotFoundException) { }
            }
            #endregion

        }
        else
        {
            throw new ArgumentNullException("lf.HostBuilder must have WrappedHostApplicationBuilder or WrappedHostBuilder");
        }

        #endregion


        // Configured supplemental AppSettings file
        builder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile(Path.Combine(configDir, "appsettings." + hostingContext.HostingEnvironment.EnvironmentName + ".json"), optional: true, reloadOnChange);

            var appSettingsFile = hostingContext.Configuration[ConfigurationKeys.AppSettingsFileKey];
            if (appSettingsFile != null)
            {
                Log.Get(typeof(DefaultsX).FullName!).LogInformation($"Adding config source: {appSettingsFile}  (exists: {File.Exists(appSettingsFile)})");
                config
                      .AddJsonFile(appSettingsFile, optional: true, reloadOnChange: true);
            }
        });

        #endregion

        builder.ConfigureLogging((context, logging) =>
        {
            logging.ClearProviders(); // Use Serilog providers by default, but you can still add more providers.  TODO - are there any to delete at this point?
            logging.AddConfiguration(context.Configuration.GetSection("Logging"));
        });
        
        lf.Serilog();

#if OLD // TODELETE - NLog (from Valor)
        //NLog.LogManager.Configuration = new NLogLoggingConfiguration(hostApplicationBuilder.Configuration.GetSection("NLog"));

//.AddLogging(loggingBuilder =>
                    //{
                    //    loggingBuilder.ClearProviders();
                    //    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                    //    //loggingBuilder.AddNLog(hostApplicationBuilder.Configuration);
                    //    //loggingBuilder.AddNLog(NLog.LogManager.Configuration);
                    //    //loggingBuilder.AddConsole();
                    //})
#endif


        lf.HostBuilder.UseDependencyContext();

        lf.ConfigureServices(services =>
        {
            services
                .AddSingleton<TypeScanner>()
                .Configure<TypeScannerOptions>(o =>
                {
                    o.DllPrefixWhitelist.Add("LionFire.");
                })
                .AddSingleton<IKeyProvider<string, object>, KeyProviderService<string, object>>();
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
