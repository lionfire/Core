using LionFire.CouchDB;
using LionFire.Data;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Vos.Handles;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.CouchDB
{

    public class CouchDBPersisterBase : PersisterBase<CouchDBPersisterOptions>
    //, IFilesystemPersistence<TReference, TPersistenceOptions>
    //, IWriter<string>
    //, IReader<string>
    {
        #region Dependencies

        protected ConnectionManager<CouchDBConnection, CouchDBConnectionOptions> ConnectionManager { get; }

        #endregion

        #region Construction

        public CouchDBPersisterBase(ConnectionManager<CouchDBConnection, CouchDBConnectionOptions> connectionManager)
        {
            ConnectionManager = connectionManager;
        }

        #endregion

        #region Connections

        protected CouchDBConnection connectionForReference(ICouchDBReference r)
        {
            switch (r)
            {
                case CouchDBReference d:
                    return ConnectionManager[d.Persister];
                case CouchDBDirectReference d:
                    break;
                default:
                    break;
            }
            throw new NotImplementedException("TODO");
        }

        #endregion
    }

    /// <summary>
    /// Uses MyCouchDB in Entity mode to persist/depersist .NET objects
    /// </summary>
    public class CouchDBEntityPersister : CouchDBPersisterBase, IPersister<ICouchDBReference>
    {
        #region Construction

        public CouchDBEntityPersister(ConnectionManager<CouchDBConnection, CouchDBConnectionOptions> connectionManager) : base(connectionManager)
        {
        }

        #endregion

        #region Read

        public Task<IPersistenceResult> Exists<TValue>(IReferencable<ICouchDBReference> referencable) => throw new NotImplementedException();
        public Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<ICouchDBReference> referencable) => throw new NotImplementedException();

        #endregion

        #region Write

        public async Task<IPersistenceResult> Create<TValue>(IReferencable<ICouchDBReference> referencable, TValue value)
        {
            var r = referencable.Reference;
            var client = connectionForReference(r).MyCouchClient;

            //POST with server generated id
            await client.Documents.PostAsync(@"{""name"":""Daniel""}");

            return PersistenceResult.Success;
        }
        public Task<IPersistenceResult> Update<TValue>(IReferencable<ICouchDBReference> referencable, TValue value)
        {
            var r = referencable.Reference;
            var client = connectionForReference(r).MyCouchClient;
            throw new NotImplementedException();
        }
        public async Task<IPersistenceResult> Upsert<TValue>(IReferencable<ICouchDBReference> referencable, TValue value)
        {
            var r = referencable.Reference;
            var client = connectionForReference(r).MyCouchClient;

            await client.Documents.PutAsync(r.Id, @"{""name"":""Daniel""}"); //PUT for client generated id
            //await client.Documents.PostAsync(@$"{{""_id"":""{r.Id}"", ""name"":""Daniel""}}"); // POST with client generated id - possible but wrong

            await client.Documents.PutAsync(r.Id, @"docRevision", @"{""name"":""Daniel Wertheim""}"); // PUT for updates
            await client.Documents.PutAsync(r.Id, @"{""_rev"": ""docRevision"", ""name"":""Daniel Wertheim""}"); // PUT for updates with _rev in JSON

            // Using entities
            // var me = new Person { Id = "SomeId", Name = "Daniel" };
            // await client.Entities.PutAsync(value);


            // TODO: what to do about the : class constraint???
            await client.Entities.PostAsync(new { Value = value }); // Using anonymous entities

            return new PersistenceResult(PersistenceResultFlags.Success);
        }

        #endregion

        #region Delete

        public Task<IPersistenceResult> Delete(IReferencable<ICouchDBReference> referencable)
        {
            var r = referencable.Reference;
            var client = connectionForReference(r).MyCouchClient;

            throw new NotImplementedException();
        }

        #endregion
    }
}
