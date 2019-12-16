using LionFire.Dependencies;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting
{
    public static class AsyncHostBuilderExtensions
    {
        public static IHostBuilder AllowAsync(this IHostBuilder hostBuilder)
        {
            DependencyLocatorConfiguration.UseServiceProviderToActivateSingletons = false;
            DependencyLocatorConfiguration.UseSingletons = false;
            DependencyContext.AsyncLocal = new DependencyContext();
            return hostBuilder;
        }
    }
}
