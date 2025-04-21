using LionFire.Dependencies;
using LionFire.IO.Reactive.Hjson;
using LionFire.Ontology;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Filesystemlike;
using LionFire.Reactive.Persistence;
using LionFire.Referencing;
using LionFire.Structures;
using Microsoft.Extensions.DependencyInjection;
using System.Reactive.Disposables;

namespace LionFire.IO.Reactive.Filesystem;

public static class FsObservableCollectionFactoryX
{
    public static IServiceCollection RegisterObservablesInSubDirForType<TValue>(this IServiceCollection services, IServiceProvider serviceProvider, IReference parentDir, string? entitySubdir = null, bool recursive = true, bool autosave = true)
        where TValue : notnull
    {
        var pluralTypeName = typeof(TValue).GetPluralName();
        entitySubdir ??= pluralTypeName;

        var valuesDir = entitySubdir.Length == 0 ? parentDir : parentDir.GetChildSubpath(entitySubdir);
        var valuesDirSelector = new LionFire.IO.Reactive.DirectoryReferenceSelector(valuesDir) { Recursive = recursive };

        services.RegisterObservablesInDir<TValue>(serviceProvider, valuesDirSelector, autosave: autosave);

        return services;
    }

    public static IServiceCollection RegisterObservablesInDir<TValue>(this IServiceCollection services, IServiceProvider serviceProvider, DirectoryReferenceSelector valuesDirReferenceSelector, bool autosave = true) where TValue : notnull
    {
        var x = serviceProvider.RegisterObservablesInDir<TValue>(valuesDirReferenceSelector, autosave);
        if (x != null)
        {
            services.AddSingleton<IObservableReaderWriter<string, TValue>>(x);
        }
        return services;
    }

    public static Func<IServiceProvider, IObservableReaderWriter<string, TValue>> RegisterObservablesInDir<TValue>(this IServiceProvider serviceProvider, DirectoryReferenceSelector valuesDirReferenceSelector, bool autosave = true) where TValue : notnull
    {
        if (valuesDirReferenceSelector.Path is FileReference fileReference) // TODO remove this HARDCODE - allow extensibility via DI instead of this
        {
            var dirSelector = new DirectorySelector(fileReference.Path)
            {
                // FUTURE: Clone any future properties automatically
                Recursive = valuesDirReferenceSelector.Recursive,
            };

            DirectoryTypeOptions DirectoryTypeOptions;
            try
            {
                DirectoryTypeOptions = new DirectoryTypeOptions
                {
                    ExtensionConvention = serviceProvider.GetService<IFileExtensionConvention>() ?? Singleton<DefaultExtensionConvention>.Instance,
                };
            }
            catch (ObjectDisposedException)
            {
                return null!; // NULLABILITY OVERRIDE
            }
            DirectoryTypeOptions.SecondExtension = DirectoryTypeOptions.ExtensionConvention.FileExtensionForType(typeof(TValue));

            var r = ActivatorUtilities.CreateInstance<HjsonFsDirectoryReaderRx<string, TValue>>(serviceProvider, dirSelector, DirectoryTypeOptions);
            var w = ActivatorUtilities.CreateInstance<HjsonFsDirectoryWriterRx<string, TValue>>(serviceProvider, dirSelector, DirectoryTypeOptions);
            return sp =>
            {
                var c = new ObservableReaderWriterFromComponents<string, TValue>(r, w);
                if (autosave)
                {
                    var disposable = c.AutosaveValueChanges();
                }
                return c;
            };
        }
        else
        {
            throw new NotSupportedException("Only FileReference is supported for now");
        }
    }
}

