using LionFire.MagicOnion_;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

public static class MagicOnionHostingX
{
    public static IServiceCollection AddMagicOnionClient(this IServiceCollection services)
    {

        return services
            .AddTransient<MagicOnionConnection>()
            .AddSingleton<MagicOnionConnectionManager>()
            ;
    }
}
