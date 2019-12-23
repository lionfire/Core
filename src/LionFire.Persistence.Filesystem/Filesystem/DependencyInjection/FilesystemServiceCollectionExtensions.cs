using Microsoft.Extensions.DependencyInjection;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;

namespace LionFire.Services
{
    public static class FilesystemServiceCollectionExtensions
    {
      
        public static IServiceCollection AddFilesystem(this IServiceCollection services)
        {
            services
                .AddSingleton<FilesystemPersisterOptions>()
                .AddSingleton<FilesystemPersister>()
                .AddSingleton<IReadHandleProvider<FileReference>, FileHandleProvider>()
                .AddSingleton<IReadWriteHandleProvider<FileReference>, FileHandleProvider>()
                .AddSingleton<IReadHandleProvider<ProviderFileReference>, FileHandleProvider>()
                .AddSingleton<IPersisterProvider<FileReference>, FilesystemPersisterProvider>()
                .AddSingleton<IPersisterProvider<ProviderFileReference>, FilesystemPersisterProvider>()
                ;

            //IReadHandleProvider<ProviderFileReference> a;
            //FileHandleProvider b;
            //b = new FileHandleProvider(null);
            //a = b;
            //services
            //.AddSingleton<, FileHandleProvider>();


            return services;
        }
    }

}
