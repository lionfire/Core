using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;

namespace LionFire.Hosting
{
    public static class LionFireLoggingExtensions
    {

        public static NLogProviderOptions DefaultNLogProviderOptions { get; set; } = new NLogProviderOptions
        {
            LoggingConfigurationSectionName = "Logging:NLog",
        };

        public static LionFireHostBuilder Log(this LionFireHostBuilder b, bool clearProviders = false)
            => b.ForHostBuilder(b => b.AddLionFireLogging(clearProviders));

        public static IHostBuilder AddLionFireLogging(this IHostBuilder builder, bool clearProviders = false)
            => builder
                    //.ConfigureHostConfiguration(c =>
                    //    c.AddInMemoryCollection(new KeyValuePair<string, string>[] { new("Logging:Console:LogLevel:Default", "Information") }))
                    //.ConfigureServices((context,services) => services
                    //    .AddLogging(l => l.AddLionFireLogging(context.Configuration, clearProviders))
                    //)
                    .UseNLog(DefaultNLogProviderOptions)
                ;

        //builder.ConfigureServices((builderContext, services) => AddNLogLoggerProvider(services, builderContext.Configuration, options, CreateNLogLoggerProvider));


        public static ILoggingBuilder AddLionFireLogging(this ILoggingBuilder builder, IConfiguration configuration, bool clearProviders = false, LogLevel logLevel = LogLevel.Information)
            => builder
                //.If(clearProviders, x => x.ClearProviders())
                //.SetMinimumLevel(logLevel)
                .AddLionFireNLog(configuration)  // TODO: Reconcile with UseNLog above
            ;


    }
}
