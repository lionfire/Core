using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Data.Async.Gets;
using LionFire.Results;
using LionFire.Structures;
using Microsoft.Extensions.Logging.EventSource;
using MorseCode.ITask;
using System;
using System.Threading;
using System.Threading.Tasks;
using LionFire.Data.Async.Sets;
using System.Reactive.Subjects;

namespace LionFire.Persistence.Handles;

/// <summary>
/// Convenience class that combines Reference and Handle
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TReference"></typeparam>
public class ReadWriteHandlePassthrough<TValue, TReference> 
    : IReadWriteHandle<TValue>
    , IReferencable<TReference>
    , IHasReadWriteHandle<TValue>
    where TReference : IReference<TValue>
{
    #region Construction

    public ReadWriteHandlePassthrough() { }
    public ReadWriteHandlePassthrough(IReadWriteHandle<TValue> handle)
    {
        this.readWriteHandle = handle;
        Reference = (TReference)handle?.Reference;
    }

    #endregion


    public TReference Reference { get; set; }
    IReference<TValue> IReferencableAsValueType<TValue>.Reference => Reference;

    public IReadWriteHandle<TValue> ReadWriteHandle => readWriteHandle ??= Reference?.GetReadWriteHandle<TValue>();
    protected IReadWriteHandle<TValue> readWriteHandle;

    [Ignore]
    public TValue Value
    {
        get => ReadWriteHandle.ReadCacheValue; 
        set
        {
            if (ReadWriteHandle != null)
            {
                ReadWriteHandle.StagedValue = value;
            }
            else
            {
                if (Reference != null)
                {
                    readWriteHandle = Reference.GetReadWriteHandlePrestaged<TValue>(value, overwriteValue: true).handle;
                }
                else
                {
                    // ENH - optional ability to detect missing Reference at set-time?
                    //if (IsReferenceRequired)
                    //{
                    //    throw new Exception();
                    //}
                    readWriteHandle = value.GetObjectReadWriteHandle();
                }
            }
        }
    }

    // ENH: Option to return Value from StagedValue if HasStagedValue is true, or always return ReadCacheValue
    TValue IReadWrapper<TValue>.Value => ReadWriteHandle.HasStagedValue ? ReadWriteHandle.StagedValue : ReadWriteHandle.ReadCacheValue;

    TValue IWriteWrapper<TValue>.Value { set => ReadWriteHandle.StagedValue = value; }

    public string Key => ReadWriteHandle.Key;

    public bool HasValue => ReadWriteHandle.HasValue;

    public Type Type => ReadWriteHandle.Type;

    IReference IReferencable.Reference => ReadWriteHandle.Reference;

    public PersistenceFlags Flags => ReadWriteHandle.Flags;

    public TValue? ReadCacheValue => ReadWriteHandle.ReadCacheValue;

    public TValue? StagedValue { get => ReadWriteHandle.StagedValue; set => ReadWriteHandle.StagedValue = value; }
    public bool HasStagedValue { get => ReadWriteHandle.HasStagedValue; set => ReadWriteHandle.HasStagedValue = value; }

    public ITask<IGetResult<TValue>> GetIfNeeded() => ReadWriteHandle.GetIfNeeded();
    public ITask<IGetResult<TValue>> GetOrInstantiateValue() => ReadWriteHandle.GetOrInstantiateValue();
    public IGetResult<TValue> QueryValue() => ReadWriteHandle.QueryValue();
    public Task<ISetResult<TValue>> Set(TValue value, CancellationToken cancellationToken = default) => ReadWriteHandle.Set(value, cancellationToken);
    public ITask<IGetResult<TValue>> Get() => ReadWriteHandle.Get();
    public Task<ISetResult> Set() => ReadWriteHandle.Set();
    public Task<bool?> Delete() => ReadWriteHandle.Delete();
    public void MarkDeleted() => ReadWriteHandle.MarkDeleted();
    public void DiscardValue() => ReadWriteHandle.DiscardValue();

    public ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        return ReadWriteHandle.Get(cancellationToken);
    }

    public void DiscardStagedValue()
    {
        ReadWriteHandle.DiscardStagedValue();
    }

    public void Discard()
    {
        ReadWriteHandle.Discard();
    }

    public Task<ISetResult> Set(CancellationToken cancellationToken = default)
    {
        return ReadWriteHandle.Set(cancellationToken);
    }

    public Task<bool> Exists() => ReadWriteHandle.Exists();

    public IObservable<ITask<IGetResult<TValue>>> GetOperations => readWriteHandle.GetOperations;

}
