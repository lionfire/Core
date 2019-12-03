using LionFire.Referencing;
using LionFire.Resolves;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    public abstract class PersisterReadHandle<TReference, TValue> : ReadHandle<TReference, TValue>, IPersisterHandle<TReference>
    where TReference : IReference
    {
        protected PersisterReadHandle() { }
        protected PersisterReadHandle(TReference reference) : base(reference) { }

        public abstract IPersister<TReference> Persister { get; protected set; }


        protected override ITask<IResolveResult<TValue>> ResolveImpl() => throw new NotImplementedException();

    }

}
