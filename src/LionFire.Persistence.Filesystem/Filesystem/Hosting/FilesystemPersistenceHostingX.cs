using Microsoft.Extensions.DependencyInjection;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using Microsoft.Extensions.Options;
using LionFire.Hosting;

namespace LionFire.Hosting;

public static class FilesystemPersistenceHostingX
{
    // TODO: optional auto-extension support for deserializers/serializers. Serializer adapter?
    public static IServiceCollection AddFilesystemPersister(this IServiceCollection services)
    {
        return services
        #region Dependencies
            .AddFilesystemResilience()
            .AddFilesystemReferences()
        #endregion

            .Configure<FilesystemPersisterOptions>(o => { })
            .AddSingleton(s => s.GetService<IOptionsMonitor<FilesystemPersisterOptions>>()?.CurrentValue)
            .AddSingleton<FilesystemPersister>()

            .AddSingleton<FileHandleProvider>()
            .AddSingleton<IReadHandleProvider<FileReference>, FileHandleProvider>(s => s.GetRequiredService<FileHandleProvider>())
            .AddSingleton<IReadWriteHandleProvider<FileReference>, FileHandleProvider>(s => s.GetRequiredService<FileHandleProvider>())
            .AddSingleton<IReadHandleProvider<ProviderFileReference>, FileHandleProvider>(s => s.GetRequiredService<FileHandleProvider>())

            .AddSingleton<FilesystemPersisterProvider>()
            .AddSingleton<IPersisterProvider<IFileReference>, FilesystemPersisterProvider>(s => s.GetRequiredService<FilesystemPersisterProvider>())
            .AddSingleton<IPersisterProvider<ProviderFileReference>, FilesystemPersisterProvider>(s => s.GetRequiredService<FilesystemPersisterProvider>())

            .AddSingleton<IWriteHandleProvider<FileReference>, FileHandleProvider>(s => s.GetRequiredService<FileHandleProvider>())
            //.AddSingleton<IReadWriteHandleProvider<FileReference>, FileHandleProvider>(s => s.GetRequiredService<FileHandleProvider>())
            //.AddSingleton<IWriteHandleProvider<ProviderFileReference>, FileHandleProvider>(s => s.GetRequiredService<FileHandleProvider>())
            ;
    }

}
