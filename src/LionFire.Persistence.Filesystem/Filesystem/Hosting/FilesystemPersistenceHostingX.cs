using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Resilience;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using Microsoft.Extensions.Options;
using LionFire.Referencing;
using LionFire.Hosting;
using System.IO;
using Polly;
using Polly.Retry;
using System;

namespace LionFire.Hosting;

public static class FilesystemResilienceHostingX
{

}
public static class FilesystemPersistenceHostingX
{
    public static IServiceCollection AddFilesystemResilience(this IServiceCollection s) => s
        .AddResilienceEnricher() // For Telemetry
        .AddResiliencePipeline(FilesystemPersistenceResilience.RetryPolicyKey, static builder =>
        {
            // See: https://www.pollydocs.org/strategies/retry.html
            builder.AddRetry(new RetryStrategyOptions
            {
                // "being used by another process"
                ShouldHandle = new PredicateBuilder().Handle<IOException>(),
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromMilliseconds(200),
                MaxDelay = TimeSpan.FromSeconds(1),
                MaxRetryAttempts = 5,
            });

            // See: https://www.pollydocs.org/strategies/timeout.html
            builder.AddTimeout(TimeSpan.FromSeconds(1.5));
        })
    ;

    public static IServiceCollection AddFilesystemReferences(this IServiceCollection services)
        => services
            .TryAddEnumerableSingleton<IReferenceProvider, FileReferenceProvider>();

    // TODO: optional auto-extension support for deserializers/serializers. Serializer adapter?
    //public static IServiceCollection AddFilesystemPersister(this IServiceCollection services)
    //{
    //    return services
    //        .Configure<FilesystemPersisterOptions>(o => { })
    //        .AddSingleton(s => s.GetService<IOptionsMonitor<FilesystemPersisterOptions>>()?.CurrentValue)
    //        .AddSingleton<FilesystemPersister>()

    //        .AddSingleton<FileHandleProvider>()
    //        .AddSingleton<IReadHandleProvider<FileReference>, FileHandleProvider>(s => s.GetRequiredService<FileHandleProvider>())
    //        .AddSingleton<IReadWriteHandleProvider<FileReference>, FileHandleProvider>(s => s.GetRequiredService<FileHandleProvider>())
    //        .AddSingleton<IReadHandleProvider<ProviderFileReference>, FileHandleProvider>(s => s.GetRequiredService<FileHandleProvider>())

    //        .AddSingleton<FilesystemPersisterProvider>()
    //        .AddSingleton<IPersisterProvider<IFileReference>, FilesystemPersisterProvider>(s => s.GetRequiredService<FilesystemPersisterProvider>())
    //        .AddSingleton<IPersisterProvider<ProviderFileReference>, FilesystemPersisterProvider>(s => s.GetRequiredService<FilesystemPersisterProvider>())

    //        .AddSingleton<IWriteHandleProvider<FileReference>, FileHandleProvider>(s => s.GetRequiredService<FileHandleProvider>())
    //        //.AddSingleton<IReadWriteHandleProvider<FileReference>, FileHandleProvider>(s => s.GetRequiredService<FileHandleProvider>())
    //        //.AddSingleton<IWriteHandleProvider<ProviderFileReference>, FileHandleProvider>(s => s.GetRequiredService<FileHandleProvider>())
    //        ;
    //}

    public static IServiceCollection AddFilesystemPersister(this IServiceCollection services)
        => services
            .AddFilesystemResilience()
            .AddFilesystemReferences()
            //.AddFilesystemPersister()
            ;
}
