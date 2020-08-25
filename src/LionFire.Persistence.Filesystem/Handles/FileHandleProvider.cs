#nullable enable
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;

namespace LionFire.Persistence.Filesystem
{

    public class FileHandleProvider :
        IReadHandleProvider<FileReference>
        , IReadWriteHandleProvider<FileReference>
        , IReadWriteHandleProvider // REVIEW
        , IReadHandleProvider<ProviderFileReference>
        , IWriteHandleProvider<FileReference>
        , IWriteHandleProvider
    {
        IPersister<FileReference> persister;
        IPersisterProvider<ProviderFileReference> providerFilePersisterProvider;
        public FileHandleProvider(
            IPersisterProvider<FileReference> filePersisterProvider,
            IPersisterProvider<ProviderFileReference> providerFilePersisterProvider
            )
        {
            this.persister = filePersisterProvider.GetPersister();
            this.providerFilePersisterProvider = providerFilePersisterProvider;
        }

        public IReadHandle<T> GetReadHandle<T>(FileReference reference, T preresolvedValue = default)
            => new PersisterReadHandle<FileReference, T, IPersister<FileReference>>(persister, reference, preresolvedValue);
        IReadHandle<T>? IReadHandleProvider.GetReadHandle<T>(IReference reference, T preresolvedValue) => (reference is FileReference fileReference) ? GetReadHandle<T>(fileReference, preresolvedValue) : null;

        public IReadWriteHandle<T> GetReadWriteHandle<T>(FileReference reference, T preresolvedValue = default)
            => new PersisterReadWriteHandle<FileReference, T, IPersister<FileReference>>(persister, reference, preresolvedValue);
        IReadWriteHandle<T>? IReadWriteHandleProvider.GetReadWriteHandle<T>(IReference reference, T preresolvedValue) => (reference is FileReference fileReference) ? GetReadWriteHandle<T>(fileReference, preresolvedValue) : null;

        public IReadHandle<T> GetReadHandle<T>(ProviderFileReference reference, T preresolvedValue = default)
            => new PersisterReadWriteHandle<ProviderFileReference, T, IPersister<ProviderFileReference>>(providerFilePersisterProvider.GetPersister(reference.Persister), reference, preresolvedValue);
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
