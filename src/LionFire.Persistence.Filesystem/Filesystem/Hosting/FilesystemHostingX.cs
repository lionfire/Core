using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

public static class FilesystemHostingX
{
    /// <summary>
    /// Add the full Framework for Filesystem
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddFilesystem(this IServiceCollection services)
        => services
            .AddFilesystemPersister()
            ;
}
