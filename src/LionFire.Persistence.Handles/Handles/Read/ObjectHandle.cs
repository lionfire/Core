using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LionFire.Persistence.Handles;
using LionFire.Structures;
using LionFire.Referencing;
using LionFire.Data.Gets;
using MorseCode.ITask;
using LionFire.Results;
using System.Threading;

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

    IReference<TValue> IReferencableAsValueType<TValue>.Reference => Reference;

    string IKeyed<string>.Key => Reference?.Key;

    #endregion

    #region Lifecycle

    public ObjectHandle() { }

    public ObjectHandle(TValue initialValue)
    {
        ProtectedValue = initialValue;
    }

    public ObjectHandle(NamedReference<TValue> reference, TValue initialValue = default) : base(reference)
    {
        if (!EqualityComparer<TValue>.Default.Equals(initialValue, default))
        {
            ProtectedValue = initialValue;
        }
    }

    private void Dispose()
    {
        isDisposed = true;
        ProtectedValue = default;
    }

    #endregion

    #region State

    private bool isDisposed = false;

    protected TValue ProtectedValue;
    TValue IWrapper<TValue>.Value
    {
        get
        {
            if (isDisposed) throw new ObjectDisposedException("");
            return ProtectedValue;
        }
        set => ThrowCannotSet();
    }
    TValue IWriteWrapper<TValue>.Value { set => ThrowCannotSet(); }
    public TValue? StagedValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool HasStagedValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

    public Task<ITransferResult> Set(TValue value, CancellationToken cancellationToken = default)
    {
        if (!ReferenceEquals(value, ProtectedValue))
        {
            ThrowCannotSet();
        }
        return Task.FromResult<ITransferResult>(TransferResult.NoopSuccess);
    }

    public Task<ITransferResult> Set(CancellationToken cancellationToken = default)
    {
        if (isDeletePending)
        {
            Delete();
            return Task.FromResult<ITransferResult>(TransferResult.Success);
        }
        else
        {
            return Task.FromResult<ITransferResult>(TransferResult.NoopSuccess);
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

    #endregion

}
