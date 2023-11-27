using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting;

public static class IHostApplicationBuilderX
{
    public static IHostApplicationBuilder ConfigureServices(this IHostApplicationBuilder builder, Action<IServiceCollection> s)
    {
        s(builder.Services);
        return builder;
    }

    public static T I<T>(this T hostApplicationBuilder, Action<IHostApplicationBuilder> configure)
        where T : IHostApplicationBuilder
        => hostApplicationBuilder.AsIHostApplicationBuilder(configure);

    public static T AsIHostApplicationBuilder<T>(this T hostApplicationBuilder, Action<IHostApplicationBuilder> configure)
        where T : IHostApplicationBuilder
    {
        configure(hostApplicationBuilder);
        return hostApplicationBuilder;
    }
}
