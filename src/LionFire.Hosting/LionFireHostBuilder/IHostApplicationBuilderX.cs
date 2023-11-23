using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

public static class IHostApplicationBuilderX
{
    public static IHostApplicationBuilder ConfigureServices(this IHostApplicationBuilder builder, Action<IServiceCollection> s)
    {
        s(builder.Services);
        return builder;
    }
}
