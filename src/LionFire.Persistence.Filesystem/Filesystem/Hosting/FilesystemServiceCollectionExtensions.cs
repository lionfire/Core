﻿using Microsoft.Extensions.DependencyInjection;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using Microsoft.Extensions.Options;
using LionFire.Referencing;

namespace LionFire.Services
{
    public static class FilesystemServiceCollectionExtensions
    {

        // TODO: optional auto-extension support for deserializers/serializers. Serializer adapter?
        public static IServiceCollection AddFilesystem(this IServiceCollection services)
        {
            services
                .TryAddEnumerableSingleton<IReferenceProvider, FileReferenceProvider>()
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
