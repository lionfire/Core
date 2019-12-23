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
        protected PersisterReadHandle(TReference reference) : base(reference) { }
        public PersisterReadHandle(TPersister persister, TReference reference) : base(reference) => Persister = persister;

        public IPersister<TReference> Persister { get; protected set; }


        protected override async ITask<IResolveResult<TValue>> ResolveImpl() => await Persister.Retrieve<TReference, TValue>(Reference).ConfigureAwait(false);
    }

}
