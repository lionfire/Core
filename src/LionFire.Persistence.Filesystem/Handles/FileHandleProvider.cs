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

        public IReadHandle<T> GetReadHandle<T>(FileReference reference)
            => new PersisterReadWriteHandle<FileReference, T, IPersister<FileReference>>(persister, reference);
        public IReadWriteHandle<T> GetReadWriteHandle<T>(FileReference reference)
            => new PersisterReadWriteHandle<FileReference, T, IPersister<FileReference>>(persister, reference);

        public IReadHandle<T> GetReadHandle<T>(ProviderFileReference reference)
            => new PersisterReadWriteHandle<ProviderFileReference, T, IPersister<ProviderFileReference>>(providerFilePersisterProvider.GetPersister(reference.Persister), reference);
        IReadWriteHandle<T> IReadWriteHandleProvider.GetReadWriteHandle<T>(IReference reference) => GetReadWriteHandle<T>((FileReference)reference);
#warning TODO: ReadWrite handle

    }
}
