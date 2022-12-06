#nullable enable
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;

namespace LionFire.Persistence.Filesystem
{

    public class FileHandleProvider :

        // R
        IReadHandleProvider<FileReference>
        , IPreresolvableReadHandleProvider<FileReference>
        , IReadHandleProvider<ProviderFileReference>
        , IPreresolvableReadHandleProvider<ProviderFileReference>

        // RW
        , IReadWriteHandleProvider<FileReference>
        , IReadWriteHandleProvider // REVIEW

        // W
        , IWriteHandleProvider<FileReference>
        , IWriteHandleProvider
    {
        IPersister<IFileReference> persister;
        IPersisterProvider<ProviderFileReference> providerFilePersisterProvider;
        public FileHandleProvider(
            IPersisterProvider<IFileReference> filePersisterProvider,
            IPersisterProvider<ProviderFileReference> providerFilePersisterProvider
            )
        {
            this.persister = filePersisterProvider.GetPersister();
            this.providerFilePersisterProvider = providerFilePersisterProvider;
        }

        public IReadHandle<T> GetReadHandle<T>(FileReference reference)
            => new PersisterReadHandle<IFileReference, T, IPersister<IFileReference>>(persister, reference.ForType<T>());
        public IReadHandle<T> GetReadHandlePreresolved<T>(FileReference reference, T? preresolvedValue = default)
            => new PersisterReadHandle<IFileReference, T, IPersister<IFileReference>>(persister, reference.ForType<T>(), preresolvedValue);
        IReadHandle<T>? IReadHandleProvider.GetReadHandle<T>(IReference reference) => (reference is FileReference fileReference) ? GetReadHandle<T>(fileReference) : null;
        IReadHandle<T>? IPreresolvableReadHandleProvider.GetReadHandlePreresolved<T>(IReference reference, T preresolvedValue) => (reference is FileReference fileReference) ? GetReadHandlePreresolved<T>(fileReference, preresolvedValue) : null;

        public IReadWriteHandle<T> GetReadWriteHandle<T>(FileReference reference, T? preresolvedValue = default)
            => new PersisterReadWriteHandle<IFileReference, T, IPersister<IFileReference>>(persister, reference.ForType<T>(), preresolvedValue);
        IReadWriteHandle<T>? IReadWriteHandleProvider.GetReadWriteHandle<T>(IReference reference, T preresolvedValue) => (reference is FileReference fileReference) ? GetReadWriteHandle<T>(fileReference, preresolvedValue) : null;

        public IReadHandle<T> GetReadHandle<T>(ProviderFileReference reference)
            => new PersisterReadWriteHandle<ProviderFileReference, T, IPersister<ProviderFileReference>>(providerFilePersisterProvider.GetPersister(reference.Persister), reference.ForType<T>());
        public IReadHandle<T> GetReadHandlePreresolved<T>(ProviderFileReference reference, T preresolvedValue = default)
            => new PersisterReadWriteHandle<ProviderFileReference, T, IPersister<ProviderFileReference>>(providerFilePersisterProvider.GetPersister(reference.Persister), reference.ForType<T>(), preresolvedValue);
        //public IReadHandle<T> GetReadWriteHandle<T>(ProviderFileReference reference)
        //        => new PersisterReadWriteHandle<ProviderFileReference, T, IPersister<ProviderFileReference>>(providerFilePersisterProvider.GetPersister(reference.Persister), reference);

        //IReadHandle<T> IReadHandleProvider<FileReference>.GetReadHandle<T>(FileReference reference) => throw new System.NotImplementedException();

        public IWriteHandle<T> GetWriteHandle<T>(FileReference reference,T prestagedValue = default) => GetReadWriteHandle<T>(reference, prestagedValue); // REVIEW - 
        IWriteHandle<T>? IWriteHandleProvider.GetWriteHandle<T>(IReference reference,T prestagedValue) => (reference is FileReference fileReference) ? GetWriteHandle<T>(fileReference, prestagedValue) : null;
    }

    //public static class IReadHandleProviderExtensions
    //{
    //    public static IReadHandle<TValue> GetReadHandle<TValue, TReference>(this IReadHandleProvider<TReference> readHandleProvider, IReference reference)
    //        where TReference : IReference
    //        => (reference is TReference concreteReference) ? readHandleProvider.GetReadHandle<TValue>(concreteReference) : null;
    //}
}
