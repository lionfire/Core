#nullable enable
using LionFire;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolves;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    // REVIEW: Is there a point to having TReference as a generic parameter, or should it just be replaced with IReference<TValue>?

    public class PersisterReadHandle<TPersisterReference, TValue, TPersister> : PersisterReadHandle<TPersisterReference, TValue, TPersister, IReference<TValue>>
       where TPersister : IPersister<TPersisterReference>
       where TPersisterReference : IReference
    {
        protected PersisterReadHandle() { }
        protected PersisterReadHandle(IReference<TValue> reference, TValue preresolvedValue) 
            : base(reference, preresolvedValue) { }

        public PersisterReadHandle(TPersister persister, IReference<TValue> reference, TValue? preresolvedValue = default) 
            : base(persister, reference, preresolvedValue) { }
        
    }

    [Ignore] // REVIEW - if good, put on other Persister*Handle classes.
    public class PersisterReadHandle<TPersisterReference, TValue, TPersister, TReference> 
        : ReadHandle<TReference, TValue>
        , IPersisterHandle<TPersisterReference, TPersister>
       where TReference : IReference<TValue>
       where TPersister : IPersister<TPersisterReference>
       where TPersisterReference : IReference
    {
        protected PersisterReadHandle() { }
        protected PersisterReadHandle(TReference reference, TValue preresolvedValue) : base(reference, preresolvedValue) { }
        public PersisterReadHandle(TPersister persister, TReference reference, TValue? preresolvedValue = default) 
            : base(reference, preresolvedValue)
        {
            Persister = persister ?? throw new ArgumentNullException(nameof(persister));
        }
        

        [Ignore(LionSerializeContext.AllSerialization)]
        public TPersister? Persister { get; protected set; }
        IPersister<TPersisterReference>? IPersisterHandle<TPersisterReference>.Persister => Persister;

        protected override async ITask<IResolveResult<TValue>> ResolveImpl() => await Persister.Retrieve<TPersisterReference, TValue>((TPersisterReference)(object)Reference).ConfigureAwait(false); // HARDCAST
    }
}
