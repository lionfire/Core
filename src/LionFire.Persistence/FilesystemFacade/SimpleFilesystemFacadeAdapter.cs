#if TODO
using System;
using System.Threading.Tasks;
using LionFire.Referencing;
using LionFire.Serialization;

namespace LionFire.Persistence.FilesystemFacade
{
    // TODO

    public class SimpleFilesystemFacadeAdapter<TReference, TPersistenceOptions> : IFSPersistence<TReference, TPersistenceOptions>
        where TReference : IReference
        where TPersistenceOptions : IPersistenceOptions
    {
        public ISimpleFilesystemFacade SimpleFSFacade { get; protected set; }

        public ISerializationProvider DefaultSerializationProvider => throw new NotImplementedException();

        public Task<bool?> CanDelete(string objectPath) => throw new NotImplementedException();
        public Task Delete(string objectPath) => throw new NotImplementedException();
        public Task<bool> Exists(string objectPath) => throw new NotImplementedException();
        public Task<bool> Exists<T>(string objectPath) => throw new NotImplementedException();
        public Task<bool> TryDelete<T>(string objectPath, bool preview = false) => throw new NotImplementedException();
        public Task<IRetrieveResult<T>> TryRetrieve<T>(TReference fileReference, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null) => throw new NotImplementedException();
        public Task<IPersistenceResult> Update<T>(T obj, string diskPath, Type type = null, PersistenceContext context = null) => throw new NotImplementedException();
        public Task<IPersistenceResult> Upsert<T>(T obj, string diskPath, Type type = null, PersistenceContext context = null) => throw new NotImplementedException();
        public Task<IPersistenceResult> Write<T>(T obj, string diskPath, Type type = null, PersistenceContext context = null, bool requireOverwrite = false, bool allowOverwrite = false, TPersistenceOptions options = default) => throw new NotImplementedException();
    }
}
#endif