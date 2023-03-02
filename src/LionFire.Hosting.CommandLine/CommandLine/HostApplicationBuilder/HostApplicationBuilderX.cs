#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting;

public static class HostApplicationBuilderX
{
    public static HostApplicationBuilder ConfigureServices(this HostApplicationBuilder hostApplicationBuilder, Action<IServiceCollection> configure)
    {
        configure(hostApplicationBuilder.Services);
        return hostApplicationBuilder;
    }
}
