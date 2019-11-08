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

#pragma warning disable CS8603 // Possible null reference return. // TODO - uncomment this and use nullable types
    public class NullReadHandle<T> : IReadHandle<T>
        where T : class
    {

        public T Value => default;
        public bool HasValue => false;

        public string Key => null;

        public bool IsResolved => true;


        public IReference Reference => default;


        public PersistenceFlags Flags => PersistenceFlags.UpToDate;

        //public event Action<RH<T>, HandleEvents> HandleEvents;
        //public event Action<RH<T>, T, T> ObjectReferenceChanged;

        public event Action<bool> IsResolvedChanged { add { } remove { } }

        //event Action<INotifyingWrapper<T>, T, T> INotifyingWrapper<T>.ObjectChanged { add { } remove { } }

        public event Action<IReadHandleBase<T>> ObjectChanged { add { } remove { } }

        public void DiscardValue() { }
        public Task<bool> Exists(bool forceCheck = false) => Task.FromResult(true);


        public ITask<ILazyResolveResult<T>> GetValue() => Task.FromResult<ILazyResolveResult<T>>(ResolveResultNoop<T>.Instance).AsITask();
        public ITask<IResolveResult<T>> Resolve() => Task.FromResult<IResolveResult<T>>(NoopRetrieveResult).AsITask();
        public Task<bool> TryResolveObject() => Task.FromResult(true);
        public ILazyResolveResult<T> QueryValue() => ResolveResultNoop<T>.Instance;

        public static readonly RetrieveResult<T> NoopRetrieveResult = new RetrieveResult<T>()
        {
            Value = default,
            Flags = PersistenceResultFlags.Success | PersistenceResultFlags.Noop,
            Error = null
        };
    }
#pragma warning restore CS8603 // Possible null reference return.

}
#nullable restore