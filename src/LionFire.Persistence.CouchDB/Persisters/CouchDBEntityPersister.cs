using LionFire.CouchDB;
using LionFire.Data;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Serialization;
using LionFire.Vos.Handles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.CouchDB
{
    internal interface ICouchDBPersisterInternal
    {
        ConnectionManager<CouchDBConnection, CouchDBConnectionOptions> ConnectionManager { get; }

    }

    internal static class CouchDBPersisterCommon
    {
        internal static CouchDBConnection connectionForReference(this ICouchDBPersisterInternal persister, ICouchDBReference r)
        {
            switch (r)
            {
                case CouchDBReference d:
                    return persister.ConnectionManager[d.Persister];
                case CouchDBDirectReference d:
                    break;
                default:
                    break;
            }
            throw new NotImplementedException("TODO");
        }
    }

    //public class CouchDBPersisterBase : PersisterBase<CouchDBPersisterOptions>
    ////, IFilesystemPersistence<TReference, TPersistenceOptions>
    ////, IWriter<string>
    ////, IReader<string>
    //{
        

    //    #region Construction

    //    public CouchDBPersisterBase(ConnectionManager<CouchDBConnection, CouchDBConnectionOptions> connectionManager, SerializationOptions serializationOptions) : base ()
    //    {
    //        ConnectionManager = connectionManager;
    //    }

    //    #endregion

    //    #region Connections



    //    #endregion
    //}

    /// <summary>
    /// Uses MyCouchDB in Entity mode to persist/depersist .NET objects
    /// </summary>
    public class CouchDBEntityPersister : PersisterBase<CouchDBPersisterOptions>, IPersister<ICouchDBReference>, ICouchDBPersisterInternal
    {
        #region Dependencies

        public ConnectionManager<CouchDBConnection, CouchDBConnectionOptions> ConnectionManager { get; }

        #endregion

        #region Construction

        public CouchDBEntityPersister(ConnectionManager<CouchDBConnection, CouchDBConnectionOptions> connectionManager)
        {
            ConnectionManager = connectionManager;
        }

        #endregion

        #region Read

        public Task<ITransferResult> Exists<TValue>(IReferencable<ICouchDBReference> referencable) => throw new NotImplementedException();
        public Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<ICouchDBReference> referencable) => throw new NotImplementedException();

        #endregion

        #region Write

        public async Task<ITransferResult> Create<TValue>(IReferencable<ICouchDBReference> referencable, TValue value)
        {
            var r = referencable.Reference;
            var client = this.connectionForReference(r).MyCouchClient;

            //POST with server generated id
            await client.Documents.PostAsync(@"{""name"":""Daniel""}");

            return PersistenceResult.Success;
        }
        public Task<ITransferResult> Update<TValue>(IReferencable<ICouchDBReference> referencable, TValue value)
        {
            var r = referencable.Reference;
            var client = this.connectionForReference(r).MyCouchClient;
            throw new NotImplementedException();
        }
        public async Task<ITransferResult> Upsert<TValue>(IReferencable<ICouchDBReference> referencable, TValue value)
        {
            var r = referencable.Reference;
            var client = this.connectionForReference(r).MyCouchClient;

            await client.Documents.PutAsync(r.Id, @"{""name"":""Daniel""}"); //PUT for client generated id
            //await client.Documents.PostAsync(@$"{{""_id"":""{r.Id}"", ""name"":""Daniel""}}"); // POST with client generated id - possible but wrong

            await client.Documents.PutAsync(r.Id, @"docRevision", @"{""name"":""Daniel Wertheim""}"); // PUT for updates
            await client.Documents.PutAsync(r.Id, @"{""_rev"": ""docRevision"", ""name"":""Daniel Wertheim""}"); // PUT for updates with _rev in JSON

            // Using entities
            // var me = new Person { Id = "SomeId", Name = "Daniel" };
            // await client.Entities.PutAsync(value);


            // TODO: what to do about the : class constraint???
            await client.Entities.PostAsync(new { Value = value }); // Using anonymous entities

            return new PersistenceResult(TransferResultFlags.Success);
        }

        #endregion

        #region Delete

        public Task<ITransferResult> Delete(IReferencable<ICouchDBReference> referencable)
        {
            var r = referencable.Reference;
            var client = this.connectionForReference(r).MyCouchClient;

            throw new NotImplementedException();
        }

        #endregion

        public Task<IRetrieveResult<IEnumerable<string>>> List(IReferencable<ICouchDBReference> referencable, ListFilter filter = null) => throw new NotImplementedException();
        public Task<IRetrieveResult<IEnumerable<Listing<T>>>> List<T>(IReferencable<ICouchDBReference> referencable, ListFilter filter = null) => throw new NotImplementedException();
    }
}
