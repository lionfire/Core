using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolves;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    public class PersisterReadWriteHandle<TPersisterReference, TValue, TPersister> : PersisterReadWriteHandle<TPersisterReference, TValue, TPersister, IReference<TValue>>
       where TPersister : IPersister<TPersisterReference>
       where TPersisterReference : IReference
    {
        protected PersisterReadWriteHandle() { }
        protected PersisterReadWriteHandle(IReference<TValue> reference, TValue preresolvedValue)
            : base(reference, preresolvedValue) { }

#nullable enable
        public PersisterReadWriteHandle(TPersister persister, IReference<TValue> reference, TValue? preresolvedValue = default)
            : base(persister, reference, preresolvedValue) { }
        //public PersisterReadWriteHandle(TPersister persister, IReference reference, TValue? preresolvedValue = default)
            //: base(persister, reference, preresolvedValue) { }
#nullable disable
    }

    public  class PersisterReadWriteHandle<TPersisterReference, TValue, TPersister, TReference> 
        : ReadWriteHandle<TReference, TValue>
        , IPersisterHandle<TPersisterReference, TPersister>
        , IReferencable<TPersisterReference>
        where TReference : IReference<TValue>
        where TPersister : IPersister<TPersisterReference>
        where TPersisterReference : IReference
    {
        TPersisterReference IReferencable<TPersisterReference>.Reference => (TPersisterReference)(object)Reference; // HARDCAST

        protected PersisterReadWriteHandle() { }

        protected PersisterReadWriteHandle(TReference reference, TValue preresolvedValue = default) : base(reference, preresolvedValue) { }
        public PersisterReadWriteHandle(TPersister persister, TReference reference, TValue preresolvedValue = default) : base(reference, preresolvedValue)
        {
            Persister = persister;
        }
        //public PersisterReadWriteHandle(TPersister persister, IReference reference, TValue preresolvedValue = default) : base(reference, preresolvedValue)
        //{
        //    Persister = persister;
        //}


        public TPersister Persister { get; protected set; }

        IPersister<TPersisterReference> IPersisterHandle<TPersisterReference>.Persister => Persister;

        //public override event Action<PersistenceEvent<TValue>> PersistenceStateChanged;

        //public override ILazyResolveResult<TValue> QueryValue() => throw new NotImplementedException();
        //public override void RaisePersistenceEvent(PersistenceEvent<TValue> ev) => throw new NotImplementedException();
        protected override async ITask<IResolveResult<TValue>> ResolveImpl() => await Persister.Retrieve<TPersisterReference, TValue>((TPersisterReference)(object)Reference).ConfigureAwait(false);

        protected override async Task<IPersistenceResult> UpsertImpl() => await Persister.Upsert(this, ProtectedValue);

        protected override async Task<IPersistenceResult> DeleteImpl() => await Persister.Delete(this);

    }
}
