using LionFire.Referencing;
using LionFire.Resolvables;
using LionFire.Results;
using LionFire.Structures;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    public class NullReadHandle<T> : IReadHandleEx<T>
    {

        public T Object => default(T);
        public bool HasValue => false;

        public string Key => null;

        public bool IsResolved => true;

        public IReference Reference => null;

        public PersistenceState State => PersistenceState.UpToDate;

        public event Action<RH<T>, HandleEvents> HandleEvents;
        public event Action<RH<T>, T, T> ObjectReferenceChanged;

        public event Action<bool> IsResolvedChanged { add { } remove { } }

        event Action<INotifyingWrapper<T>, T, T> INotifyingWrapper<T>.ObjectChanged { add { } remove { } }

        public event Action<RH<T>> ObjectChanged { add { } remove { } }

        public void DiscardObject() { }
        public Task<bool> Exists(bool forceCheck = false) => Task.FromResult(true);


        public Task<(bool HasObject, object Object)> Get() => Task.FromResult<(bool,object)>((true, null));
        public Task<IResolveResult> ResolveAsync() => Task.FromResult((IResolveResult)SuccessResult.Success);
        public Task<bool> TryResolveObject() => Task.FromResult(true);
    }

}
