#nullable enable
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolves;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    public class PersisterReadHandle<TReference, TValue, TPersister> : ReadHandle<TReference, TValue>, IPersisterHandle<TReference>
       where TReference : IReference
       where TPersister : IPersister<TReference>
    {
        protected PersisterReadHandle() { }
        protected PersisterReadHandle(TReference reference, TValue preresolvedValue) : base(reference, preresolvedValue) { }
        public PersisterReadHandle(TPersister persister, TReference reference, TValue preresolvedValue = default) : base(reference, preresolvedValue)
        {
            Persister = persister ?? throw new ArgumentNullException(nameof(persister));
        }

        [Ignore(LionSerializeContext.AllSerialization)]
        public IPersister<TReference>? Persister { get; protected set; }


        protected override async ITask<IResolveResult<TValue>> ResolveImpl() => await Persister.Retrieve<TReference, TValue>(Reference).ConfigureAwait(false);
    }

}
