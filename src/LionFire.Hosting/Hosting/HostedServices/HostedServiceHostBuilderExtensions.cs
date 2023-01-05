using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Hosting
{
    public static class HostedServiceHostBuilderExtensions
    {
        public static IHostBuilder UseHostedService<T>(this IHostBuilder hostBuilder)
            where T : class, IHostedService
        {
            return hostBuilder.ConfigureServices((context, services) =>
                services.AddHostedService<T>());
        }
    }
}
