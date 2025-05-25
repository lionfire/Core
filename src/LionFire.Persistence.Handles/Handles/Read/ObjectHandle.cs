#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.Persistence.Handles;
using LionFire.Structures;
using LionFire.Referencing;
using LionFire.Data.Async.Gets;
using MorseCode.ITask;
using LionFire.Results;
using System.Threading;
using LionFire.Data.Async.Sets;

namespace LionFire.Persistence.Handles;

/// <summary>
/// REVIEW: Incomplete? / needs design analysis
/// Reference type: NamedReference
/// Read-only Handles to .NET object references (can be null) -- can be named and then retrieved by the handle registry system.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class ObjectHandle<TValue> : ReadHandleBase<NamedReference<TValue>, TValue>, IReadWriteHandle<TValue>
{
    #region Identity

    IReference<TValue> IReferenceableAsValueType<TValue>.Reference => Reference;

    string IKeyed<string>.Key => Reference?.Key;

    #endregion

    #region Lifecycle

    public ObjectHandle() { }

    public ObjectHandle(TValue? initialValue)
    {
        InitValue(initialValue);
    }

    public ObjectHandle(NamedReference<TValue> reference, TValue initialValue = default) : base(reference)
    {
        if (!EqualityComparer<TValue>.Default.Equals(initialValue, default))
        {
            InitValue(initialValue);
        }
    }

    private void InitValue(TValue? initialValue)
    {
        ReadCacheValue = initialValue;
        HasValue = ReadCacheValue != null;
    }
    private void Dispose()
    {
        isDisposed = true;
        ProtectedValue = default;
    }

    #endregion

    #region State

    private bool isDisposed = false;

    //protected TValue? ProtectedValue; // OLD - REVIEW - ok to use ReadCacheValue?
    protected TValue? ProtectedValue { get => ReadCacheValue; set => ReadCacheValue = value; }
    TValue? IWrapper<TValue>.Value
    {
        get
        {
            if (isDisposed) throw new ObjectDisposedException("");
            return ProtectedValue;
        }
        set => ThrowCannotSet();
    }
    TValue IWriteWrapper<TValue>.Value { set => ThrowCannotSet(); }
    public TValue? StagedValue
    {
        get => ReadCacheValue;
        set
        {
            if (ReferenceEquals(value, ReadCacheValue)) return;
            ThrowCannotSet();
        }
    }
    public bool HasStagedValue { get; set; }

    void ThrowCannotSet() => throw new InvalidOperationException("Cannot set the value on an ObjectHandle after creation");

    #endregion

    #region Get

    protected override ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default)
    {
        if (HasValue)
        {
            return Task.FromResult<IGetResult<TValue>>(new RetrieveResult<TValue>()
            {
                Flags = TransferResultFlags.Success,
                Value = ProtectedValue,
            }).AsITask();
        }
        else
        {
            return Task.FromResult<IGetResult<TValue>>(RetrieveResult<TValue>.NotFound).AsITask();
        }
    }

    #endregion

    #region ReadWrite

    public ITask<IGetResult<TValue>> GetOrInstantiateValue()
    {
        if (HasValue) return Task.FromResult((IGetResult<TValue>)RetrieveResult<TValue>.Noop(Value)).AsITask();
        ProtectedValue = Activator.CreateInstance<TValue>();
        return Task.FromResult((IGetResult<TValue>)new RetrieveResult<TValue>(Value, TransferResultFlags.Success | TransferResultFlags.Instantiated)).AsITask();
    }

    #endregion

    #region Write

    #region Set

    public Task<ISetResult<TValue>> Set(TValue value, CancellationToken cancellationToken = default)
    {
        if (!ReferenceEquals(value, ProtectedValue))
        {
            ThrowCannotSet();
        }
        return Task.FromResult<ISetResult<TValue>>(SetResult<TValue>.NoopSuccess(value));
    }

    public Task<ISetResult> Set(CancellationToken cancellationToken = default)
    {
        if (isDeletePending)
        {
            Delete();
            return Task.FromResult<ISetResult>((SetResult<TValue>.DeleteSuccess));
        }
        else
        {
            return Task.FromResult<ISetResult>(SetResult<TValue>.NoopSuccess(ProtectedValue));
        }
    }

    #endregion

    public Task<bool?> Delete()
    {
        Dispose();
        return Task.FromResult<bool?>(true);
    }
    bool isDeletePending = false;
    public void MarkDeleted() => isDeletePending = true;

    public void DiscardStagedValue()
    {
        throw new NotImplementedException();
    }

    public Task<bool> Exists()
    {
        throw new NotImplementedException();
    }

    #endregion

}
