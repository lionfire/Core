using Microsoft.Extensions.DependencyInjection;
using LionFire.Persistence.Filesystem;
using LionFire.Referencing;

namespace LionFire.Hosting;

public static class FilesystemReferencesHostingX
{
    public static IServiceCollection AddFilesystemReferences(this IServiceCollection services)
        => services
            .TryAddEnumerableSingleton<IReferenceProvider, FileReferenceProvider>();
}
