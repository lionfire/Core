using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Data.Gets;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    public class PersisterWriteHandle<TPersisterReference, TValue, TPersister> : PersisterWriteHandle<TPersisterReference, TValue, TPersister, IReference<TValue>>
       where TPersister : IPersister<TPersisterReference>
       where TPersisterReference : IReference
    {
        protected PersisterWriteHandle() { }
        protected PersisterWriteHandle(IReference<TValue> reference, TValue preresolvedValue)
            : base(reference, preresolvedValue) { }

        public PersisterWriteHandle(TPersister persister, IReference<TValue> reference, TValue preresolvedValue = default)
            : base(persister, reference, preresolvedValue) { }
    }

    public  class PersisterWriteHandle<TPersisterReference, TValue, TPersister, TReference> 
        : WriteHandle<TReference, TValue>
        , IPersisterHandle<TPersisterReference, TPersister>
        , IReferencable<TPersisterReference>
        //where TValue : class
        where TReference : IReference<TValue>
        where TPersister : IPersister<TPersisterReference>
        where TPersisterReference : IReference
    {
        TPersisterReference IReferencable<TPersisterReference>.Reference => (TPersisterReference)(object)Reference; // HARDCAST

        protected PersisterWriteHandle() { }

        protected PersisterWriteHandle(TReference reference) : base(reference) { }
        protected PersisterWriteHandle(TReference reference, TValue prestagedValue) : base(reference, prestagedValue) { }
        public PersisterWriteHandle(TPersister persister, TReference reference) : base(reference) => Persister = persister;
        public PersisterWriteHandle(TPersister persister, TReference reference, TValue prestagedValue) : base(reference, prestagedValue) => Persister = persister;

        public TPersister Persister { get; protected set; }

        IPersister<TPersisterReference> IPersisterHandle<TPersisterReference>.Persister => Persister;

        //public override event Action<PersistenceEvent<TValue>> PersistenceStateChanged;

        //public override void RaisePersistenceEvent(PersistenceEvent<TValue> ev) => throw new NotImplementedException();

        protected override async Task<ITransferResult> UpsertImpl() => await Persister.Upsert(this, ProtectedValue);

        protected override async Task<ITransferResult> DeleteImpl() => await Persister.Delete(this);
    }
}
