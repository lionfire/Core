#nullable enable

namespace LionFire.Hosting;

//public static class LionFireNLogX { 
    #region NLog

    //builder.ConfigureLogging((context, logger) => logger
    //    .AddLionFireNLog(context.Configuration.GetSection("Logging"), null)
    //);

    //public static NLogProviderOptions DefaultNLogProviderOptions { get; set; } = new NLogProviderOptions
    //{
    //    LoggingConfigurationSectionName = "Logging:NLog",
    //};

    //public static ILionFireHostBuilder NLog(this ILionFireHostBuilder b, Action<LionFireSerilogBuilder>? configure = null)
    //{
    ////NLog.Extensions.Hosting.ConfigureExtensions.UseNLog
    //if (b.HostBuilder.WrappedHostApplicationBuilder != null)
    //{
    //    var b2 = b.HostBuilder.WrappedHostApplicationBuilder;

    //    AddNLogLoggerProvider(b2.Services, b2.Configuration, builderContext.HostingEnvironment, options, CreateNLogLoggerProvider));

    //    NLogProviderOptions options = null;
    //    AddNLogLoggerProvider(b2.Services, b2.Configuration, options, CreateNLogLoggerProvider));

    //}

    //b.ForHostBuilder(b => b.AddLionFireLogging(clearProviders)); // TEMP FIXME

    //}

    //=> b; // .ForHostBuilder(b => b.AddLionFireLogging(clearProviders)); // TEMP FIXME

    //public static HostApplicationBuilder AddLionFireSerilog(this HostApplicationBuilder builder)
    //    => builder.UseSerilog();

    //private static void AddNLogLoggerProvider(IServiceCollection services, IConfiguration hostConfiguration, IHostEnvironment hostEnvironment, NLogProviderOptions options, Func<IServiceProvider, IConfiguration, IHostEnvironment, NLogProviderOptions, NLogLoggerProvider> factory)
    //{
    //    LogManager.AddHiddenAssembly(typeof(ConfigureExtensions).GetTypeInfo().Assembly);
    //    services.TryAddNLogLoggingProvider((svc, addlogging) => svc.AddLogging(addlogging), hostConfiguration, options, (provider, cfg, opt) => factory(provider, cfg, hostEnvironment, opt));
    //}

    //private static NLogLoggerProvider CreateNLogLoggerProvider(IServiceProvider serviceProvider, IConfiguration hostConfiguration, IHostEnvironment hostEnvironment, NLogProviderOptions options)
    //{
    //    NLogLoggerProvider provider = serviceProvider.CreateNLogLoggerProvider(hostConfiguration, options, null);

    //    //var contentRootPath = hostEnvironment?.ContentRootPath;
    //    //if (!string.IsNullOrEmpty(contentRootPath))
    //    //{
    //    //    TryLoadConfigurationFromContentRootPath(provider.LogFactory, contentRootPath); // if !IsLoggingConfigurationLoaded, try XML file
    //    //}

    //    provider.LogFactory.Setup().LoadConfiguration(config =>
    //    {
    //        if (!IsLoggingConfigurationLoaded(config.Configuration))
    //        {

    //        }
    //    });

    //        return provider;
    //}


    //public static IHostBuilder AddLionFireLogging(this IHostBuilder builder, bool clearProviders = false)
    //    => builder
    //            //.ConfigureHostConfiguration(c =>
    //            //    c.AddInMemoryCollection(new KeyValuePair<string, string>[] { new("Logging:Console:LogLevel:Default", "Information") }))
    //            //.ConfigureServices((context,services) => services
    //            //    .AddLogging(l => l.AddLionFireLogging(context.Configuration, clearProviders))
    //            //)
    //            .UseNLog(DefaultNLogProviderOptions)
    //        ;

    //builder.ConfigureServices((builderContext, services) => AddNLogLoggerProvider(services, builderContext.Configuration, options, CreateNLogLoggerProvider));


    //public static ILoggingBuilder AddLionFireLogging(this ILoggingBuilder builder, IConfiguration configuration, bool clearProviders = false, LogLevel logLevel = LogLevel.Information)
    //    => builder
    //        //.If(clearProviders, x => x.ClearProviders())
    //        //.SetMinimumLevel(logLevel)
    //        .AddLionFireNLog(configuration)  // TODO: Reconcile with UseNLog above
    //    ;

    #endregion

//}

