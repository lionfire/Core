#nullable enable
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Data.Gets;
using MorseCode.ITask;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles;

/// <summary>
/// Convenience class that combines Reference and Handle.  
/// Implementors can implement a static implicit operator from string to provide easy (implicit) conversion from a string to a particular IReference type.
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
    public ReadHandlePassthrough(IReadHandle<TValue> handle) { this.handle = handle; Reference = (TReference?) handle?.Reference; }

    #endregion

    #region ReadHandle

    [Ignore(LionSerializeContext.AllSerialization)]
    public IReadHandle<TValue>? ReadHandle => handle ??= Reference?.GetReadHandle<TValue>();
    protected IReadHandle<TValue>? handle;
    IReadHandle<TValue>? IHasReadHandle<TValue>.ReadHandle => ReadHandle;
    IReadHandleBase<object> IHasReadHandle.ReadHandle => (IReadHandle<object>)ReadHandle;

    #region Pass-thru

    public Type? Type => ReadHandle?.Type;

    IReference IReferencable.Reference => ReadHandle.Reference;

    public string? Key => ReadHandle?.Key;

    public PersistenceFlags Flags => ReadHandle.Flags;

    public bool HasValue => ReadHandle?.HasValue == true;
    public ITask<IGetResult<TValue>> Resolve() => ReadHandle.Get();
    public ITask<ILazyGetResult<TValue>> GetIfNeeded() => ReadHandle.GetIfNeeded();
    public ILazyGetResult<TValue> QueryValue() => ReadHandle.QueryValue();
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

    public TValue Value
    {
        get => ReadHandle == null ? default : ReadHandle.Value;
        protected set
        {
            if (handle != null) throw new AlreadySetException($"{nameof(ReadHandle)} is already set");
            if(Reference != null)
            {
                var result = Reference.GetReadHandlePreresolved<TValue>(value);
                handle = result.handle;
                if(!result.usedPreresolved) { }
            } else
            {
                handle = value.ToObjectHandle();
            }
        }
    }

    
}
