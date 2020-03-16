using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolves;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    public  class PersisterWriteHandle<TReference, TValue, TPersister> : WriteHandle<TReference, TValue>, IPersisterHandle<TReference> 
        //where TValue : class
        where TReference :  IReference
        where TPersister : IPersister<TReference>
    {
        protected PersisterWriteHandle() { }

        protected PersisterWriteHandle(TReference reference) : base(reference) { }
        protected PersisterWriteHandle(TReference reference, TValue prestagedValue) : base(reference, prestagedValue) { }
        public PersisterWriteHandle(TPersister persister, TReference reference) : base(reference) => Persister = persister;
        public PersisterWriteHandle(TPersister persister, TReference reference, TValue prestagedValue) : base(reference, prestagedValue) => Persister = persister;

        public  TPersister Persister { get; protected set; }

        IPersister<TReference> IPersisterHandle<TReference>.Persister => Persister;

        //public override event Action<PersistenceEvent<TValue>> PersistenceStateChanged;

        //public override void RaisePersistenceEvent(PersistenceEvent<TValue> ev) => throw new NotImplementedException();

        protected override async Task<IPersistenceResult> UpsertImpl() => await Persister.Upsert(this, ProtectedValue);

        protected override async Task<IPersistenceResult> DeleteImpl() => await Persister.Delete(this);
    }
}
