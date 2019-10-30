#nullable enable
using LionFire.Referencing;
using LionFire.Resolvables;
using LionFire.Resolves;
using LionFire.Results;
using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    public class NullReadHandle<T> : IReadHandleEx<T>
    {

        public T Value => default(T);
        public bool HasValue => false;

        public string Key => null;

        public bool IsResolved => true;

        public IReference Reference => null;

        public PersistenceState State => PersistenceState.UpToDate;

        //public event Action<RH<T>, HandleEvents> HandleEvents;
        //public event Action<RH<T>, T, T> ObjectReferenceChanged;

        public event Action<bool> IsResolvedChanged { add { } remove { } }

        //event Action<INotifyingWrapper<T>, T, T> INotifyingWrapper<T>.ObjectChanged { add { } remove { } }

        public event Action<RH<T>> ObjectChanged { add { } remove { } }

        public void DiscardValue() { }
        public Task<bool> Exists(bool forceCheck = false) => Task.FromResult(true);


        public ITask<ILazyResolveResult<T>> GetValue() => Task.FromResult<ILazyResolveResult<T>>(LazyResolveResultNoop<T>.Instance).AsITask();
        public ITask<IResolveResult<T>> Resolve() => Task.FromResult<IResolveResult<T>>(NoopRetrieveResult).AsITask();
        public Task<bool> TryResolveObject() => Task.FromResult(true);

        public static readonly RetrieveResult<T> NoopRetrieveResult = new RetrieveResult<T>()
        {
            Value = default,
            Flags = PersistenceResultFlags.Success | PersistenceResultFlags.Noop,
            Error = null
        };
    }

}
#nullable restore