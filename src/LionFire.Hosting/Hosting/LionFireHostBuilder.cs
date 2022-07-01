using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace LionFire.Hosting
{
    /// <summary>
    /// Marker type for fluent building of LionFire
    /// </summary>
    public class LionFireHostBuilder
    {
        public LionFireHostBuilder(IHostBuilder hostBuilder) { HostBuilder = new HostBuilderWrapper(hostBuilder, this); }

        public HostBuilderWrapper HostBuilder { get; }

        public LionFireHostBuilder ForHostBuilder(Action<IHostBuilder> action)
        {
            action(HostBuilder.Wrapped);
            return this;
        }

        public LionFireHostBuilder ConfigureServices(Action<IServiceCollection> configure) { HostBuilder.ConfigureServices(configure); return this; }
        public LionFireHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configure) { HostBuilder.ConfigureServices(configure); return this; }
    }

    public static class DoneExtensions
    {
        public static LionFireHostBuilder Done(this IHostBuilder hostBuilder) => (hostBuilder as HostBuilderWrapper)?.Done() 
            ?? throw new ArgumentException("hostBuilder is not HostBuilderWrapper and does not need Done")
            ;
    }
}
