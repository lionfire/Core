using LionFire.Persistence.Handles;
using LionFire.Referencing;

namespace LionFire.Persistence.Persisters
{
    public abstract class PersisterReadWriteHandleBase<TReference, TValue> : ReadWriteHandleBase<TReference, TValue>, IPersisterHandle<TReference>
        where TReference : IReference<TValue>
    {
        public IPersister<TReference> Persister { get; protected set; }

        public PersisterReadWriteHandleBase(IPersister<TReference> persister) => Persister = persister;
        public PersisterReadWriteHandleBase(IPersister<TReference> persister, TReference reference) : base(reference) => Persister = persister;


        //public override event Action<PersistenceEvent<TValue>> PersistenceStateChanged;

        //public override ILazyGetResult<TValue> QueryValue() => throw new NotImplementedException();
        //public override void RaisePersistenceEvent(PersistenceEvent<TValue> ev) => throw new NotImplementedException();
        //protected override Task<IGetResult<TValue>> ResolveImpl() => throw new NotImplementedException();

        //protected override async Task<ITransferResult> UpsertImpl() =>
        //    await Persister.Upsert<IReference, TValue>(this, ProtectedValue);
    }



}
