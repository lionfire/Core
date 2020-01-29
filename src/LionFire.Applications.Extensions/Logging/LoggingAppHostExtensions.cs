using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Applications.Hosting
{
    public static class LoggingAppHostExtensions
    {
        // Compare with Microsoft's approach:
        // https://stackoverflow.com/questions/50744024/iloggerfactory-vs-servicecollection-addlogging-vs-webhostbuilder-configureloggin

        public static IAppHost AddLogging(this IAppHost app, Action<ILoggingBuilder> configure)
        {
            app.ConfigureServices(sc => sc.AddLogging(configure));
            return app;
        }
        public static IAppHost AddLogging(this IAppHost app)
        {
            app.ConfigureServices(sc => sc.AddLogging());
            return app;
        }
    }
}
