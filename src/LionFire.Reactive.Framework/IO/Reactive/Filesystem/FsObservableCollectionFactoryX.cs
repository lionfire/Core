using LionFire.IO.Reactive.Hjson;
using LionFire.Ontology;
using LionFire.Persistence.Filesystem;
using LionFire.Reactive.Persistence;
using LionFire.Referencing;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.IO.Reactive.Filesystem;

public static class FsObservableCollectionFactoryX
{
    public static IServiceCollection RegisterObservablesInSubDirForType<TValue>(this IServiceCollection services, IServiceProvider serviceProvider, IReference parentDir, string? entitySubdir = null, bool recursive = true)
        where TValue : notnull
    {
        var pluralTypeName = typeof(TValue).GetPluralName();
        entitySubdir ??= pluralTypeName;

        var valuesDir = entitySubdir.Length == 0 ? parentDir : parentDir.GetChildSubpath(entitySubdir);
        var valuesDirSelector = new LionFire.IO.Reactive.DirectoryReferenceSelector(valuesDir) { Recursive = recursive };

        services.RegisterObservablesInDir<TValue>(serviceProvider, valuesDirSelector);

        return services;
    }
    
    public static void RegisterObservablesInDir<TValue>(this  IServiceCollection services, IServiceProvider serviceProvider, DirectoryReferenceSelector valuesDirReferenceSelector) where TValue : notnull
    {
        if (valuesDirReferenceSelector.Path is FileReference fileReference) // TODO remove this HARDCODE - allow extensibility via DI instead of this
        {
            var dirSelector = new DirectorySelector(fileReference.Path)
            {
                // FUTURE: Clone any future properties automatically
                Recursive = valuesDirReferenceSelector.Recursive,
            };

            var r = ActivatorUtilities.CreateInstance<HjsonFsDirectoryReaderRx<string, TValue>>(serviceProvider, dirSelector);
            var w = ActivatorUtilities.CreateInstance<HjsonFsDirectoryWriterRx<string, TValue>>(serviceProvider, dirSelector);
            services.AddSingleton<IObservableReaderWriter<string, TValue>>(sp => new ObservableReaderWriterFromComponents<string, TValue>(r, w));
        }
        else
        {
            throw new NotSupportedException("Only FileReference is supported for now");
        }
    }
}

