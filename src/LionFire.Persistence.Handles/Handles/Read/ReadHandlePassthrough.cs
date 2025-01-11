#nullable enable
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Data.Async.Gets;
using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace LionFire.Persistence.Handles;

/// <summary>
/// Convenience class that combines Reference and Handle.  
/// Implementers can implement a static implicit operator from string to provide easy (implicit) conversion from a string to a particular IReference type.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TReference"></typeparam>
public class ReadHandlePassthrough<TValue, TReference>
    : IReadHandle<TValue>
    , IReferencable<TReference>
    , IHasReadHandle<TValue>
    where TReference : IReference<TValue>
    where TValue : class
{
    #region Reference

    [SetOnce]
    public TReference? Reference
    {
        get => reference;
        set
        {

            if (EqualityComparer<TReference>.Default.Equals(reference, value)) return;
            if (!EqualityComparer<TReference>.Default.Equals(reference, default)) throw new AlreadySetException();
            reference = value;
        }
    }
    private TReference? reference;

    IReference<TValue> IReferencableAsValueType<TValue>.Reference => Reference;

    #endregion

    #region Lifecycle

    public ReadHandlePassthrough() { }
    public ReadHandlePassthrough(IReadHandle<TValue> handle) { this.handle = handle; Reference = (TReference?)handle?.Reference; }

    #endregion

    #region ReadHandle

    [Ignore(LionSerializeContext.AllSerialization)]
    public IReadHandle<TValue>? ReadHandle => handle ??= Reference?.GetReadHandle<TValue>();
    protected IReadHandle<TValue>? handle;
    IReadHandle<TValue>? IHasReadHandle<TValue>.ReadHandle => ReadHandle;
    IReadHandleBase<object> IHasReadHandle.ReadHandle => (IReadHandle<object>)ReadHandle;

    #region Pass-thru

    public Type? Type => ReadHandle?.Type ?? (Reference as ITypedReference)?.Type ?? typeof(TValue);

    IReference IReferencable.Reference => ReadHandle.Reference;

    public string? Key => ReadHandle?.Key;

    public PersistenceFlags Flags => ReadHandle.Flags;

    public bool HasValue => ReadHandle?.HasValue == true;
    public ITask<IGetResult<TValue>> Resolve() => ReadHandle.Get();
    public ITask<IGetResult<TValue>> GetIfNeeded() => ReadHandle.GetIfNeeded();
    public IGetResult<TValue> QueryGetResult() => ReadHandle.QueryGetResult();
    public void DiscardValue() => ReadHandle.DiscardValue();

    public ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        return ReadHandle.Get(cancellationToken);
    }

    public void Discard() => ReadHandle?.Discard();

    public Task<bool> Exists() => ReadHandle.Exists();

    public TValue? ReadCacheValue => ReadHandle?.ReadCacheValue;

    #endregion

    #endregion

    public TValue? Value
    {
        get => ReadHandle == null ? default : ReadHandle.Value;
        protected set
        {
            if (handle != null) throw new AlreadySetException($"{nameof(ReadHandle)} is already set");

            handle = Reference != null 
                ? Reference.GetReadHandlePreresolved<TValue>(value).handle 
                : value.ToObjectHandle();
        }
    }

    public IObservable<ITask<IGetResult<TValue>>> GetOperations => ReadHandle?.GetOperations ?? Observable.Empty<ITask<IGetResult<TValue>>>();

}
