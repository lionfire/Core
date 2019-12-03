using LionFire.Referencing;
using LionFire.Resolves;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    public abstract class PersisterReadWriteHandle<TReference, TValue> : ReadWriteHandle<TReference, TValue>, IPersisterHandle<TReference>
        where TReference : IReference
    {
        protected PersisterReadWriteHandle() { }
        protected PersisterReadWriteHandle(TReference reference) : base(reference) { }

        public abstract IPersister<TReference> Persister { get; protected set; }


        public override event Action<PersistenceEvent<TValue>> PersistenceStateChanged;

        public override ILazyResolveResult<TValue> QueryValue() => throw new NotImplementedException();
        public override void RaisePersistenceEvent(PersistenceEvent<TValue> ev) => throw new NotImplementedException();
        protected override Task<IResolveResult<TValue>> ResolveImpl() => throw new NotImplementedException();

        protected override async Task<IPersistenceResult> UpsertImpl() => await Persister.Upsert(this, ProtectedValue);
    }
}
