using LionFire.IO;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

public static class VirtualFilesystemHostingExtensions
{
    public static IServiceCollection AddVirtualFilesystem(this IServiceCollection services)
    {
        return services
            .AddSingleton<NativeFilesystem>()
            .AddSingleton<IVirtualFilesystem>(serviceProvider => serviceProvider.GetRequiredService<NativeFilesystem>())

            //.TryAddEnumerable(ServiceDescriptor.Singleton<IVirtualFilesystem, NativeFilesystem>())
            //.TryAddEnumerable(ServiceDescriptor.Singleton<IFilesystemPlugin, ArchiveFilesystem>())
            //.TryAddEnumerable<IFilesystemPlugin, ArchiveFilesystem>()
            ;
    }
}
