using LionFire.CouchDB;
using LionFire.Data;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Persistence.CouchDB
{
    /// <summary>
    /// Use LionFire JSON serialization to serialize/deserialize .NET objects to JSON which then gets persisted to MyCouchDB in document mode.
    /// </summary>
    public class CouchDBSerializingPersister : SerializingPersisterBase<CouchDBPersisterOptions>, IPersister<ICouchDBReference>, ICouchDBPersisterInternal
    {
        #region Dependencies

        public ConnectionManager<CouchDBConnection, CouchDBConnectionOptions> ConnectionManager { get; }

        #endregion

        #region Construction

        public CouchDBSerializingPersister(ConnectionManager<CouchDBConnection, CouchDBConnectionOptions> connectionManager, ISerializationProvider serializationProvider, SerializationOptions serializationOptions) : base(serializationOptions)
        {
            SerializationProvider = serializationProvider;
        }

        #endregion

        #region Read

        public Task<ITransferResult> Exists<TValue>(IReferencable<ICouchDBReference> referencable) => throw new NotImplementedException();
        public Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<ICouchDBReference> referencable) => throw new NotImplementedException();

        #endregion

        #region Write

        public async Task<ITransferResult> Create<TValue>(IReferencable<ICouchDBReference> referencable, TValue value)
        {        var r = referencable.Reference;
            var client = this.connectionForReference(r).MyCouchClient;

            var jsonStrategies = SerializationProvider.Strategies.Where(s => s.Formats.Where(f => f.FileExtensions.Contains("json")).Any());

            throw new NotImplementedException("NEXT: get serializationProvider to serialize using json file extension or application/json format.");

            //POST with server generated id
            await client.Documents.PostAsync(@"{""name"":""Daniel""}");

            return TransferResult.Success;
        }

        public Task<ITransferResult> Update<TValue>(IReferencable<ICouchDBReference> referencable, TValue value)
        {
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


            await client.Entities.PostAsync(new { Value = value }); // Using anonymous entities

            return new PersistenceResult(TransferResultFlags.Success);
        }

        #endregion

        #region Delete

        public Task<ITransferResult> Delete(IReferencable<ICouchDBReference> referencable) => throw new NotImplementedException();

        #endregion

        public Task<IRetrieveResult<IEnumerable<string>>> List(IReferencable<ICouchDBReference> referencable, ListFilter filter = null) => throw new NotImplementedException();
        public Task<IRetrieveResult<IEnumerable<Listing<T>>>> List<T>(IReferencable<ICouchDBReference> referencable, ListFilter filter = null) => throw new NotImplementedException();
    }
}
