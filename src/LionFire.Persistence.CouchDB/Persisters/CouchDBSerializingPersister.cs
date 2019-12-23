using LionFire.CouchDB;
using LionFire.Data;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Serialization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Persistence.CouchDB
{
    /// <summary>
    /// Use LionFire JSON serialization to serialize/deserialize .NET objects to JSON which then gets persisted to MyCouchDB in document mode.
    /// </summary>
    public class CouchDBSerializingPersister : CouchDBPersisterBase, IPersister<ICouchDBReference>
    {
        #region Dependencies

        #region SerializationProvider

        [SetOnce]
        public ISerializationProvider SerializationProvider
        {
            get => serializationProvider;
            set
            {
                if (serializationProvider == value) return;
                if (serializationProvider != default) throw new AlreadySetException();
                serializationProvider = value;
            }
        }
        private ISerializationProvider serializationProvider;

        #endregion

        #endregion

        #region Construction

        public CouchDBSerializingPersister(ConnectionManager<CouchDBConnection, CouchDBConnectionOptions> connectionManager, ISerializationProvider serializationProvider) : base(connectionManager)
        {
            SerializationProvider = serializationProvider;
        }

        #endregion

        #region Read

        public Task<IPersistenceResult> Exists(IReferencable<ICouchDBReference> referencable) => throw new NotImplementedException();
        public Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<ICouchDBReference> referencable) => throw new NotImplementedException();

        #endregion

        #region Write

        public async Task<IPersistenceResult> Create<TValue>(IReferencable<ICouchDBReference> referencable, TValue value)
        {        var r = referencable.Reference;
            var client = connectionForReference(r).MyCouchClient;

            var jsonStrategies = serializationProvider.Strategies.Where(s => s.Formats.Where(f => f.FileExtensions.Contains("json")).Any());

            throw new NotImplementedException("NEXT: get serializationProvider to serialize using json file extension or application/json format.");

            //POST with server generated id
            await client.Documents.PostAsync(@"{""name"":""Daniel""}");

            return PersistenceResult.Success;
        }

        public Task<IPersistenceResult> Update<TValue>(IReferencable<ICouchDBReference> referencable, TValue value)
        {
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


            await client.Entities.PostAsync(new { Value = value }); // Using anonymous entities

            return new PersistenceResult(PersistenceResultFlags.Success);
        }

        #endregion

        #region Delete

        public Task<IPersistenceResult> Delete(IReferencable<ICouchDBReference> referencable) => throw new NotImplementedException();

        #endregion
    }
}
