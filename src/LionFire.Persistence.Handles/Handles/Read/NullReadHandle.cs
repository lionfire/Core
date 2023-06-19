#nullable enable
using LionFire.Referencing;
using LionFire.Resolvables;
using LionFire.Data.Async.Gets;
using LionFire.Results;
using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{

#pragma warning disable CS8603 // Possible null reference return. // TODO - uncomment this and use nullable types
    public class NullReadHandle<TValue> : IReadHandle<TValue>
        where TValue : class
    {
        public Type Type => typeof(TValue);

        public TValue Value => default;
        public bool HasValue => false;

        public string Key => null;

        public bool IsResolved => true;


        public IReference Reference => default;
        IReference<TValue> IReferencableAsValueType<TValue>.Reference => default;


        public PersistenceFlags Flags => PersistenceFlags.UpToDate;


        //public event Action<RH<T>, HandleEvents> HandleEvents;
        //public event Action<RH<T>, T, T> ObjectReferenceChanged;

        public event Action<bool> IsResolvedChanged { add { } remove { } }

        //event Action<INotifyingWrapper<T>, T, T> INotifyingWrapper<T>.ObjectChanged { add { } remove { } }

        public event Action<IReadHandleBase<TValue>> ObjectChanged { add { } remove { } }

        public void DiscardValue() { }
        public Task<bool> Exists(bool forceCheck = false) => Task.FromResult(true);


        public ITask<ILazyGetResult<TValue>> TryGetValue() => Task.FromResult<ILazyGetResult<TValue>>(ResolveResultNoop<TValue>.Instance).AsITask();
        public ITask<IGetResult<TValue>> Resolve() => Task.FromResult<IGetResult<TValue>>(NoopRetrieveResult).AsITask();
        public Task<bool> TryResolveObject() => Task.FromResult(true);
        public ILazyGetResult<TValue> QueryValue() => ResolveResultNoop<TValue>.Instance;

        public static readonly RetrieveResult<TValue> NoopRetrieveResult = new RetrieveResult<TValue>()
        {
            Value = default,
            Flags = TransferResultFlags.Success | TransferResultFlags.Noop,
            Error = null
        };
    }
#pragma warning restore CS8603 // Possible null reference return.

}
#nullable restore