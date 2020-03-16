using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolves;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    public  class PersisterReadWriteHandle<TReference, TValue, TPersister> : ReadWriteHandle<TReference, TValue>, IPersisterHandle<TReference>
        where TReference : IReference
        where TPersister : IPersister<TReference>
    {
        protected PersisterReadWriteHandle() { }

        protected PersisterReadWriteHandle(TReference reference, TValue preresolvedValue = default) : base(reference, preresolvedValue) { }
        public PersisterReadWriteHandle(TPersister persister, TReference reference, TValue preresolvedValue = default) : base(reference, preresolvedValue)
        {
            Persister = persister;
        }

        public  TPersister Persister { get; protected set; }

        IPersister<TReference> IPersisterHandle<TReference>.Persister => Persister;


        //public override event Action<PersistenceEvent<TValue>> PersistenceStateChanged;

        public override ILazyResolveResult<TValue> QueryValue() => throw new NotImplementedException();
        public override void RaisePersistenceEvent(PersistenceEvent<TValue> ev) => throw new NotImplementedException();
        protected override async Task<IResolveResult<TValue>> ResolveImpl() => await Persister.Retrieve<TReference, TValue>(Reference).ConfigureAwait(false);

        protected override async Task<IPersistenceResult> UpsertImpl() => await Persister.Upsert(this, ProtectedValue);

        protected override async Task<IPersistenceResult> DeleteImpl() => await Persister.Delete(this);
    }
}
