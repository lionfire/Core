using LionFire.Dependencies;
using LionFire.IO.Reactive.Hjson;
using LionFire.Ontology;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Filesystemlike;
using LionFire.Reactive.Persistence;
using LionFire.Referencing;
using LionFire.Structures;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reactive.Disposables;

namespace LionFire.IO.Reactive.Filesystem;

public static class FsObservableCollectionFactoryX
{
    // Each HjsonFsDirectoryReaderRx opens a FileSystemWatcher in its constructor, and on Linux every
    // FileSystemWatcher consumes one whole inotify instance (fs.inotify.max_user_instances, default 128).
    // Callers such as WorkspaceLayoutVM re-register the same directory per Blazor circuit, so the readers
    // must be shared per directory rather than constructed per call.
    private static readonly ConcurrentDictionary<(Type ValueType, string Path, bool Recursive, int RecursionDepth), object> observablesByDir = new();

    public static IServiceCollection RegisterObservablesInSubDirForType<TValue>(this IServiceCollection services, IServiceProvider serviceProvider, IReference parentDir, string? entitySubdir = null, bool recursive = true, bool autosave = true, int recursionDepth = int.MaxValue)
        where TValue : notnull
    {
        var pluralTypeName = typeof(TValue).GetPluralName();
        entitySubdir ??= pluralTypeName;

        var valuesDir = entitySubdir.Length == 0 ? parentDir : parentDir.GetChildSubpath(entitySubdir);
        var valuesDirSelector = new LionFire.IO.Reactive.DirectoryReferenceSelector(valuesDir) { Recursive = recursive, RecursionDepth = recursionDepth };

        services.RegisterObservablesInDir<TValue>(serviceProvider, valuesDirSelector, autosave: autosave);

        return services;
    }

    public static IServiceCollection RegisterObservablesInDir<TValue>(this IServiceCollection services, IServiceProvider serviceProvider, DirectoryReferenceSelector valuesDirReferenceSelector, bool autosave = true) where TValue : notnull
    {
        var x = serviceProvider.RegisterObservablesInDir<TValue>(valuesDirReferenceSelector, autosave);
        if (x != null)
        {
            services.AddSingleton<IObservableReaderWriter<string, TValue>>(x);
            services.AddSingleton<IObservableReader<string, TValue>>(x);
            services.AddSingleton<IObservableWriter<string, TValue>>(x);
        }
        return services;
    }

    public static Func<IServiceProvider, IObservableReaderWriter<string, TValue>> RegisterObservablesInDir<TValue>(this IServiceProvider serviceProvider, DirectoryReferenceSelector valuesDirReferenceSelector, bool autosave = true) where TValue : notnull
    {
        if (valuesDirReferenceSelector.Path is FileReference fileReference) // TODO remove this HARDCODE - allow extensibility via DI instead of this
        {
            var key = (typeof(TValue), fileReference.Path, valuesDirReferenceSelector.Recursive, valuesDirReferenceSelector.RecursionDepth);

            ObservableReaderWriterFromComponents<string, TValue> c;
            try
            {
                c = (ObservableReaderWriterFromComponents<string, TValue>)observablesByDir.GetOrAdd(key, _ =>
                {
                    var dirSelector = new DirectorySelector(fileReference.Path)
                    {
                        // FUTURE: Clone any future properties automatically
                        Recursive = valuesDirReferenceSelector.Recursive,
                        RecursionDepth = valuesDirReferenceSelector.RecursionDepth,
                    };

                    var directoryTypeOptions = new DirectoryTypeOptions
                    {
                        ExtensionConvention = serviceProvider.GetService<IFileExtensionConvention>() ?? Singleton<DefaultExtensionConvention>.Instance,
                    };
                    directoryTypeOptions.SecondExtension = directoryTypeOptions.ExtensionConvention.FileExtensionForType(typeof(TValue));

                    var r = ActivatorUtilities.CreateInstance<HjsonFsDirectoryReaderRx<string, TValue>>(serviceProvider, dirSelector, directoryTypeOptions);
                    var w = ActivatorUtilities.CreateInstance<HjsonFsDirectoryWriterRx<string, TValue>>(serviceProvider, dirSelector, directoryTypeOptions);

                    var created = new ObservableReaderWriterFromComponents<string, TValue>(r, w);
                    if (autosave)
                    {
                        // Subscribed once for the lifetime of the shared instance, so it is never unsubscribed.
                        created.AutosaveValueChanges();
                    }
                    return created;
                });
            }
            catch (ObjectDisposedException)
            {
                return null!; // NULLABILITY OVERRIDE - ServiceProvider disposed during async race
            }

            return sp => c;
        }
        else
        {
            throw new NotSupportedException("Only FileReference is supported for now");
        }
    }
}

