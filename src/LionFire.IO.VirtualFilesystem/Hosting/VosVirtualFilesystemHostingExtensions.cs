using LionFire.IO;
using LionFire.IO.Vos;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

public static class VosVirtualFilesystemHostingExtensions // MOVE
{
    public static IServiceCollection AddVosVirtualFilesystem(this IServiceCollection services)
    {
        return services
            .AddSingleton<VosVirtualFilesystem>()
            .AddSingleton<IVirtualFilesystem>(serviceProvider => serviceProvider.GetRequiredService<VosVirtualFilesystem>())
            ;
    }
}
