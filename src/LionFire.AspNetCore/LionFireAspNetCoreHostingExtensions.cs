using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Infra.HealthChecker
{
    public static class LionFireAspNetCoreHostingExtensions
    {
        public static IHostBuilder ConfigureWebHostDefaults<TStartup>(this IHostBuilder hostBuilder, bool configureKestrel = true) 
            where TStartup : class 
            => hostBuilder
                .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder
                            .UseKestrel((context, serverOptions) =>
                            {
                                if (configureKestrel)
                                {
                                    serverOptions.Configure(context.Configuration.GetSection("Kestrel"), reloadOnChange: false);
                                }
                            })
                            .UseStartup<TStartup>()
                        ;
                    })
            .ConfigureServices((context, services) =>
            {
                if (configureKestrel)
                {
                    services.Configure<KestrelServerOptions>(
                        context.Configuration.GetSection("Kestrel"));
                }
            })
            ;
    }
}
