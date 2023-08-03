#nullable enable
using LionFire.Referencing;
using LionFire.Resolvables;
using LionFire.Data.Async.Gets;
using LionFire.Results;
using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace LionFire.Persistence.Handles;


#pragma warning disable CS8603 // Possible null reference return. // TODO - uncomment this and use nullable types
public class NullReadHandle<TValue> : IReadHandle<TValue>
    where TValue : class
{
    public Type Type => typeof(TValue);

    public TValue Value => default;
    public TValue ReadCacheValue => default;

    public bool HasValue => false;

    public string Key => null;

    public bool IsResolved => true;


    public IReference Reference => default;
    IReference<TValue> IReferencableAsValueType<TValue>.Reference => default;


    public PersistenceFlags Flags => PersistenceFlags.UpToDate;


    //public event Action<RH<TValue>, HandleEvents> HandleEvents;
    //public event Action<RH<TValue>, TValue, TValue> ObjectReferenceChanged;

    public event Action<bool> IsResolvedChanged { add { } remove { } }

    //event Action<INotifyingWrapper<TValue>, TValue, TValue> INotifyingWrapper<TValue>.ObjectChanged { add { } remove { } }

    public event Action<IReadHandleBase<TValue>> ObjectChanged { add { } remove { } }

    public void DiscardValue() { }
    public Task<bool> Exists(bool forceCheck = false) => Task.FromResult(true);


    public ITask<IGetResult<TValue>> GetIfNeeded() => Task.FromResult<IGetResult<TValue>>(NoopGetResult2<TValue>.Instance).AsITask();
    public ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default) => Task.FromResult<IGetResult<TValue>>(NoopRetrieveResult).AsITask();
    public Task<bool> TryResolveObject() => Task.FromResult(true);
    public IGetResult<TValue> QueryValue() => NoopGetResult2<TValue>.Instance;

    public void Discard()
    {
        throw new NotImplementedException();
    }

    public Task<bool> Exists() => Task.FromResult(false);

    public static readonly RetrieveResult<TValue> NoopRetrieveResult = new RetrieveResult<TValue>()
    {
        Value = default,
        Flags = TransferResultFlags.Success | TransferResultFlags.Noop,
        Error = null
    };
}
#pragma warning restore CS8603 // Possible null reference return.

#nullable restore